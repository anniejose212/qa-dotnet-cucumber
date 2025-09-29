using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Reqnroll;
using Reqnroll.BoDi;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using System.IO;
using System.Text.Json;
using qa_dotnet_cucumber.Config;
using qa_dotnet_cucumber.Pages;

namespace qa_dotnet_cucumber.Hooks
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _objectContainer;
        private static TestSettings _settings;

        public static TestSettings Settings => _settings;

        public Hooks(IObjectContainer objectContainer)
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

            // Get project root by navigating up from bin/Debug/net8.0
            string projectRoot = Path.GetFullPath(Path.Combine(currentDir, "..", ".."));
            Console.WriteLine($"BeforeTestRun started at {DateTime.Now}");
        }

        [BeforeScenario]
        public void BeforeScenario(ScenarioContext scenarioContext)
        {
            Console.WriteLine($"Starting {scenarioContext.ScenarioInfo.Title} on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
            new DriverManager().SetUpDriver(new ChromeConfig());
            var chromeOptions = new ChromeOptions();
            if (_settings.Browser.Headless)
            {
                chromeOptions.AddArgument("--headless");
            }
            var driver = new ChromeDriver(chromeOptions);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(_settings.Browser.TimeoutSeconds);
            driver.Manage().Window.Maximize();

            _objectContainer.RegisterInstanceAs<IWebDriver>(driver);
            _objectContainer.RegisterInstanceAs(new NavigationHelper(driver));
            _objectContainer.RegisterInstanceAs(new LoginPage(driver));
            _objectContainer.RegisterInstanceAs(new ProfileOverviewPage(driver));
            _objectContainer.RegisterInstanceAs(new ProfileLanguagesCrudPage(driver));
            _objectContainer.RegisterInstanceAs(new ProfileSkillsCrudPage(driver));

            Console.WriteLine($"Created test: {scenarioContext.ScenarioInfo.Title} on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
        }

        [AfterScenario]
        public void AfterScenario()
        {
            var driver = _objectContainer.Resolve<IWebDriver>();
            driver?.Quit();
            Console.WriteLine($"Finished scenario on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
        }
    }
}
