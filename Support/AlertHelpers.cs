// AlertHelpers.cs
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace qa_dotnet_cucumber.Support

{
    public static class AlertHelpers
    {
        /// <summary>
        /// Wait up to `seconds` for a JS alert. Returns the alert or null.
        /// </summary>
        public static IAlert TryGetAlert(this IWebDriver driver, int seconds = 2)
        {
            if (driver == null) return null;

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));
            try
            {
                return wait.Until(d =>
                {
                    try { return d.SwitchTo().Alert(); }
                    catch (NoAlertPresentException) { return null; }
                });
            }
            catch (WebDriverTimeoutException)
            {
                return null;
            }
        }

        /// <summary>
        /// Dismisses any alert if present (optionally waits a bit). Returns true if something was dismissed.
        /// </summary>
        public static bool TryDismissAnyAlert(this IWebDriver driver, int seconds = 0)
        {
            if (driver == null) return false;

            // Reuse the ONE alert finder above (no duplication).
            var alert = seconds > 0 ? driver.TryGetAlert(seconds) : driver.TryGetAlert(0);
            if (alert == null) return false;

            try
            {
                var text = alert.Text; 
                Console.WriteLine($"[TEARDOWN] Dismissing stray alert: '{text}'");
                alert.Accept(); 
                return true;
            }
            catch
            {
                return false; 
            }
        }
    }
}
