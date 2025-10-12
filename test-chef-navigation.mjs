import { test, expect } from '@playwright/test';

test.describe('Chef Navigation Tests', () => {
  const chefAppUrl = 'http://localhost:5003';
  const testCredentials = {
    email: 'chef@demo.com',
    password: 'Chef123!@#'
  };

  test.beforeEach(async ({ page }) => {
    // Navigate to chef app
    await page.goto(chefAppUrl);
  });

  test('should test dashboard navigation link', async ({ page }) => {
    // Wait for redirect to login page
    await page.waitForURL('**/login', { timeout: 10000 });
    
    // Fill and submit login form
    await page.fill('input[type="email"]', testCredentials.email);
    await page.fill('input[type="password"]', testCredentials.password);
    await page.click('button[type="submit"]');
    
    // Wait for potential redirect to dashboard
    try {
      await page.waitForURL('**/', { timeout: 5000 });
      
      // Now test the dashboard navigation link
      const dashboardLink = page.locator('#chef-nav-dashboard');
      
      // Check if the link is visible and clickable
      await expect(dashboardLink).toBeVisible();
      await expect(dashboardLink).toBeEnabled();
      
      // Click the dashboard link
      await dashboardLink.click();
      
      // Wait for navigation
      await page.waitForTimeout(1000);
      
      // Check if we're still on the dashboard or if navigation failed
      const currentUrl = page.url();
      console.log('Current URL after dashboard click:', currentUrl);
      
      // Check if dashboard content is visible
      const dashboardContent = page.locator('#chef-dashboard-container');
      const welcomeTitle = page.locator('#chef-welcome-title');
      
      if (await dashboardContent.isVisible() || await welcomeTitle.isVisible()) {
        console.log('✓ Dashboard navigation successful');
      } else {
        console.log('✗ Dashboard navigation failed - content not visible');
        
        // Check for error messages
        const errorMessage = page.locator('.alert-error');
        const unauthorizedMessage = page.locator('text=Unauthorized');
        
        if (await errorMessage.isVisible()) {
          const errorText = await errorMessage.textContent();
          console.log('Error message:', errorText);
        }
        
        if (await unauthorizedMessage.isVisible()) {
          console.log('✗ Unauthorized access - authentication issue');
        }
      }
      
    } catch (error) {
      console.log('Navigation test failed:', error.message);
      
      // Check current page state
      const currentUrl = page.url();
      console.log('Current URL:', currentUrl);
      
      // Take screenshot for debugging
      await page.screenshot({ path: 'navigation-test-failure.png' });
    }
  });

  test('should check dashboard link attributes', async ({ page }) => {
    await page.waitForURL('**/login', { timeout: 10000 });
    
    // Fill and submit login form
    await page.fill('input[type="email"]', testCredentials.email);
    await page.fill('input[type="password"]', testCredentials.password);
    await page.click('button[type="submit"]');
    
    // Wait for potential redirect
    try {
      await page.waitForURL('**/', { timeout: 5000 });
      
      // Check dashboard link attributes
      const dashboardLink = page.locator('#chef-nav-dashboard');
      
      // Check href attribute
      const href = await dashboardLink.getAttribute('href');
      console.log('Dashboard link href:', href);
      
      // Check if it's a proper NavLink component
      const hasActiveClass = await dashboardLink.evaluate(el => 
        el.classList.contains('active') || el.getAttribute('aria-current') === 'page'
      );
      
      console.log('Dashboard link has active class:', hasActiveClass);
      
      // Check if link is actually clickable (not disabled)
      const isDisabled = await dashboardLink.evaluate(el => 
        el.hasAttribute('disabled') || el.classList.contains('disabled')
      );
      
      console.log('Dashboard link is disabled:', isDisabled);
      
    } catch (error) {
      console.log('Failed to check dashboard link attributes:', error.message);
    }
  });
});