import { chromium } from 'playwright';

(async () => {
  const browser = await chromium.launch();
  const page = await browser.newPage();

  try {
    console.log('Navigating to http://localhost:5201...');
    await page.goto('http://localhost:5201', { waitUntil: 'networkidle', timeout: 30000 });

    console.log('Taking screenshot...');
    await page.screenshot({ path: 'admin-ui-screenshot.png', fullPage: true });

    console.log('Getting HTML...');
    const html = await page.content();
    console.log('Page title:', await page.title());
    console.log('First 500 chars of HTML:', html.substring(0, 500));

    // Check if Tailwind CSS is loaded
    const tailwindLoaded = await page.evaluate(() => {
      const links = Array.from(document.querySelectorAll('link[rel="stylesheet"]'));
      return links.map(link => link.href);
    });
    console.log('CSS files loaded:', tailwindLoaded);

    // Check computed styles on body
    const bodyBg = await page.evaluate(() => {
      return window.getComputedStyle(document.body).backgroundColor;
    });
    console.log('Body background color:', bodyBg);

    console.log('Screenshot saved as admin-ui-screenshot.png');
  } catch (error) {
    console.error('Error:', error.message);
  }

  await browser.close();
})();
