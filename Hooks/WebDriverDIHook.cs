// FILE: WebDriverDIHook.cs
// ROLE: WebDriver lifecycle + DI registration for pages/helpers. Reads settings.json.

using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using qa_dotnet_cucumber.Config;
using qa_dotnet_cucumber.Pages;
using qa_dotnet_cucumber.Support;
using Reqnroll;
using Reqnroll.BoDi;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace qa_dotnet_cucumber.Hooks
{
    [Binding]
    public class WebDriverDIHook
    {
        private readonly IObjectContainer _objectContainer;
        private static TestSettings _settings;

        public static TestSettings Settings => _settings;

        public WebDriverDIHook(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            string currentDir = Directory.GetCurrentDirectory();
            string settingsPath = Path.Combine(currentDir, "settings.json");
            string json = File.ReadAllText(settingsPath);
            _settings = JsonSerializer.Deserialize<TestSettings>(json);

            string projectRoot = Path.GetFullPath(Path.Combine(currentDir, "..", ".."));
            Console.WriteLine($"BeforeTestRun started at {DateTime.Now}");
        }

        [BeforeScenario(Order = 0)]
        public void BeforeScenario(ScenarioContext scenarioContext)
        {
            Console.WriteLine($"Starting {scenarioContext.ScenarioInfo.Title} on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");

            new DriverManager().SetUpDriver(new ChromeConfig());
            var chromeOptions = new ChromeOptions
            {
                UnhandledPromptBehavior = UnhandledPromptBehavior.DismissAndNotify
            };

            if (_settings.Browser.Headless)
            {
                chromeOptions.AddArgument("--headless=new");
            }

            var driver = new ChromeDriver(chromeOptions);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(_settings.Browser.TimeoutSeconds);
            driver.Manage().Window.Maximize();

            // --- register core singletons for this scenario ---
            _objectContainer.RegisterInstanceAs<IWebDriver>(driver);

            var nav = new NavigationHelper(driver);
            var login = new LoginPage(driver, nav);

            _objectContainer.RegisterInstanceAs(nav);
            _objectContainer.RegisterInstanceAs(login);

            // Page objects
            _objectContainer.RegisterInstanceAs(new ProfileOverviewPage(driver));
            _objectContainer.RegisterInstanceAs(new ProfileLanguagesCrudPage(driver));
            _objectContainer.RegisterInstanceAs(new ProfileSkillsCrudPage(driver));

            _objectContainer.RegisterInstanceAs(new AuthHelper(login));

            Console.WriteLine($"Created test: {scenarioContext.ScenarioInfo.Title} on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
        }

        [AfterScenario(Order = 1000)]
        public void AfterScenario()
        {
            var driver = _objectContainer.Resolve<IWebDriver>();
            driver.TryDismissAnyAlert();

            try
            {
                driver?.Quit();
                Console.WriteLine($"ChromeDriver quit successfully on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
            }
            catch (Exception)
            {
                Console.WriteLine($"[ERROR] Failed to quit ChromeDriver on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
            }

            Console.WriteLine($"Finished scenario on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
        }
    }
}
