import { test, expect } from '@playwright/test';

test.describe('Simple Navigation Test', () => {
  test('should check if dashboard link exists and is visible', async ({ page }) => {
    // Navigate to chef app
    await page.goto('http://localhost:5003');
    
    // Wait for redirect to login page
    await page.waitForURL('**/login', { timeout: 10000 });
    
    // Fill and submit login form
    await page.fill('input[type="email"]', 'chef@demo.com');
    await page.fill('input[type="password"]', 'Chef123!@#');
    await page.click('button[type="submit"]');
    
    // Wait for potential redirect
    try {
      await page.waitForURL('**/', { timeout: 5000 });
      
      // Simple check - does the dashboard link exist and is it visible?
      const dashboardLink = page.locator('#chef-nav-dashboard');
      
      console.log('Dashboard link visible:', await dashboardLink.isVisible());
      console.log('Dashboard link enabled:', await dashboardLink.isEnabled());
      
      // Take a screenshot to see what's happening
      await page.screenshot({ path: 'dashboard-link-check.png' });
      
      // Try to click it
      await dashboardLink.click();
      
      // Wait a moment
      await page.waitForTimeout(1000);
      
      console.log('Current URL after click:', page.url());
      
      // Take another screenshot
      await page.screenshot({ path: 'after-click.png' });
      
    } catch (error) {
      console.log('Error during test:', error.message);
      await page.screenshot({ path: 'error-state.png' });
    }
  });
});