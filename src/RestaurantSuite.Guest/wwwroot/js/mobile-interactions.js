// Mobile Interactions JavaScript Library
// Provides pull-to-refresh, swipe gestures, and haptic feedback

window.mobileInteractions = {
    // ===================================
    // HAPTIC FEEDBACK
    // ===================================

    /**
     * Triggers haptic feedback on supported devices
     * @param {string} type - 'light', 'medium', 'heavy', 'success', 'warning', 'error'
     */
    haptic: function(type = 'light') {
        // Check if Vibration API is supported
        if (!navigator.vibrate) {
            console.log('[Haptic] Vibration API not supported');
            return;
        }

        const patterns = {
            light: [10],
            medium: [20],
            heavy: [30],
            success: [10, 50, 10],
            warning: [15, 100, 15],
            error: [30, 100, 30, 100, 30],
            selection: [5],
            impact: [15]
        };

        const pattern = patterns[type] || patterns.light;
        navigator.vibrate(pattern);
        console.log(`[Haptic] Triggered ${type} feedback`);
    },

    /**
     * Add haptic feedback to an element
     * @param {string} selector - CSS selector for the element
     * @param {string} type - Haptic feedback type
     */
    addHapticToElement: function(selector, type = 'light') {
        const element = document.querySelector(selector);
        if (!element) {
            console.warn(`[Haptic] Element not found: ${selector}`);
            return;
        }

        element.addEventListener('click', () => {
            this.haptic(type);
        });

        console.log(`[Haptic] Added ${type} feedback to ${selector}`);
    },

    /**
     * Add haptic feedback to all buttons
     */
    initializeButtonHaptics: function() {
        const buttons = document.querySelectorAll('button, .btn, [role="button"]');
        buttons.forEach(button => {
            if (!button.hasAttribute('data-haptic-initialized')) {
                button.addEventListener('click', () => {
                    this.haptic('light');
                });
                button.setAttribute('data-haptic-initialized', 'true');
            }
        });
        console.log(`[Haptic] Initialized haptics for ${buttons.length} buttons`);
    },

    // ===================================
    // PULL-TO-REFRESH
    // ===================================

    pullToRefreshState: {
        startY: 0,
        currentY: 0,
        isDragging: false,
        threshold: 80,
        maxPull: 120,
        element: null,
        indicator: null,
        callback: null
    },

    /**
     * Initialize pull-to-refresh on an element
     * @param {string} elementId - ID of the scrollable element
     * @param {Function} callback - Function to call when refresh is triggered (should return a Promise)
     */
    initializePullToRefresh: function(elementId, dotnetHelper) {
        const element = document.getElementById(elementId);
        if (!element) {
            console.error(`[PTR] Element not found: ${elementId}`);
            return false;
        }

        this.pullToRefreshState.element = element;
        this.pullToRefreshState.dotnetHelper = dotnetHelper;

        // Create pull-to-refresh indicator
        this.createPullToRefreshIndicator(element);

        // Add touch event listeners
        element.addEventListener('touchstart', this.handlePullStart.bind(this), { passive: true });
        element.addEventListener('touchmove', this.handlePullMove.bind(this), { passive: false });
        element.addEventListener('touchend', this.handlePullEnd.bind(this), { passive: true });

        console.log(`[PTR] Initialized on element: ${elementId}`);
        return true;
    },

    createPullToRefreshIndicator: function(parentElement) {
        const indicator = document.createElement('div');
        indicator.id = 'pull-to-refresh-indicator';
        indicator.className = 'pull-to-refresh-indicator';
        indicator.innerHTML = `
            <div class="ptr-spinner">
                <svg class="ptr-icon" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                          d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"/>
                </svg>
            </div>
            <div class="ptr-text">Pull to refresh</div>
        `;

        // Insert at the beginning of the parent element
        parentElement.insertBefore(indicator, parentElement.firstChild);
        this.pullToRefreshState.indicator = indicator;
    },

    handlePullStart: function(e) {
        const element = this.pullToRefreshState.element;

        // Only trigger if scrolled to top
        if (element.scrollTop <= 0) {
            this.pullToRefreshState.startY = e.touches[0].clientY;
            this.pullToRefreshState.isDragging = true;
        }
    },

    handlePullMove: function(e) {
        if (!this.pullToRefreshState.isDragging) return;

        const element = this.pullToRefreshState.element;
        const indicator = this.pullToRefreshState.indicator;

        // Only allow pulling down when at top of scroll
        if (element.scrollTop > 0) {
            this.pullToRefreshState.isDragging = false;
            return;
        }

        this.pullToRefreshState.currentY = e.touches[0].clientY;
        const pullDistance = this.pullToRefreshState.currentY - this.pullToRefreshState.startY;

        if (pullDistance > 0) {
            // Prevent default scrolling
            e.preventDefault();

            // Calculate pull with resistance
            const resistance = 2.5;
            const adjustedDistance = Math.min(pullDistance / resistance, this.pullToRefreshState.maxPull);

            // Update indicator position and rotation
            indicator.style.transform = `translateY(${adjustedDistance}px)`;
            indicator.style.opacity = Math.min(adjustedDistance / this.pullToRefreshState.threshold, 1);

            const rotation = (adjustedDistance / this.pullToRefreshState.threshold) * 180;
            const icon = indicator.querySelector('.ptr-icon');
            icon.style.transform = `rotate(${rotation}deg)`;

            // Update text based on threshold
            const text = indicator.querySelector('.ptr-text');
            if (adjustedDistance >= this.pullToRefreshState.threshold) {
                text.textContent = 'Release to refresh';
                indicator.classList.add('ptr-ready');
                this.haptic('selection');
            } else {
                text.textContent = 'Pull to refresh';
                indicator.classList.remove('ptr-ready');
            }
        }
    },

    handlePullEnd: async function(e) {
        if (!this.pullToRefreshState.isDragging) return;

        const pullDistance = this.pullToRefreshState.currentY - this.pullToRefreshState.startY;
        const indicator = this.pullToRefreshState.indicator;

        this.pullToRefreshState.isDragging = false;

        if (pullDistance >= this.pullToRefreshState.threshold) {
            // Trigger refresh
            indicator.classList.add('ptr-loading');
            const text = indicator.querySelector('.ptr-text');
            text.textContent = 'Refreshing...';

            this.haptic('success');

            try {
                // Call the .NET callback
                if (this.pullToRefreshState.dotnetHelper) {
                    await this.pullToRefreshState.dotnetHelper.invokeMethodAsync('OnPullToRefresh');
                }

                // Show success briefly
                indicator.classList.remove('ptr-loading');
                indicator.classList.add('ptr-success');
                text.textContent = 'Refreshed!';

                setTimeout(() => {
                    this.resetPullToRefresh();
                }, 500);
            } catch (error) {
                console.error('[PTR] Refresh failed:', error);
                indicator.classList.remove('ptr-loading');
                indicator.classList.add('ptr-error');
                text.textContent = 'Refresh failed';

                setTimeout(() => {
                    this.resetPullToRefresh();
                }, 1000);
            }
        } else {
            // Reset without refresh
            this.resetPullToRefresh();
        }
    },

    resetPullToRefresh: function() {
        const indicator = this.pullToRefreshState.indicator;

        indicator.style.transform = 'translateY(-60px)';
        indicator.style.opacity = '0';
        indicator.classList.remove('ptr-ready', 'ptr-loading', 'ptr-success', 'ptr-error');

        const text = indicator.querySelector('.ptr-text');
        text.textContent = 'Pull to refresh';

        const icon = indicator.querySelector('.ptr-icon');
        icon.style.transform = 'rotate(0deg)';
    },

    // ===================================
    // SWIPE GESTURES
    // ===================================

    swipeState: {
        startX: 0,
        startY: 0,
        startTime: 0,
        endX: 0,
        endY: 0,
        endTime: 0,
        minDistance: 50,
        maxTime: 500,
        threshold: 30,
        element: null,
        dotnetHelper: null
    },

    /**
     * Initialize swipe gesture detection
     * @param {string} elementId - ID of the element to detect swipes on
     * @param {object} dotnetHelper - .NET reference for callbacks
     */
    initializeSwipeGestures: function(elementId, dotnetHelper) {
        const element = elementId ? document.getElementById(elementId) : document.body;

        if (!element) {
            console.error(`[Swipe] Element not found: ${elementId}`);
            return false;
        }

        this.swipeState.element = element;
        this.swipeState.dotnetHelper = dotnetHelper;

        element.addEventListener('touchstart', this.handleSwipeStart.bind(this), { passive: true });
        element.addEventListener('touchend', this.handleSwipeEnd.bind(this), { passive: true });

        console.log(`[Swipe] Initialized on element: ${elementId || 'body'}`);
        return true;
    },

    handleSwipeStart: function(e) {
        this.swipeState.startX = e.touches[0].clientX;
        this.swipeState.startY = e.touches[0].clientY;
        this.swipeState.startTime = new Date().getTime();
    },

    handleSwipeEnd: function(e) {
        this.swipeState.endX = e.changedTouches[0].clientX;
        this.swipeState.endY = e.changedTouches[0].clientY;
        this.swipeState.endTime = new Date().getTime();

        this.detectSwipe();
    },

    detectSwipe: function() {
        const deltaX = this.swipeState.endX - this.swipeState.startX;
        const deltaY = this.swipeState.endY - this.swipeState.startY;
        const deltaTime = this.swipeState.endTime - this.swipeState.startTime;

        // Check if gesture was quick enough
        if (deltaTime > this.swipeState.maxTime) {
            return;
        }

        // Check if horizontal swipe
        if (Math.abs(deltaX) > Math.abs(deltaY) && Math.abs(deltaX) > this.swipeState.minDistance) {
            if (deltaX > 0) {
                this.onSwipe('right');
            } else {
                this.onSwipe('left');
            }
        }
        // Check if vertical swipe
        else if (Math.abs(deltaY) > this.swipeState.minDistance) {
            if (deltaY > 0) {
                this.onSwipe('down');
            } else {
                this.onSwipe('up');
            }
        }
    },

    onSwipe: async function(direction) {
        console.log(`[Swipe] Detected: ${direction}`);
        this.haptic('light');

        if (this.swipeState.dotnetHelper) {
            try {
                await this.swipeState.dotnetHelper.invokeMethodAsync('OnSwipe', direction);
            } catch (error) {
                console.error('[Swipe] Callback error:', error);
            }
        }
    },

    /**
     * Dispose swipe gestures
     */
    disposeSwipeGestures: function() {
        if (this.swipeState.element) {
            this.swipeState.element.removeEventListener('touchstart', this.handleSwipeStart);
            this.swipeState.element.removeEventListener('touchend', this.handleSwipeEnd);
            console.log('[Swipe] Disposed');
        }
    },

    /**
     * Dispose pull-to-refresh
     */
    disposePullToRefresh: function() {
        const element = this.pullToRefreshState.element;
        if (element) {
            element.removeEventListener('touchstart', this.handlePullStart);
            element.removeEventListener('touchmove', this.handlePullMove);
            element.removeEventListener('touchend', this.handlePullEnd);

            if (this.pullToRefreshState.indicator) {
                this.pullToRefreshState.indicator.remove();
            }

            console.log('[PTR] Disposed');
        }
    }
};

// Initialize button haptics when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        window.mobileInteractions.initializeButtonHaptics();
    });
} else {
    window.mobileInteractions.initializeButtonHaptics();
}

// Re-initialize button haptics when new content is loaded (for Blazor)
const observer = new MutationObserver(() => {
    window.mobileInteractions.initializeButtonHaptics();
});

observer.observe(document.body, {
    childList: true,
    subtree: true
});

console.log('[Mobile Interactions] Library loaded successfully');
