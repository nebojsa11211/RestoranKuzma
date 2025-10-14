import { test, expect } from '@playwright/test';

test.describe('Diagnostic Navigation Test', () => {
  test('should diagnose dashboard navigation issue', async ({ page }) => {
    // Listen for console messages
    page.on('console', msg => {
      console.log('Browser console:', msg.text());
    });

    // Listen for any errors
    page.on('pageerror', error => {
      console.log('Page error:', error.message);
    });

    // Navigate to chef app
    console.log('Navigating to chef app...');
    await page.goto('http://localhost:5003');
    
    // Check current URL
    console.log('Current URL after initial navigation:', page.url());
    
    // Wait for redirect to login page
    try {
      await page.waitForURL('**/login', { timeout: 10000 });
      console.log('Successfully redirected to login page');
    } catch (error) {
      console.log('Failed to redirect to login page:', error.message);
      console.log('Current URL:', page.url());
      await page.screenshot({ path: 'diagnostic-login-redirect-failed.png' });
      return;
    }
    
    // Check if login form exists
    const emailInput = page.locator('input[type="email"]');
    const passwordInput = page.locator('input[type="password"]');
    const submitButton = page.locator('button[type="submit"]');
    
    console.log('Email input visible:', await emailInput.isVisible());
    console.log('Password input visible:', await passwordInput.isVisible());
    console.log('Submit button visible:', await submitButton.isVisible());
    
    // Fill and submit login form
    console.log('Filling login form...');
    await page.fill('input[type="email"]', 'chef@demo.com');
    await page.fill('input[type="password"]', 'Chef123!@#');
    
    console.log('Clicking submit button...');
    await submitButton.click();
    
    // Wait and see what happens
    await page.waitForTimeout(3000);
    
    console.log('Current URL after login attempt:', page.url());
    
    // Check if we're on dashboard or still on login
    const dashboardContainer = page.locator('#chef-dashboard-container');
    const welcomeTitle = page.locator('#chef-welcome-title');
    const dashboardLink = page.locator('#chef-nav-dashboard');
    
    console.log('Dashboard container visible:', await dashboardContainer.isVisible().catch(() => false));
    console.log('Welcome title visible:', await welcomeTitle.isVisible().catch(() => false));
    console.log('Dashboard link visible:', await dashboardLink.isVisible().catch(() => false));
    
    // Take a screenshot of current state
    await page.screenshot({ path: 'diagnostic-after-login.png' });
    
    // If dashboard link is visible, try to click it
    if (await dashboardLink.isVisible().catch(() => false)) {
      console.log('Attempting to click dashboard link...');
      
      // Log the link's properties
      const href = await dashboardLink.getAttribute('href');
      const className = await dashboardLink.getAttribute('class');
      console.log('Dashboard link href:', href);
      console.log('Dashboard link class:', className);
      
      try {
        await dashboardLink.click();
        console.log('Dashboard link clicked successfully');
        
        // Wait for navigation
        await page.waitForTimeout(2000);
        console.log('URL after dashboard click:', page.url());
        
        // Take final screenshot
        await page.screenshot({ path: 'diagnostic-after-dashboard-click.png' });
        
      } catch (clickError) {
        console.log('Failed to click dashboard link:', clickError.message);
      }
    } else {
      console.log('Dashboard link not found or not visible');
    }
    
    // Final state
    console.log('Test completed. Final URL:', page.url());
  });
});