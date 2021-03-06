﻿using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using NUnit.Framework;

namespace Coypu.AcceptanceTests
{
	//bug https://github.com/mozilla/geckodriver/issues/469 => https://bugzilla.mozilla.org/show_bug.cgi?id=1332122
	[TestFixture, Ignore("Navigating to file:.// hangs Marionette. Re-enable when that is fixed")]
    public class Screenshots : WaitAndRetryExamples
    {
        [Test]
        public void SavesToSpecifiedLocation()
        {
            browser.Visit(TestPageLocation("test-card.jpg"));
            browser.ResizeTo(800, 600);

            SavesToSpecifiedLocation(browser);
        }

        [Test]
        public void CapturesCorrectWindow()
        {
            browser.ClickLink("Open pop up window");
            var popUp = browser.FindWindow("Pop Up Window");
            popUp.Visit(TestPageLocation("test-card.jpg"));
            popUp.ResizeTo(800, 600);

            // Do something in the main window
            browser.FindCss("body").Click();

            SavesToSpecifiedLocation(popUp);
        }

        private static void SavesToSpecifiedLocation(BrowserWindow browserWindow)
        {
            
            const string fileName = "screenshot-test-card.jpg";
            try
            {
                browserWindow.SaveScreenshot(fileName, ImageFormat.Jpeg);

                Assert.That(File.Exists(fileName), "Expected screenshot saved to " + new FileInfo(fileName).FullName);
                using (var saved = Image.FromFile("screenshot-test-card.jpg"))
                {
                    var docWidth = float.Parse(browserWindow.ExecuteScript("return window.document.body.clientWidth;").ToString());
                    var docHeight = float.Parse(browserWindow.ExecuteScript("return window.document.body.clientHeight;").ToString());
                    Assert.That(saved.PhysicalDimension, Is.EqualTo(new SizeF(docWidth, docHeight)));
                }
            }
            finally
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
        }
    }
}