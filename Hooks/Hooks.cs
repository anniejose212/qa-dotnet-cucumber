using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Reqnroll;
using Reqnroll.BoDi;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace qa_dotnet_cucumber.Hooks
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _objectContainer;
        private static ExtentReports? _extent;
        private static ExtentSparkReporter? _htmlReporter;
        private ExtentTest? _test;

        public Hooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            _htmlReporter = new ExtentSparkReporter("TestReport.html");
            _extent = new ExtentReports();
            _extent.AttachReporter(_htmlReporter);
        }

        [BeforeScenario]
        public void BeforeScenario(ScenarioContext scenarioContext)
        {
            new DriverManager().SetUpDriver(new ChromeConfig());
            var driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            _objectContainer.RegisterInstanceAs<IWebDriver>(driver);
            _test = _extent!.CreateTest(scenarioContext.ScenarioInfo.Title);
        }

        [AfterStep]
        public void AfterStep(ScenarioContext scenarioContext)
        {
            var stepType = scenarioContext.StepContext.StepInfo.StepDefinitionType.ToString();
            var stepText = scenarioContext.StepContext.StepInfo.Text;
            if (scenarioContext.TestError == null)
            {
                _test!.Log(Status.Pass, $"{stepType} {stepText}");
            }
            else
            {
                var driver = _objectContainer.Resolve<IWebDriver>();
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                var screenshotPath = Path.Combine(Directory.GetCurrentDirectory(), $"Screenshot_{DateTime.Now.Ticks}.png");
                screenshot.SaveAsFile(screenshotPath);
                _test!.Log(Status.Fail, $"{stepType} {stepText}", MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build());
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
            _extent!.Flush();
        }
    }
}