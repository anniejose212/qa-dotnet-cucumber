// FILE: NavigationHelper.cs
// ROLE: Thin navigation wrapper for composing absolute URLs via test settings.

using OpenQA.Selenium;

namespace qa_dotnet_cucumber.Support
{
    public class NavigationHelper
    {
        private readonly IWebDriver _driver;

        public NavigationHelper(IWebDriver driver)
        {
            _driver = driver;
        }

        public void NavigateTo(string urlPath)
        {
            _driver.Navigate().GoToUrl(Hooks.WebDriverDIHook.Settings.Environment.BaseUrl + urlPath);
        }
    }
}
