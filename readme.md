# QA-DotNet-Cucumber Framework

A .NET-based test automation framework using Reqnroll (Cucumber for .NET), Selenium WebDriver, and NUnit. This framework
is designed to test web applications with a clean, maintainable structure.

## Overview

This framework provides automated functional testing for web applications with the following features:

- **Reqnroll**: Implements Cucumber's Gherkin syntax for readable tests
- **Selenium WebDriver**: Handles browser interactions
- **NUnit**: Manages test execution and assertions
- **ExtentReports**: Generates HTML test reports
- **Page Object Model (POM)**: Separates test logic from page interactions

## Prerequisites

- **.NET SDK**: Version 8.0 or higher (install from [dotnet.microsoft.com](https://dotnet.microsoft.com))
- **IDE**: Visual Studio Code or Visual Studio (recommended)
- **Chrome Browser**: Required for Selenium WebDriver (ChromeDriver version must match your browser version via
  WebDriverManager)

## Project Structure

```
├── Features/           # Gherkin feature files
├── Steps/              # C# step implementations
├── Pages/              # Page Object Model classes
├── Hooks/              # Setup and teardown logic
├── Config/             # Configuration classes
├── Tests/              # NUnit test runner
└── settings.json       # Configuration file
```

## Getting Started

### Installation

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd qa-dotnet-cucumber
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

### Configuration

1. Configure `settings.json` in the project root:
   ```json
   {
     "Browser": {
       "Type": "Chrome",
       "Headless": false,
       "TimeoutSeconds": 30
     },
     "Report": {
       "Path": "TestReport.html",
       "Title": "Test Automation Report"
     },
     "Environment": {
       "BaseUrl": "http://the-internet.herokuapp.com" 
     }
   }
   ```
   Note: Update `BaseUrl` to match your test environment.

### Running Tests

1. Execute tests:
   ```bash
   dotnet test
   ```

2. View results:
    - Open `TestReport.html` in your browser to see the test report

## Writing Tests

### Feature Files (Gherkin)

Create feature files in the `Features/` directory:

```gherkin
Feature: Login Functionality
As a user, I want to log in to access restricted content.

Scenario: Perform a successful login
  Given I am on the login page
  When I enter valid credentials
  Then I should see the secure area
```

### Step Definitions

Implement steps in `StepDefinitions/`:

```csharp
[Binding]
public class LoginSteps
{
    private readonly LoginPage _loginPage;
    private readonly NavigationHelper _navigationHelper;

    public LoginSteps(LoginPage loginPage, NavigationHelper navigationHelper)
    {
        _loginPage = loginPage;
        _navigationHelper = navigationHelper;
    }

    [Given("I am on the login page")]
    public void GivenIAmOnTheLoginPage()
    {
        _navigationHelper.NavigateTo("/login");
    }

    [When("I enter valid credentials")]
    public void WhenIEnterValidCredentials()
    {
        _loginPage.Login("tomsmith", "SuperSecretPassword!");
    }

    [Then("I should see the secure area")]
    public void ThenIShouldSeeTheSecureArea()
    {
        var successMessage = _loginPage.GetSuccessMessage();
        Assert.That(successMessage, Does.Contain("You logged into a secure area!"));
    }
}
```

### Page Object Model

Create page classes in `Pages/`:

```csharp
public class LoginPage
{
    private readonly IWebDriver _driver;
    private readonly By UsernameField = By.Id("username");
    private readonly By PasswordField = By.Id("password");
    private readonly By LoginButton = By.CssSelector("button[type='submit']");
    private readonly By SuccessMessage = By.CssSelector(".flash.success");

    public LoginPage(IWebDriver driver)
    {
        _driver = driver;
    }

    public void Login(string username, string password)
    {
        _driver.FindElement(UsernameField).SendKeys(username);
        _driver.FindElement(PasswordField).SendKeys(password);
        _driver.FindElement(LoginButton).Click();
    }

    public string GetSuccessMessage()
    {
        return _driver.FindElement(SuccessMessage).Text;
    }
}
```

## Configuration Options

- **Browser Settings**:
    - `Type`: Currently supports "Chrome"
    - `Headless`: Set to `true` for headless execution
    - `TimeoutSeconds`: Default wait timeout

- **Report Settings**:
    - `Path`: Output report filename
    - `Title`: Report title

- **Environment Settings**:
    - `BaseUrl`: Target application URL

## Best Practices

1. **Adding New Tests**:
    - Write a new `.feature` file
    - Implement steps in a new or existing step definition class
    - Follow the Page Object Model pattern

2. **Debugging**:
    - Use IDE breakpoints in step definitions or page objects
    - Check the HTML report for test execution details

3. **Extending the Framework**:
    - Add new page classes for different parts of the application
    - Keep page objects focused and maintainable
    - Follow the Single Responsibility Principle

## Support

For additional help or questions, please reach out to the team or create an issue in the repository.