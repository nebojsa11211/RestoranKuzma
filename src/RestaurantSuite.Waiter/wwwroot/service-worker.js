// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [ /\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/ ];
const offlineAssetsExclude = [ /^service-worker\.js$/ ];

async function onInstall(event) {
    console.info('Service worker: Install');

    // Fetch and cache all matching items from the assets manifest
    const assetsRequests = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        .map(asset => new Request(asset.url, { integrity: asset.hash, cache: 'no-cache' }));

    await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));

    // Cache API endpoints for offline support
    const apiEndpoints = [
        '/api/menu',
        '/api/tables'
    ];

    try {
        const cache = await caches.open(`${cacheName}-api`);
        await Promise.all(
            apiEndpoints.map(endpoint =>
                fetch(endpoint)
                    .then(response => {
                        if (response.ok) {
                            return cache.put(endpoint, response);
                        }
                    })
                    .catch(err => console.log('Failed to cache', endpoint, err))
            )
        );
    } catch (error) {
        console.error('Error caching API endpoints:', error);
    }
}

async function onActivate(event) {
    console.info('Service worker: Activate');

    // Delete unused caches
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName && key !== `${cacheName}-api`)
        .map(key => caches.delete(key)));
}

async function onFetch(event) {
    let cachedResponse = null;
    if (event.request.method === 'GET') {
        // For static assets, use cache-first strategy
        const shouldServeIndexHtml = event.request.mode === 'navigate';
        const request = shouldServeIndexHtml ? 'index.html' : event.request;
        const cache = await caches.open(cacheName);
        cachedResponse = await cache.match(request);

        // If not in static cache, check API cache
        if (!cachedResponse && event.request.url.includes('/api/')) {
            const apiCache = await caches.open(`${cacheName}-api`);
            cachedResponse = await apiCache.match(event.request);
        }
    }

    // Network-first strategy with fallback to cache
    return cachedResponse || fetch(event.request)
        .then(response => {
            // Cache successful API GET requests
            if (event.request.method === 'GET' && event.request.url.includes('/api/') && response.ok) {
                const responseClone = response.clone();
                caches.open(`${cacheName}-api`).then(cache => {
                    cache.put(event.request, responseClone);
                });
            }
            return response;
        })
        .catch(error => {
            console.error('Fetch failed; returning offline page instead.', error);

            // Return a custom offline page for navigation requests
            if (event.request.mode === 'navigate') {
                return caches.match('/offline.html') || new Response(
                    '<html><body><h1>You are offline</h1><p>Please check your internet connection.</p></body></html>',
                    { headers: { 'Content-Type': 'text/html' } }
                );
            }

            // For other requests, return cached response or error
            return cachedResponse || new Response('Network error', {
                status: 408,
                headers: { 'Content-Type': 'text/plain' }
            });
        });
}

// Background sync for queued requests
self.addEventListener('sync', event => {
    if (event.tag === 'sync-orders') {
        event.waitUntil(syncOrders());
    }
});

async function syncOrders() {
    console.log('Service worker: Syncing queued orders');
    // Implementation would retrieve queued orders from IndexedDB
    // and attempt to send them to the server
}

// Push notification handling
self.addEventListener('push', event => {
    const data = event.data ? event.data.json() : {};
    const title = data.title || 'Waiter Notification';
    const options = {
        body: data.message || 'You have a new notification',
        icon: '/icon-192.png',
        badge: '/icon-192.png',
        vibrate: [200, 100, 200],
        tag: data.tag || 'notification',
        requireInteraction: data.requireInteraction || false,
        data: {
            url: data.url || '/'
        }
    };

    event.waitUntil(
        self.registration.showNotification(title, options)
    );
});

// Handle notification click
self.addEventListener('notificationclick', event => {
    event.notification.close();
    event.waitUntil(
        clients.openWindow(event.notification.data.url || '/')
    );
});
