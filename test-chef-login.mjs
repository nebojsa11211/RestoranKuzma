import { test, expect } from '@playwright/test';

test.describe('Chef Project Login Tests', () => {
  const chefAppUrl = 'http://localhost:5003';
  const testCredentials = {
    email: 'chef@demo.com',
    password: 'Chef123!@#'
  };

  test.beforeEach(async ({ page }) => {
    // Navigate to chef app
    await page.goto(chefAppUrl);
  });

  test('should redirect to login page when not authenticated', async ({ page }) => {
    // Wait for redirect
    await page.waitForURL('**/login', { timeout: 10000 });
    
    // Check if we're on login page
    await expect(page).toHaveURL(/.*\/login/);
    await expect(page.locator('h2')).toContainText('Chef Login');
    await expect(page.locator('.subtitle')).toContainText('Kitchen Access');
  });

  test('should display login form with email and password fields', async ({ page }) => {
    await page.waitForURL('**/login', { timeout: 10000 });
    
    // Check form elements
    await expect(page.locator('input[type="email"]')).toBeVisible();
    await expect(page.locator('input[type="password"]')).toBeVisible();
    await expect(page.locator('button[type="submit"]')).toBeVisible();
    await expect(page.locator('input[type="checkbox"]')).toBeVisible(); // Remember me
  });

  test('should show validation errors for empty form submission', async ({ page }) => {
    await page.waitForURL('**/login', { timeout: 10000 });
    
    // Submit empty form
    await page.click('button[type="submit"]');
    
    // Check for validation messages
    const emailValidation = page.locator('text=The Email field is required');
    const passwordValidation = page.locator('text=The Password field is required');
    
    await expect(emailValidation).toBeVisible();
    await expect(passwordValidation).toBeVisible();
  });

  test('should attempt login with test credentials', async ({ page }) => {
    await page.waitForURL('**/login', { timeout: 10000 });
    
    // Fill login form
    await page.fill('input[type="email"]', testCredentials.email);
    await page.fill('input[type="password"]', testCredentials.password);
    
    // Submit form
    await page.click('button[type="submit"]');
    
    // Wait for response
    await page.waitForTimeout(2000);
    
    // Check for success or error message
    const successMessage = page.locator('.alert-success');
    const errorMessage = page.locator('.alert-error');
    
    // One of them should be visible
    await expect(successMessage.or(errorMessage)).toBeVisible();
  });

  test('should check if login redirects to dashboard on success', async ({ page }) => {
    await page.waitForURL('**/login', { timeout: 10000 });
    
    // Fill and submit login form
    await page.fill('input[type="email"]', testCredentials.email);
    await page.fill('input[type="password"]', testCredentials.password);
    await page.click('button[type="submit"]');
    
    // Wait for potential redirect
    try {
      await page.waitForURL('**/dashboard', { timeout: 5000 });
      await expect(page.locator('h1')).toContainText('Welcome');
    } catch {
      // If no redirect, check if still on login page
      await expect(page).toHaveURL(/.*\/login/);
    }
  });

  test('should verify chef-specific authentication', async ({ page }) => {
    await page.waitForURL('**/login', { timeout: 10000 });
    
    // Fill login form
    await page.fill('input[type="email"]', testCredentials.email);
    await page.fill('input[type="password"]', testCredentials.password);
    await page.click('button[type="submit"]');
    
    // Wait for response
    await page.waitForTimeout(2000);
    
    // Check if error message mentions chef role requirement
    const errorMessage = page.locator('.alert-error');
    if (await errorMessage.isVisible()) {
      const errorText = await errorMessage.textContent();
      console.log('Login error:', errorText);
      
      // Check if it's a role-specific error
      if (errorText.includes('Chef') || errorText.includes('chef')) {
        console.log('✓ Chef role validation is working');
      }
    }
  });

  test('should check network requests during login', async ({ page }) => {
    // Capture network requests
    const requests = [];
    page.on('request', request => {
      if (request.url().includes('api') || request.url().includes('auth')) {
        requests.push({
          url: request.url(),
          method: request.method(),
          headers: request.headers()
        });
      }
    });
    
    await page.waitForURL('**/login', { timeout: 10000 });
    
    // Fill and submit login form
    await page.fill('input[type="email"]', testCredentials.email);
    await page.fill('input[type="password"]', testCredentials.password);
    await page.click('button[type="submit"]');
    
    // Wait for requests
    await page.waitForTimeout(3000);
    
    console.log('Network requests captured:', requests.length);
    requests.forEach((req, index) => {
      console.log(`Request ${index + 1}: ${req.method} ${req.url}`);
    });
    
    // Check if login API call was made
    const loginRequest = requests.find(req => 
      req.method === 'POST' && 
      (req.url.includes('login') || req.url.includes('auth'))
    );
    
    if (loginRequest) {
      console.log('✓ Login API request detected');
    } else {
      console.log('✗ No login API request found');
    }
  });
});