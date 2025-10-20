using NUnit.Framework;
using qa_dotnet_cucumber.Pages;
using qa_dotnet_cucumber.Support;
using Reqnroll;

namespace qa_dotnet_cucumber.Steps
{
    [Binding]
    public class AuthSteps
    {
        private readonly AuthHelper _auth;
        private readonly LoginPage _login;

        public AuthSteps(AuthHelper auth, LoginPage login)
        {
            _auth = auth;
            _login = login;
        }

        [Given(@"I am logged in as the default user")]
        public void GivenIAmLoggedInAsDefaultUser()
        {
            _auth.LoginAsDefaultUser();              // wraps the POM and default creds
            Assert.That(_login.IsLoggedIn(), "Login failed for default user.");
        }
    }
}
