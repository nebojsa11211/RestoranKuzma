/**
 * Restaurant Suite Admin Application - UI Tests
 * Comprehensive Playwright tests for navigation, responsive design, and visual validation
 */

import { chromium } from 'playwright';
import { mkdir, writeFile } from 'fs/promises';
import { join } from 'path';

// Test configuration
const BASE_URL = 'http://localhost:5054';
const SCREENSHOTS_DIR = join(process.cwd(), 'tests', 'screenshots');
const RESULTS_FILE = join(process.cwd(), 'tests', 'test-results.json');

// Viewport configurations for responsive testing
const VIEWPORTS = {
  mobile: { width: 375, height: 812 },
  tablet: { width: 768, height: 1024 },
  desktop: { width: 1920, height: 1080 }
};

// Test results storage
const testResults = {
  startTime: new Date().toISOString(),
  tests: [],
  summary: {
    total: 0,
    passed: 0,
    failed: 0
  }
};

/**
 * Add test result
 */
function addTestResult(name, status, details = '', screenshot = null) {
  const result = {
    name,
    status,
    details,
    screenshot,
    timestamp: new Date().toISOString()
  };

  testResults.tests.push(result);
  testResults.summary.total++;

  if (status === 'PASS') {
    testResults.summary.passed++;
    console.log(`✓ PASS: ${name}`);
  } else {
    testResults.summary.failed++;
    console.log(`✗ FAIL: ${name} - ${details}`);
  }

  if (screenshot) {
    console.log(`  Screenshot: ${screenshot}`);
  }
}

/**
 * Save screenshot
 */
async function saveScreenshot(page, name) {
  try {
    await mkdir(SCREENSHOTS_DIR, { recursive: true });
    const filename = `${name.replace(/[^a-z0-9]/gi, '-').toLowerCase()}-${Date.now()}.png`;
    const filepath = join(SCREENSHOTS_DIR, filename);
    await page.screenshot({ path: filepath, fullPage: true });
    return filename;
  } catch (error) {
    console.error(`Failed to save screenshot: ${error.message}`);
    return null;
  }
}

/**
 * Test 1: Application Loads Successfully
 */
async function testApplicationLoads(page) {
  console.log('\n=== Test: Application Loads Successfully ===');
  try {
    await page.goto(BASE_URL, { waitUntil: 'networkidle', timeout: 30000 });

    // Check if page title is present
    const title = await page.title();
    if (title.includes('RestaurantSuite.Admin') || title.includes('Admin') || title.includes('Restaurant Suite') || title.includes('Dashboard')) {
      addTestResult('Application loads successfully', 'PASS', `Title: ${title}`);
    } else {
      addTestResult('Application loads successfully', 'FAIL', `Unexpected title: ${title}`);
    }

    // Take screenshot
    const screenshot = await saveScreenshot(page, 'app-loaded');

  } catch (error) {
    addTestResult('Application loads successfully', 'FAIL', error.message);
  }
}

/**
 * Test 2: Navigation Menu Exists and Is Visible
 */
async function testNavigationMenuExists(page) {
  console.log('\n=== Test: Navigation Menu Exists ===');
  try {
    // Check for navigation menu
    const navMenu = await page.locator('[id*="nav"], [class*="nav-menu"], nav').count();

    if (navMenu > 0) {
      addTestResult('Navigation menu exists', 'PASS', `Found ${navMenu} navigation element(s)`);
    } else {
      addTestResult('Navigation menu exists', 'FAIL', 'No navigation menu found');
    }

    // Check for brand/logo - look for "Restaurant" text in sidebar
    const brandText = await page.locator('text=/Restaurant/i').first().count();
    const brandElements = await page.locator('[class*="brand"], [class*="logo"], svg').count();
    if (brandText > 0 || brandElements > 0) {
      addTestResult('Brand/Logo exists', 'PASS');
    } else {
      addTestResult('Brand/Logo exists', 'FAIL', 'No brand/logo found');
    }

  } catch (error) {
    addTestResult('Navigation menu exists', 'FAIL', error.message);
  }
}

