using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using qa_dotnet_cucumber.Config;
using qa_dotnet_cucumber.Pages;
using Reqnroll;

namespace qa_dotnet_cucumber.Steps
{
    [Binding]
    public class LoginSteps
    {
        private readonly LoginPage _loginPage;
        private readonly NavigationHelper _navigationHelper;
        private readonly TestSettings _settings;

        public LoginSteps(LoginPage loginPage, NavigationHelper navigationHelper)
        {
            _loginPage = loginPage;
            _navigationHelper = navigationHelper;
        }

        [Given("I am on the login page")]
        public void GivenIAmOnTheLoginPage()
        {
            _navigationHelper.NavigateTo("/");
            _loginPage.OpenSignIn();
            Assert.That(_loginPage.IsAtLoginPage(), Is.True, "Should be on the login page");
        }

        [When("I enter valid credentials")]
        public void WhenIEnterValidCredentials()
        {
            _loginPage.Login("annie.jose1202@gmail.com", "123456");
        }

        [When("I enter an invalid username and valid password")]
        public void WhenIEnterAnInvalidUsernameAndValidPassword()
        {
            _loginPage.Login("invaliduser", "SuperSecretPassword!");
        }

        [When("I enter a valid username and invalid password")]
        public void WhenIEnterAValidUsernameAndInvalidPassword()
        {
            _loginPage.Login("tomsmith", "wrongpassword");
        }

        [When("I enter empty credentials")]
        public void WhenIEnterEmptyCredentials()
        {
            _loginPage.Login("", "");
        }

        [Then("I should see the secure area")]
        public void ThenIShouldSeeTheSecureArea()
        {
            var successMessage = _loginPage.GetSuccessMessage();
            Assert.That(successMessage, Does.Contain("Sign Out"), "Expected Sign Out to be visible after login.");

        }

        [Then("I should see an error message")]
        public void ThenIShouldSeeAnErrorMessage()
        {
            var d = _loginPage.Driver;
            var wait = new WebDriverWait(d, TimeSpan.FromSeconds(10));

            
            var emailPrompt = d.FindElements(By.XPath(
                "//input[@type='email' or @placeholder='Email address' or @name='Email' or @id='email']" +
                "/following-sibling::*[contains(@class,'prompt') and contains(@class,'red')]"
            )).FirstOrDefault();

            var passPrompt = d.FindElements(By.XPath(
                "//input[@type='password']" +
                "/following-sibling::*[contains(@class,'prompt') and contains(@class,'red')]"
            )).FirstOrDefault();

            if (emailPrompt != null || passPrompt != null)
            {
                if (emailPrompt != null)
                    Assert.That(emailPrompt.Text.Trim(),
                        Does.Match("valid|email|required").IgnoreCase,
                        $"Unexpected email error: '{emailPrompt.Text}'");

                if (passPrompt != null)
                    Assert.That(passPrompt.Text.Trim(),
                        Does.Match("at least 6|required|password").IgnoreCase,
                        $"Unexpected password error: '{passPrompt.Text}'");
                return; 
            }

            var toast = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
                By.CssSelector(".ns-box.ns-show, .ui.error.message, .negative.message, .alert-danger, .validation-summary-errors")
            ));
            var msg = toast.Text.Trim();

            Assert.That(msg, Is.Not.Empty, "Expected an error popup.");
            Assert.That(msg, Does.Match("invalid|incorrect|unauthor|confirm|verification|failed|required").IgnoreCase,
                $"Unexpected popup text: '{msg}'");
        }

    }
}
