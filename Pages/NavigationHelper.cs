using OpenQA.Selenium;

namespace qa_dotnet_cucumber.Pages
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
            _driver.Navigate().GoToUrl(Hooks.Hooks.Settings.Environment.BaseUrl + urlPath);
        }
    }
}