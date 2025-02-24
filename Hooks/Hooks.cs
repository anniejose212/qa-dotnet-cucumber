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
        private static TestSettings _settings;
        private ExtentTest? _test;
        private static readonly object _reportLock = new object();

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
            string reportFileName = _settings.Report.Path.TrimStart('/'); // e.g., "TestReport.html"
            string reportPath = Path.Combine(projectRoot, reportFileName);

            _htmlReporter = new ExtentSparkReporter(reportPath);
            _extent = new ExtentReports();
            _extent.AttachReporter(_htmlReporter);
            _extent.AddSystemInfo("Environment", _settings.Environment.BaseUrl);
            _extent.AddSystemInfo("Browser", _settings.Browser.Type);
            Console.WriteLine($"BeforeTestRun started at {DateTime.Now}, Report Path: {reportPath}");
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

            lock (_reportLock)
            {
                _test = _extent!.CreateTest(scenarioContext.ScenarioInfo.Title);
            }
            Console.WriteLine($"Created test: {scenarioContext.ScenarioInfo.Title} on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
        }

        [AfterStep]
        public void AfterStep(ScenarioContext scenarioContext)
        {
            var stepType = scenarioContext.StepContext.StepInfo.StepDefinitionType.ToString();
            var stepText = scenarioContext.StepContext.StepInfo.Text;
            lock (_reportLock)
            {
                if (scenarioContext.TestError == null)
                {
                    _test!.Log(Status.Pass, $"{stepType} {stepText}");
                    Console.WriteLine($"Logged pass: {stepType} {stepText} on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
                }
                else
                {
                    var driver = _objectContainer.Resolve<IWebDriver>();
                    var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                    var screenshotPath = Path.Combine(Directory.GetCurrentDirectory(), $"Screenshot_{DateTime.Now.Ticks}_{Thread.CurrentThread.ManagedThreadId}.png");
                    screenshot.SaveAsFile(screenshotPath);
                    _test!.Log(Status.Fail, $"{stepType} {stepText}", MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build());
                    Console.WriteLine($"Logged fail with screenshot: {screenshotPath} on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
                }
            }
        }

        [AfterScenario]
        public void AfterScenario()
        {
            var driver = _objectContainer.Resolve<IWebDriver>();
            driver?.Quit();
            Console.WriteLine($"Finished scenario on Thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now}");
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            lock (_reportLock)
            {
                Console.WriteLine("AfterTestRun executed - Flushing report to: " + _settings.Report.Path + " at " + DateTime.Now);
                _extent!.Flush();
            }
        }
    }
}