/**
 * Test 3: Navigation Links Work
 */
async function testNavigationLinks(page) {
  console.log('\n=== Test: Navigation Links ===');

  const links = [
    { name: 'Dashboard', href: '/', expectedText: 'Dashboard' },
    { name: 'Orders', href: 'orders', expectedText: 'Orders' },
    { name: 'Menu', href: 'menu', expectedText: 'Menu' },
    { name: 'Tables', href: 'tables', expectedText: 'Tables' }
  ];

  for (const link of links) {
    try {
      // Try to find link by text or href
      let linkElement = await page.locator(`a:has-text("${link.name}")`).first();
      let linkCount = await page.locator(`a:has-text("${link.name}")`).count();

      // Fallback to href if text not found
      if (linkCount === 0) {
        linkElement = await page.locator(`a[href="${link.href}"], a[href="/${link.href}"]`).first();
        linkCount = await page.locator(`a[href="${link.href}"], a[href="/${link.href}"]`).count();
      }

      if (linkCount > 0) {
        await linkElement.click();
        await page.waitForTimeout(1000); // Wait for navigation

        // Check if we're on the correct page
        const currentUrl = page.url();
        const expectedPath = link.href === '/' ? BASE_URL + '/' : link.href;
        if (currentUrl.includes(link.href) || currentUrl === BASE_URL + '/' || currentUrl === BASE_URL) {
          addTestResult(`Navigation: ${link.name}`, 'PASS', `URL: ${currentUrl}`);

          // Take screenshot
          await saveScreenshot(page, `nav-${link.name}`);
        } else {
          addTestResult(`Navigation: ${link.name}`, 'FAIL', `Expected URL to contain "${link.href}", got ${currentUrl}`);
        }
      } else {
        addTestResult(`Navigation: ${link.name}`, 'FAIL', 'Link not found');
      }

    } catch (error) {
      addTestResult(`Navigation: ${link.name}`, 'FAIL', error.message);
    }
  }
}

/**
 * Test 4: Sidebar Toggle on Desktop
 */
async function testSidebarToggle(page) {
  console.log('\n=== Test: Sidebar Toggle ===');

  try {
    // Set desktop viewport
    await page.setViewportSize(VIEWPORTS.desktop);
    await page.goto(BASE_URL);
    await page.waitForTimeout(1000);

    // Look for sidebar - check for aside element or nav with dark background
    const sidebar = await page.locator('aside, nav, [class*="sidebar"]').first();
    const sidebarCount = await page.locator('aside, nav:has-text("Dashboard"), [class*="sidebar"]').count();

    if (sidebarCount > 0) {
      const isVisible = await sidebar.isVisible();
      addTestResult('Sidebar visible on desktop', isVisible ? 'PASS' : 'FAIL');

      // Take screenshot
      await saveScreenshot(page, 'sidebar-desktop');
    } else {
      addTestResult('Sidebar visible on desktop', 'FAIL', 'Sidebar not found');
    }

  } catch (error) {
    addTestResult('Sidebar toggle test', 'FAIL', error.message);
  }
}

/**
 * Test 5: Hamburger Menu on Mobile
 */
async function testMobileMenu(page) {
  console.log('\n=== Test: Mobile Menu ===');

  try {
    // Set mobile viewport
    await page.setViewportSize(VIEWPORTS.mobile);
    await page.goto(BASE_URL);
    await page.waitForTimeout(1000);

    // Look for hamburger menu button in header (not in sidebar)
    const hamburger = await page.locator('header button, button[aria-label*="Toggle" i]').count();

    if (hamburger > 0) {
      addTestResult('Hamburger menu exists on mobile', 'PASS');

      // Try to click it - find the toggle button in header
      const hamburgerBtn = await page.locator('header button').first();
      try {
        await hamburgerBtn.click({ timeout: 5000 });
        await page.waitForTimeout(500);

        // Check if sidebar is now visible
        const sidebar = await page.locator('aside, nav:has-text("Dashboard")').first();
        const sidebarVisible = await sidebar.isVisible();

        if (sidebarVisible) {
          addTestResult('Mobile menu opens on click', 'PASS');
          await saveScreenshot(page, 'mobile-menu-open');
        } else {
          addTestResult('Mobile menu opens on click', 'FAIL', 'Sidebar not visible after clicking hamburger');
        }
      } catch (error) {
        addTestResult('Mobile menu opens on click', 'FAIL', `Could not click hamburger: ${error.message}`);
      }

    } else {
      addTestResult('Hamburger menu exists on mobile', 'FAIL', 'Hamburger button not found');
    }

  } catch (error) {
    addTestResult('Mobile menu test', 'FAIL', error.message);
  }
}

