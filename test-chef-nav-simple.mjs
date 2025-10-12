import { test, expect } from '@playwright/test';

test.describe('Chef Navigation Simple Test', () => {
  const chefAppUrl = 'http://localhost:5003';
  const testCredentials = {
    email: 'chef@demo.com',
    password: 'Chef123!@#'
  };

  test('should navigate and test dashboard link', async ({ page }) => {
    // Navigate to chef app
    await page.goto(chefAppUrl);

    // Wait for Blazor to load
    await page.waitForSelector('#app', { timeout: 15000 });

    // Wait a bit for client-side redirect
    await page.waitForTimeout(2000);

    console.log('Current URL after initial load:', page.url());

    // Check if we're on login page or if login form is visible
    const loginForm = page.locator('input[type="email"]');
    if (await loginForm.isVisible({ timeout: 5000 })) {
      console.log('✓ Login page loaded');

      // Fill and submit login form
      await page.fill('input[type="email"]', testCredentials.email);
      await page.fill('input[type="password"]', testCredentials.password);
      await page.click('button[type="submit"]');

      // Wait for navigation
      await page.waitForTimeout(3000);

      console.log('Current URL after login:', page.url());
    }

    // Now test the dashboard navigation link
    const dashboardLink = page.locator('#chef-nav-dashboard');

    // Check if the link exists
    const linkExists = await dashboardLink.count() > 0;
    console.log('Dashboard link exists:', linkExists);

    if (linkExists && await dashboardLink.isVisible({ timeout: 2000 })) {
      console.log('✓ Dashboard link is visible');

      // Check href attribute
      const href = await dashboardLink.getAttribute('href');
      console.log('Dashboard link href:', href);

      // Click the dashboard link
      await dashboardLink.click();

      // Wait for navigation
      await page.waitForTimeout(1000);

      // Check if we're on dashboard
      const currentUrl = page.url();
      console.log('Current URL after dashboard click:', currentUrl);

      // Check if dashboard content is visible
      const dashboardContent = page.locator('#chef-dashboard-container');
      if (await dashboardContent.isVisible({ timeout: 2000 })) {
        console.log('✓ Dashboard content is visible - NAVIGATION WORKS!');
      } else {
        console.log('✗ Dashboard content not visible');

        // Take screenshot for debugging
        await page.screenshot({ path: 'chef-nav-debug.png' });
      }
    } else {
      console.log('✗ Dashboard link not found or not visible');

      // Take screenshot for debugging
      await page.screenshot({ path: 'chef-nav-no-link.png' });
    }
  });
});
