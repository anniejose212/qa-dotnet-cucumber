using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace qa_dotnet_cucumber.Pages
{
   
    public class ProfileOverviewPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        //Common / Header 
        private readonly By NavProfileHeader = By.XPath("//a[@class='item' and normalize-space()='Profile']");
        private readonly By SuccessToastMessage = By.XPath("//div[contains(@class,'ns-box') and contains(@class,'ns-type-success')]");
        private readonly By ErrorToastMessage = By.XPath("//div[contains(@class,'ns-box') and contains(@class,'ns-type-error')]");
        private readonly By DescriptionAnchor = By.XPath("//*[normalize-space()='Description']");

        //  Basic Info
        private readonly By NameDropdownToggle = By.CssSelector("div.ui.dropdown i.dropdown.icon");
        private readonly By NameFirstInput = By.CssSelector("input[name='firstName']");
        private readonly By NameLastInput = By.CssSelector("input[name='lastName']");
        private readonly By SavePrimaryBtn = By.CssSelector("button.ui.teal.button");
        private readonly By DisplayName = By.CssSelector("div.title.active");

       
        public ProfileOverviewPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

        }
        public void OpenProfile()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(NavProfileHeader)).Click();
            
            _wait.Until(ExpectedConditions.ElementIsVisible(DescriptionAnchor));
        }
        public void WaitForLoaded() => _wait.Until(ExpectedConditions.ElementIsVisible(NavProfileHeader));

        // ---------- Displaying Profile Information ----------
        public string GetDisplayName() =>
            _wait.Until(ExpectedConditions.ElementIsVisible(DisplayName)).Text;

        // ---------- Editing Profile (basic info) ----------
        public void EditName(string firstName, string lastName)
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(NameDropdownToggle)).Click();

            var first = _wait.Until(ExpectedConditions.ElementIsVisible(NameFirstInput));
            first.Clear();
            first.SendKeys(firstName);

            var last = _driver.FindElement(NameLastInput);
            last.Clear();
            last.SendKeys(lastName);

            _wait.Until(ExpectedConditions.ElementToBeClickable(SavePrimaryBtn)).Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(SuccessToastMessage));
        }    
      
}
    }

