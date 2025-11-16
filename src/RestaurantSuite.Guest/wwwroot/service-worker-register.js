// Service Worker Registration Script

// Check if service workers are supported
if ('serviceWorker' in navigator) {
    window.addEventListener('load', () => {
        registerServiceWorker();
    });
}

async function registerServiceWorker() {
    try {
        const registration = await navigator.serviceWorker.register('/service-worker.js', {
            scope: '/'
        });

        console.log('[PWA] Service Worker registered successfully:', registration.scope);

        // Check for updates
        registration.addEventListener('updatefound', () => {
            const newWorker = registration.installing;

            newWorker.addEventListener('statechange', () => {
                if (newWorker.state === 'installed' && navigator.serviceWorker.controller) {
                    // New service worker available
                    console.log('[PWA] New version available! Please refresh.');
                    showUpdateNotification();
                }
            });
        });

        // Check for updates periodically
        setInterval(() => {
            registration.update();
        }, 60000); // Check every minute

    } catch (error) {
        console.error('[PWA] Service Worker registration failed:', error);
    }
}

// Show update notification to user
function showUpdateNotification() {
    // Create a simple notification
    const updateBanner = document.createElement('div');
    updateBanner.id = 'update-banner';
    updateBanner.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        background: #9333EA;
        color: white;
        padding: 12px 16px;
        text-align: center;
        z-index: 9999;
        font-size: 14px;
        box-shadow: 0 2px 8px rgba(0,0,0,0.15);
    `;
    updateBanner.innerHTML = `
        <span>A new version is available!</span>
        <button onclick="window.location.reload()" style="
            margin-left: 12px;
            background: white;
            color: #9333EA;
            border: none;
            padding: 6px 12px;
            border-radius: 4px;
            cursor: pointer;
            font-weight: 500;
        ">Refresh</button>
    `;

    document.body.prepend(updateBanner);
}

// Install prompt for PWA
let deferredPrompt;

window.addEventListener('beforeinstallprompt', (e) => {
    console.log('[PWA] Install prompt available');

    // Prevent the mini-infobar from appearing on mobile
    e.preventDefault();

    // Stash the event so it can be triggered later
    deferredPrompt = e;

    // Show custom install button
    showInstallPromotion();
});

function showInstallPromotion() {
    // Check if already installed
    if (window.matchMedia('(display-mode: standalone)').matches) {
        console.log('[PWA] App is already installed');
        return;
    }

    // Create install banner
    const installBanner = document.createElement('div');
    installBanner.id = 'install-banner';
    installBanner.style.cssText = `
        position: fixed;
        bottom: 80px;
        left: 16px;
        right: 16px;
        background: white;
        padding: 16px;
        border-radius: 12px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        z-index: 1001;
        display: flex;
        align-items: center;
        justify-content: space-between;
        animation: slideUp 0.3s ease;
    `;

    installBanner.innerHTML = `
        <div style="flex: 1;">
            <div style="font-weight: 600; margin-bottom: 4px;">Install Restaurant Suite</div>
            <div style="font-size: 14px; color: #64748b;">Add to your home screen for quick access</div>
        </div>
        <button id="install-button" style="
            background: #9333EA;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 8px;
            cursor: pointer;
            font-weight: 500;
            margin-left: 12px;
        ">Install</button>
        <button id="dismiss-install" style="
            background: transparent;
            border: none;
            padding: 8px;
            cursor: pointer;
            margin-left: 8px;
            font-size: 20px;
            color: #64748b;
        ">&times;</button>
    `;

    document.body.appendChild(installBanner);

    // Handle install button click
    document.getElementById('install-button').addEventListener('click', async () => {
        if (!deferredPrompt) return;

        // Show the install prompt
        deferredPrompt.prompt();

        // Wait for the user to respond
        const { outcome } = await deferredPrompt.userChoice;
        console.log('[PWA] User response to install prompt:', outcome);

        // Clear the deferredPrompt
        deferredPrompt = null;

        // Remove the banner
        installBanner.remove();
    });

    // Handle dismiss button
    document.getElementById('dismiss-install').addEventListener('click', () => {
        installBanner.remove();
    });
}

// Track when app is installed
window.addEventListener('appinstalled', () => {
    console.log('[PWA] App successfully installed');

    // Hide install promotion
    const installBanner = document.getElementById('install-banner');
    if (installBanner) {
        installBanner.remove();
    }

    // Track installation (analytics)
    if (window.gtag) {
        gtag('event', 'pwa_install', {
            event_category: 'engagement',
            event_label: 'PWA Installation'
        });
    }
});

// Detect if running as PWA
function isPWA() {
    return window.matchMedia('(display-mode: standalone)').matches
        || window.navigator.standalone
        || document.referrer.includes('android-app://');
}

// Log PWA status
console.log('[PWA] Running as installed app:', isPWA());

// Offline/Online detection
window.addEventListener('online', () => {
    console.log('[PWA] Back online');
    hideOfflineBanner();

    // Trigger background sync if available
    if ('serviceWorker' in navigator && 'sync' in ServiceWorkerRegistration.prototype) {
        navigator.serviceWorker.ready.then((registration) => {
            return registration.sync.register('sync-orders');
        });
    }
});

window.addEventListener('offline', () => {
    console.log('[PWA] Gone offline');
    showOfflineBanner();
});

function showOfflineBanner() {
    let banner = document.getElementById('offline-banner');
    if (banner) return;

    banner = document.createElement('div');
    banner.id = 'offline-banner';
    banner.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        background: #dc2626;
        color: white;
        padding: 8px 16px;
        text-align: center;
        z-index: 9999;
        font-size: 14px;
    `;
    banner.textContent = 'You are currently offline. Some features may be limited.';
    document.body.prepend(banner);
}

function hideOfflineBanner() {
    const banner = document.getElementById('offline-banner');
    if (banner) {
        banner.remove();
    }
}

// Check online status on load
if (!navigator.onLine) {
    showOfflineBanner();
}