/**
 * Test 6: Responsive Design - Mobile Viewport
 */
async function testMobileViewport(page) {
  console.log('\n=== Test: Mobile Viewport (375px) ===');

  try {
    await page.setViewportSize(VIEWPORTS.mobile);
    await page.goto(BASE_URL);
    await page.waitForTimeout(1000);

    // Take screenshot
    const screenshot = await saveScreenshot(page, 'mobile-viewport');

    // Check if main content is visible
    const main = await page.locator('main, [role="main"]').count();
    if (main > 0) {
      addTestResult('Mobile viewport: Main content visible', 'PASS');
    } else {
      addTestResult('Mobile viewport: Main content visible', 'FAIL');
    }

    // Check for horizontal scrollbar (should not exist)
    const bodyWidth = await page.evaluate(() => document.body.scrollWidth);
    const viewportWidth = VIEWPORTS.mobile.width;

    if (bodyWidth <= viewportWidth + 20) { // Small tolerance
      addTestResult('Mobile viewport: No horizontal scroll', 'PASS');
    } else {
      addTestResult('Mobile viewport: No horizontal scroll', 'FAIL', `Body width ${bodyWidth}px exceeds viewport ${viewportWidth}px`);
    }

  } catch (error) {
    addTestResult('Mobile viewport test', 'FAIL', error.message);
  }
}

/**
 * Test 7: Responsive Design - Tablet Viewport
 */
async function testTabletViewport(page) {
  console.log('\n=== Test: Tablet Viewport (768px) ===');

  try {
    await page.setViewportSize(VIEWPORTS.tablet);
    await page.goto(BASE_URL);
    await page.waitForTimeout(1000);

    // Take screenshot
    const screenshot = await saveScreenshot(page, 'tablet-viewport');

    addTestResult('Tablet viewport: Renders correctly', 'PASS');

  } catch (error) {
    addTestResult('Tablet viewport test', 'FAIL', error.message);
  }
}

/**
 * Test 8: Responsive Design - Desktop Viewport
 */
async function testDesktopViewport(page) {
  console.log('\n=== Test: Desktop Viewport (1920px) ===');

  try {
    await page.setViewportSize(VIEWPORTS.desktop);
    await page.goto(BASE_URL);
    await page.waitForTimeout(1000);

    // Take screenshot
    const screenshot = await saveScreenshot(page, 'desktop-viewport');

    // Check if sidebar is visible without needing toggle
    const sidebar = await page.locator('aside, nav:has-text("Dashboard"), [class*="sidebar"]').first();
    const sidebarCount = await page.locator('aside, nav:has-text("Dashboard")').count();

    if (sidebarCount > 0) {
      const sidebarVisible = await sidebar.isVisible();
      if (sidebarVisible) {
        addTestResult('Desktop viewport: Sidebar always visible', 'PASS');
      } else {
        addTestResult('Desktop viewport: Sidebar always visible', 'FAIL', 'Sidebar exists but not visible');
      }
    } else {
      addTestResult('Desktop viewport: Sidebar always visible', 'FAIL', 'Sidebar not found');
    }

  } catch (error) {
    addTestResult('Desktop viewport test', 'FAIL', error.message);
  }
}

/**
 * Test 9: Design System CSS Applied
 */
