import { test, expect } from '@playwright/test';

test.describe('Chef Project Login Tests', () => {
  const chefAppUrl = 'http://localhost:5173';
  const testCredentials = {
    email: 'chef@demo.com',
    password: 'Chef123!@#'
  };

  test('should access chef login page directly', async ({ page }) => {
    console.log('Navigating to chef app...');
    await page.goto(chefAppUrl);
    
    // Wait a moment for any redirects
    await page.waitForTimeout(2000);
    
    console.log('Current URL:', page.url());
    
    // Check if we're on login page or redirected to login
    if (page.url().includes('/login')) {
      console.log('✓ Successfully redirected to login page');
      
      // Verify login page elements
      const loginTitle = await page.locator('h2').textContent();
      console.log('Login title:', loginTitle);
      
      const subtitle = await page.locator('.subtitle').textContent();
      console.log('Subtitle:', subtitle);
      
      // Check for form fields
      const emailField = await page.locator('input[type="email"]').isVisible();
      const passwordField = await page.locator('input[type="password"]').isVisible();
      const submitButton = await page.locator('button[type="submit"]').isVisible();
      
      console.log('Email field visible:', emailField);
      console.log('Password field visible:', passwordField);
      console.log('Submit button visible:', submitButton);
      
      if (loginTitle.includes('Chef Login') && subtitle.includes('Kitchen Access')) {
        console.log('✓ Chef login page is properly configured');
      } else {
        console.log('✗ Login page content is incorrect');
      }
      
    } else {
      console.log('✗ Not redirected to login page');
      console.log('Current page content:');
      console.log(await page.textContent('body'));
    }
  });

  test('should attempt login with test credentials', async ({ page }) => {
    console.log('Testing login functionality...');
    await page.goto(chefAppUrl);
    await page.waitForTimeout(2000);
    
    if (page.url().includes('/login')) {
      console.log('Filling login form...');
      
      // Fill the form
      await page.fill('input[type="email"]', testCredentials.email);
      await page.fill('input[type="password"]', testCredentials.password);
      
      console.log('Clicking submit...');
      await page.click('button[type="submit"]');
      
      // Wait for response
      await page.waitForTimeout(3000);
      
      console.log('After login attempt - Current URL:', page.url());
      
      // Check for messages
      const successMessage = await page.locator('.alert-success').textContent().catch(() => '');
      const errorMessage = await page.locator('.alert-error').textContent().catch(() => '');
      
      console.log('Success message:', successMessage);
      console.log('Error message:', errorMessage);
      
      if (page.url() !== chefAppUrl + '/login' && !page.url().includes('/login')) {
        console.log('✓ Login appears successful - redirected away from login page');
      } else if (errorMessage) {
        console.log('✗ Login failed with error:', errorMessage);
      } else {
        console.log('? Login result unclear');
      }
    } else {
      console.log('✗ Could not access login page');
    }
  });

  test('should check for chef-specific authentication', async ({ page }) => {
    console.log('Checking chef-specific authentication...');
    await page.goto(chefAppUrl);
    await page.waitForTimeout(2000);
    
    if (page.url().includes('/login')) {
      // Try to login
      await page.fill('input[type="email"]', testCredentials.email);
      await page.fill('input[type="password"]', testCredentials.password);
      await page.click('button[type="submit"]');
      
      await page.waitForTimeout(3000);
      
      // Check if we got a role-specific error
      const errorMessage = await page.locator('.alert-error').textContent().catch(() => '');
      
      if (errorMessage.includes('Chef') || errorMessage.includes('chef')) {
        console.log('✓ Chef role validation is working');
        console.log('Error message:', errorMessage);
      } else if (errorMessage) {
        console.log('General login error:', errorMessage);
      } else {
        console.log('No error message found');
      }
    }
  });

  test('should verify page structure', async ({ page }) => {
    console.log('Verifying page structure...');
    await page.goto(chefAppUrl);
    await page.waitForTimeout(2000);
    
    // Check for key elements
    const hasLoginContainer = await page.locator('.login-container').isVisible();
    const hasLoginCard = await page.locator('.login-card').isVisible();
    const hasChefIcon = await page.locator('.login-icon').isVisible();
    
    console.log('Login container:', hasLoginContainer);
    console.log('Login card:', hasLoginCard);
    console.log('Chef icon:', hasChefIcon);
    
    if (hasLoginContainer && hasLoginCard && hasChefIcon) {
      console.log('✓ Login page structure is correct');
    } else {
      console.log('✗ Missing login page elements');
    }
  });
});