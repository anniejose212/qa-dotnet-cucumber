using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Reqnroll.BoDi;
using SeleniumExtras.WaitHelpers;
using System.Numerics;

namespace qa_dotnet_cucumber.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        public IWebDriver Driver => _driver;

        // Locators
        private readonly By SignInLink = By.XPath("//a[normalize-space()='Sign In']");
        private readonly By UsernameField = By.XPath("//input[@type='email' or @placeholder='Email address' or @name='Email' or @id='email']");
        private readonly By PasswordField = By.CssSelector("input[type='password']");
        private readonly By LoginButton = By.XPath("//button[normalize-space()='Login']");
        private readonly By SuccessMessage = By.XPath("//button[normalize-space()='Sign Out' ] | //a[normalize-space()='Sign Out' ]");


        private readonly By PasswordErrorPrompt = By.XPath("//div[text()='Password must be at least 6 characters']");

        private readonly By EmailErrorPrompt = By.XPath("//div[text()='Please enter a valid email address']");

        private readonly By ErrorToast = By.XPath("//div[@class='ns-box-inner' and text()='Confirm your email']");
    


        public LoginPage(IWebDriver driver) 
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10)); 
        }
        public void OpenSignIn()
        {
            var signIn = _wait.Until(
        SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(
            By.XPath("//a[normalize-space()='Sign In']")));
            signIn.Click();
        }
        public void Login(string username, string password)
        {
            var usernameElement = _wait.Until(ExpectedConditions.ElementIsVisible(UsernameField));
            usernameElement.SendKeys(username);

            var passwordElement = _wait.Until(d => d.FindElement(PasswordField));
            passwordElement.SendKeys(password);

            var loginButtonElement = _wait.Until(ExpectedConditions.ElementToBeClickable(LoginButton));
            loginButtonElement.Click();
        }

        public string GetSuccessMessage()
        {
            return _wait.Until(d => d.FindElement(SuccessMessage)).Text;
        }
        public string GetEmailInlineError()
        {
            var el = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(EmailErrorPrompt));
            return el.Text.Trim();
        }

        public string GetPasswordInlineError()
        {
            var el = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(PasswordErrorPrompt));
            return el.Text.Trim();
        }

        public string GetPopupError()
        {
            var el = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(ErrorToast));
            return el.Text.Trim();
        }

        public bool IsAtLoginPage()
        {
            try
            {
                _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(UsernameField));
                return true; 
            }
            catch { return false; }
        }
    }
}