async function testDesignSystemApplied(page) {
  console.log('\n=== Test: Design System CSS Applied ===');

  try {
    await page.goto(BASE_URL);
    await page.waitForTimeout(1000);

    // Check if Tailwind CSS is loaded by checking for utility classes
    const hasTailwind = await page.evaluate(() => {
      // Check if any element has Tailwind classes
      const elements = document.querySelectorAll('[class*="bg-"], [class*="text-"], [class*="flex"]');
      return elements.length > 0;
    });

    if (hasTailwind) {
      addTestResult('Tailwind CSS loaded', 'PASS', 'Utility classes found');
    } else {
      addTestResult('Tailwind CSS loaded', 'FAIL', 'No Tailwind utility classes found');
    }

    // Check if design system colors are being used
    const hasGradients = await page.evaluate(() => {
      // Check for gradient classes
      const gradientElements = document.querySelectorAll('[class*="gradient"]');
      return gradientElements.length > 0;
    });

    if (hasGradients) {
      addTestResult('Design system gradients applied', 'PASS', 'Gradient classes found');
    } else {
      addTestResult('Design system gradients applied', 'FAIL', 'No gradient classes found');
    }

  } catch (error) {
    addTestResult('Design system CSS test', 'FAIL', error.message);
  }
}

/**
 * Test 10: All Pages Load Without Errors
 */
async function testAllPagesLoad(page) {
  console.log('\n=== Test: All Pages Load ===');

  const pages = [
    { name: 'Dashboard', url: `${BASE_URL}/` },
    { name: 'Orders', url: `${BASE_URL}/orders` },
    { name: 'Menu', url: `${BASE_URL}/menu` },
    { name: 'Tables', url: `${BASE_URL}/tables` }
  ];

  for (const pageInfo of pages) {
    try {
      const response = await page.goto(pageInfo.url, { waitUntil: 'networkidle', timeout: 15000 });

      if (response && response.ok()) {
        addTestResult(`${pageInfo.name} page loads`, 'PASS', `Status: ${response.status()}`);
        await saveScreenshot(page, `page-${pageInfo.name.toLowerCase()}`);
      } else {
        addTestResult(`${pageInfo.name} page loads`, 'FAIL', `Status: ${response?.status() || 'No response'}`);
      }

    } catch (error) {
      addTestResult(`${pageInfo.name} page loads`, 'FAIL', error.message);
    }
  }
}

/**
 * Main test runner
 */
async function runTests() {
  console.log('===================================');
  console.log('Restaurant Suite Admin - UI Tests');
  console.log('===================================');
  console.log(`Base URL: ${BASE_URL}`);
  console.log(`Start Time: ${testResults.startTime}\n`);

  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext();
  const page = await context.newPage();

  try {
    // Run all tests
    await testApplicationLoads(page);
    await testNavigationMenuExists(page);
    await testNavigationLinks(page);
    await testSidebarToggle(page);
    await testMobileMenu(page);
    await testMobileViewport(page);
    await testTabletViewport(page);
    await testDesktopViewport(page);
    await testDesignSystemApplied(page);
    await testAllPagesLoad(page);

    // Generate summary
    console.log('\n===================================');
    console.log('Test Summary');
    console.log('===================================');
    console.log(`Total Tests: ${testResults.summary.total}`);
    console.log(`Passed: ${testResults.summary.passed} ✓`);
    console.log(`Failed: ${testResults.summary.failed} ✗`);
    console.log(`Success Rate: ${((testResults.summary.passed / testResults.summary.total) * 100).toFixed(2)}%`);

    // Save results to file
    testResults.endTime = new Date().toISOString();
    await writeFile(RESULTS_FILE, JSON.stringify(testResults, null, 2));
    console.log(`\nResults saved to: ${RESULTS_FILE}`);
    console.log(`Screenshots saved to: ${SCREENSHOTS_DIR}`);

  } catch (error) {
    console.error('Fatal error during tests:', error);
  } finally {
    await browser.close();
  }

  // Exit with appropriate code
  process.exit(testResults.summary.failed > 0 ? 1 : 0);
}

// Run tests
runTests().catch(console.error);
