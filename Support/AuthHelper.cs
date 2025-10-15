// AuthHelper.cs
using qa_dotnet_cucumber.Pages;

namespace qa_dotnet_cucumber.Support
{
    public class AuthHelper
    {
        private readonly LoginPage _login;

        public AuthHelper(LoginPage login) => _login = login;

        public void LoginAsDefaultUser()
        {
            if (_login.IsLoggedIn()) return;

            _login.OpenSignIn();
            _login.Login("annie.jose1202@gmail.com", "123456");
            _login.WaitUntilLoggedIn(5);
        }
    }
}
