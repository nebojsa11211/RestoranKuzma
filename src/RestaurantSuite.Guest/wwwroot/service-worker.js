// Service Worker for Restaurant Suite Guest Portal
// Provides offline support and caching for mobile PWA

const CACHE_NAME = 'restaurant-guest-v1';
const OFFLINE_URL = '/offline.html';

// Assets to cache on install
const ASSETS_TO_CACHE = [
    '/',
    '/index.html',
    '/css/app.css',
    '/css/mobile.css',
    '/css/tailwind.output.css',
    '/manifest.json',
    '/icon-192.png',
    '/icon-512.png'
];

// Install event - cache essential assets
self.addEventListener('install', (event) => {
    console.log('[Service Worker] Installing...');

    event.waitUntil(
        caches.open(CACHE_NAME)
            .then((cache) => {
                console.log('[Service Worker] Caching essential assets');
                return cache.addAll(ASSETS_TO_CACHE);
            })
            .then(() => {
                console.log('[Service Worker] Installation complete');
                return self.skipWaiting();
            })
            .catch((error) => {
                console.error('[Service Worker] Installation failed:', error);
            })
    );
});

// Activate event - clean up old caches
self.addEventListener('activate', (event) => {
    console.log('[Service Worker] Activating...');

    event.waitUntil(
        caches.keys()
            .then((cacheNames) => {
                return Promise.all(
                    cacheNames
                        .filter((cacheName) => cacheName !== CACHE_NAME)
                        .map((cacheName) => {
                            console.log('[Service Worker] Deleting old cache:', cacheName);
                            return caches.delete(cacheName);
                        })
                );
            })
            .then(() => {
                console.log('[Service Worker] Activation complete');
                return self.clients.claim();
            })
    );
});

// Fetch event - serve from cache with network fallback
self.addEventListener('fetch', (event) => {
    const { request } = event;

    // Skip cross-origin requests
    if (!request.url.startsWith(self.location.origin)) {
        return;
    }

    // Handle navigation requests
    if (request.mode === 'navigate') {
        event.respondWith(
            fetch(request)
                .catch(() => {
                    // If offline, serve cached page or offline page
                    return caches.match(request)
                        .then((response) => {
                            return response || caches.match(OFFLINE_URL);
                        });
                })
        );
        return;
    }

    // Cache-first strategy for other requests
    event.respondWith(
        caches.match(request)
            .then((cachedResponse) => {
                if (cachedResponse) {
                    // Return cached version and update cache in background
                    fetchAndCache(request);
                    return cachedResponse;
                }

                // Not in cache, fetch from network
                return fetchAndCache(request);
            })
            .catch((error) => {
                console.error('[Service Worker] Fetch failed:', error);
                throw error;
            })
    );
});

// Helper function to fetch and cache
function fetchAndCache(request) {
    return fetch(request)
        .then((response) => {
            // Check if valid response
            if (!response || response.status !== 200 || response.type === 'error') {
                return response;
            }

            // Clone the response
            const responseToCache = response.clone();

            // Cache the fetched response
            caches.open(CACHE_NAME)
                .then((cache) => {
                    cache.put(request, responseToCache);
                });

            return response;
        });
}

// Background sync for offline orders
self.addEventListener('sync', (event) => {
    console.log('[Service Worker] Background sync:', event.tag);

    if (event.tag === 'sync-orders') {
        event.waitUntil(syncOrders());
    }
});

// Sync pending orders when back online
async function syncOrders() {
    try {
        console.log('[Service Worker] Syncing pending orders...');
        // This would connect to IndexedDB to sync pending orders
        // Implementation depends on your offline storage strategy
        return Promise.resolve();
    } catch (error) {
        console.error('[Service Worker] Order sync failed:', error);
        throw error;
    }
}

// Push notifications (for future order updates)
self.addEventListener('push', (event) => {
    console.log('[Service Worker] Push notification received');

    const options = {
        body: event.data ? event.data.text() : 'New update from Restaurant Suite',
        icon: '/icon-192.png',
        badge: '/icon-192.png',
        vibrate: [200, 100, 200],
        data: {
            dateOfArrival: Date.now(),
            primaryKey: 1
        },
        actions: [
            {
                action: 'view',
                title: 'View',
                icon: '/icon-check.png'
            },
            {
                action: 'close',
                title: 'Close',
                icon: '/icon-close.png'
            }
        ]
    };

    event.waitUntil(
        self.registration.showNotification('Restaurant Suite', options)
    );
});

// Handle notification clicks
self.addEventListener('notificationclick', (event) => {
    console.log('[Service Worker] Notification clicked:', event.action);

    event.notification.close();

    if (event.action === 'view') {
        event.waitUntil(
            clients.openWindow('/')
        );
    }
});

// Message handling from client
self.addEventListener('message', (event) => {
    console.log('[Service Worker] Message received:', event.data);

    if (event.data && event.data.type === 'SKIP_WAITING') {
        self.skipWaiting();
    }

    if (event.data && event.data.type === 'CACHE_URLS') {
        event.waitUntil(
            caches.open(CACHE_NAME)
                .then((cache) => cache.addAll(event.data.urls))
        );
    }
});
