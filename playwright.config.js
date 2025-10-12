import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: '.',
  testMatch: ['test-chef-login.mjs', 'test-chef-navigation.mjs', 'test-simple-navigation.mjs', 'test-diagnostic.mjs', 'test-chef-nav-simple.mjs'],
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: 'html',
  use: {
    baseURL: 'http://localhost:5173',
    trace: 'on-first-retry',
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],
  webServer: {
    command: 'dotnet run --project src/RestaurantSuite.Chef/RestaurantSuite.Chef.csproj',
    port: 5173,
    reuseExistingServer: !process.env.CI,
  },
});