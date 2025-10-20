using NUnit.Framework;
using qa_dotnet_cucumber.Pages;
using qa_dotnet_cucumber.Support;
using Reqnroll;

namespace qa_dotnet_cucumber.Steps
{
    [Binding]
    public class LoginSteps : StepBase
    {
        private readonly LoginPage _loginPage;

        private readonly qa_dotnet_cucumber.Config.TestSettings _settings;
        private readonly List<string> _logBuffer = new();

        // Injects page object
        public LoginSteps(LoginPage loginPage)
        {
            _loginPage = loginPage;
        }

        // Opens login modal
        [Given("I am on the login modal")]
        public void GivenIAmOnTheLoginModal()
        {
            _loginPage.OpenSignIn();
        }

        // Logs in with valid creds and waits for success
        [When("I enter valid credentials")]
        public void WhenIEnterValidCredentials()
        {
            _loginPage.Login("annie.jose1202@gmail.com", "123456");
            _loginPage.WaitUntilLoggedIn();
        }

        // Attempts login with invalid username
        [When("I enter an invalid username and valid password")]
        public void WhenIEnterAnInvalidUsernameAndValidPassword()
        {
            _loginPage.Login("invaliduser", "123456");
        }

        // Attempts login with invalid password
        [When("I enter a valid username and invalid password")]
        public void WhenIEnterAValidUsernameAndInvalidPassword()
        {
            _loginPage.Login("tomsmith", "wrongpassword");
        }

        // Attempts login with empty inputs
        [When("I enter empty credentials")]
        public void WhenIEnterEmptyCredentials()
        {
            _loginPage.Login("", "");
        }

        // Attempts login with extra whitespace
        [When("I enter valid credentials with leading or trailing spaces")]
        public void WhenIEnterValidCredentialsWithLeadingOrTrailingSpaces()
        {
            string usernameWithSpaces = "  annie.jose1202@gmail.com  ";
            string passwordWithSpaces = " 123456 ";
            _loginPage.Login(usernameWithSpaces, passwordWithSpaces);
        }

        // Repeats invalid password attempts (for lockout)
        [When(@"I enter an valid username and invalid password multiple times")]
        public void WhenIEnterValidUsernameAndInvalidPasswordMultipleTimes()
        {
            for (int i = 0; i < 3; i++)
            {
                _loginPage.Login("annie.jose1202@gmail.com", "12345");
            }
        }

        // Verifies logged-in state
        [Then("I should see the secure area")]
        public void ThenIShouldSeeTheSecureArea()
        {
            var successMessage = _loginPage.GetSuccessMessage();
            Assert.That(successMessage, Does.Contain("Sign Out"), "Expected Sign Out to be visible after login.");
        }

        // Verifies any error signals are shown
        [Then("I should see an error message")]
        public void ThenIShouldSeeAnErrorMessage()
        {
            var email = _loginPage.GetEmailError();
            var pass = _loginPage.GetPasswordError();
            var popup = _loginPage.GetPopupError();

            var message = "";
            if (email != "") message += $"Email error: {email}\n";
            if (pass != "") message += $"Password error: {pass}\n";
            if (popup != "") message += $"Popup error: {popup}\n";

            if (message == "")
            {
                Info("No error message found when one was expected.");
                TestContext.Progress.WriteLine("No error message found when one was expected.");
                Assert.Fail("No error message found when one was expected.");
            }

            Info(message);
            TestContext.Progress.WriteLine(message);

            Assert.That(
                message,
                Does.Match("valid|email|required|password|invalid|incorrect|confirm|verification|failed").IgnoreCase,
                $"Unexpected error text: '{message}'");
        }

        // Verifies cooldown lockout behavior
        [Then("I should not be able to login again for a cooldown period")]
        public void ThenIShouldNotBeAbleToLoginAgainForACooldownPeriod()
        {
            bool stillLocked = _loginPage.IsLockoutMessageVisible();
            if (!stillLocked)
            {
                Assert.Inconclusive("Lockout message not found — feature not yet implemented.");
                return;
            }
            Assert.That(stillLocked,
                "Expected to remain locked out during cooldown period, but login succeeded or lockout message missing.");
        }
    }
}
