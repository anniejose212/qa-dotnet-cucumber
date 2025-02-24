using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Reqnroll;
using Reqnroll.BoDi;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
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
        private static ExtentReports? _extent;
        private static ExtentSparkReporter? _htmlReporter;
        private static TestSettings _settings; // Restored static field
        private ExtentTest? _test;

        public static TestSettings Settings => _settings; // Public getter

        public Hooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            string settingsPath = Path.Combine(Directory.GetCurrentDirectory(), "settings.json");
            string json = File.ReadAllText(settingsPath);
            _settings = JsonSerializer.Deserialize<TestSettings>(json);

            _htmlReporter = new ExtentSparkReporter(_settings.Report.Path);
            _extent = new ExtentReports();
            _extent.AttachReporter(_htmlReporter);
            _extent.AddSystemInfo("Environment", _settings.Environment.BaseUrl);
            _extent.AddSystemInfo("Browser", _settings.Browser.Type);
        }

        [BeforeScenario]
        public void BeforeScenario(ScenarioContext scenarioContext)
        {
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
            _objectContainer.RegisterInstanceAs(new NavigationHelper(driver)); // Register NavigationHelper
            _objectContainer.RegisterInstanceAs(new LoginPage(driver)); // Register LoginPage

            _test = _extent!.CreateTest(scenarioContext.ScenarioInfo.Title);
            Console.WriteLine($"Created test: {scenarioContext.ScenarioInfo.Title}");
        }

        [AfterStep]
        public void AfterStep(ScenarioContext scenarioContext)
        {
            var stepType = scenarioContext.StepContext.StepInfo.StepDefinitionType.ToString();
            var stepText = scenarioContext.StepContext.StepInfo.Text;
            if (scenarioContext.TestError == null)
            {
                _test!.Log(Status.Pass, $"{stepType} {stepText}");
                Console.WriteLine($"Logged pass: {stepType} {stepText}");
            }
            else
            {
                var driver = _objectContainer.Resolve<IWebDriver>();
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                var screenshotPath = Path.Combine(Directory.GetCurrentDirectory(), $"Screenshot_{DateTime.Now.Ticks}.png");
                screenshot.SaveAsFile(screenshotPath);
                _test!.Log(Status.Fail, $"{stepType} {stepText}", MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build());
                Console.WriteLine($"Logged fail with screenshot: {screenshotPath}");
            }
        }

        [AfterScenario]
        public void AfterScenario()
        {
            var driver = _objectContainer.Resolve<IWebDriver>();
            driver?.Quit();
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            Console.WriteLine("AfterTestRun executed - Flushing report to: " + _settings.Report.Path);
            _extent!.Flush();
        }
    }
}