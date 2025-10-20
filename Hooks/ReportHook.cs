// FILE: ReportHook.cs
// ROLE: ExtentReports lifecycle + per-step logging and screenshots. Minimal, thread-safe, DI-aware.

using System;
using System.IO;
using System.Threading;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
using OpenQA.Selenium;
using Reqnroll;
using Reqnroll.BoDi;
using System.Net; // for WebUtility.HtmlEncode

namespace qa_dotnet_cucumber.Hooks
{
    /// <summary>
    /// Minimal, focused reporting hook. Owns Extent lifecycle and step logging.
    /// Keeps existing Hooks.cs for WebDriver + DI.
    /// </summary>
   
    [Binding]
    public class ReportHook
    {
        private readonly IObjectContainer _container;

        private static readonly object _lock = new();
        private static ExtentReports? _extent;
        private static ExtentSparkReporter? _spark;
        private static string _reportDir = string.Empty;  // absolute dir containing the html
        private static string _reportPath = string.Empty; // absolute html path
        private static string HtmlSafe(string s) => WebUtility.HtmlEncode(s ?? string.Empty);
        // Per-context nodes (thread-safe)
        private static readonly AsyncLocal<ExtentTest?> _featureNode = new();
        private static readonly AsyncLocal<ExtentTest?> _scenarioNode = new();

        public ReportHook(IObjectContainer container)
        {
            _container = container;
        }

        // --------------------- BEFORE TEST RUN ---------------------
        [BeforeTestRun(Order = 1000)]
        public static void InitReport()
        {
            // Put reports directly under bin directory
            var baseDir = AppContext.BaseDirectory; // .../bin/Debug/netX.Y/
            _reportDir = baseDir; // report folder in bin

            var settings = Hooks.WebDriverDIHook.Settings;
            var configured = (settings?.Report?.Path ?? "TestReport.html").Trim();
            var fileName = string.IsNullOrWhiteSpace(configured) ? "TestReport.html" : Path.GetFileName(configured);

            _reportPath = Path.Combine(_reportDir, fileName);

            _spark = new ExtentSparkReporter(_reportPath);
            _spark.Config.DocumentTitle = "QA .NET – Test Report";
            _spark.Config.ReportName = "UI E2E";

            _extent = new ExtentReports();
            _extent.AttachReporter(_spark);

            if (settings != null)
            {
                if (!string.IsNullOrWhiteSpace(settings.Environment?.BaseUrl))
                    _extent.AddSystemInfo("Environment", settings.Environment.BaseUrl);
                if (!string.IsNullOrWhiteSpace(settings.Browser?.Type))
                    _extent.AddSystemInfo("Browser", settings.Browser.Type);
            }

            Console.WriteLine($"[REPORT] Writing to bin folder: {_reportPath}");
        }

        // --------------------- FEATURE ---------------------

        [BeforeFeature(Order = -10)]
        public static void BeforeFeature(FeatureContext feature)
        {
            lock (_lock)
            {
                _featureNode.Value = _extent!.CreateTest<Feature>(HtmlSafe(feature.FeatureInfo.Title));
            }
        }

        [BeforeScenario(Order = 200)] // run after WebDriver init in Hooks.cs (default order 0)
        public void BeforeScenario(ScenarioContext scenario)
        {
            lock (_lock)
            {
                var parent = _featureNode.Value ?? _extent!.CreateTest(HtmlSafe(scenario.ScenarioInfo.Title)); 
                _scenarioNode.Value = parent.CreateNode<Scenario>(HtmlSafe(scenario.ScenarioInfo.Title));       

                foreach (var tag in scenario.ScenarioInfo.Tags)
                    _scenarioNode.Value.AssignCategory(tag);
            }

            // Capture actual browser + platform from current driver session if available
            try
            {
                if (_container.IsRegistered<IWebDriver>())
                {
                    var drv = _container.Resolve<IWebDriver>();
                    if (drv is IHasCapabilities hasCaps && hasCaps.Capabilities != null)
                    {
                        var caps = hasCaps.Capabilities;
                        var browserVersion = caps.GetCapability("browserVersion")?.ToString();
                        var platformName = caps.GetCapability("platformName")?.ToString();

                        if (!string.IsNullOrWhiteSpace(browserVersion))
                            _extent!.AddSystemInfo("BrowserVersion", browserVersion);
                        if (!string.IsNullOrWhiteSpace(platformName))
                            _extent!.AddSystemInfo("Platform", platformName);
                    }
                }
            }
            catch { /* ignore – capabilities aren’t critical */ }
        }

        // --------------------- STEP ---------------------
        [AfterStep(Order = int.MaxValue)]
        public void AfterStep(ScenarioContext scenario)
        {
            var text = scenario.StepContext.StepInfo.Text;
            var safeText = HtmlSafe(text); // ✅ encode the title shown in the report

            var type = scenario.StepContext.StepInfo.StepDefinitionType;

            ExtentTest stepNode = type switch
            {
                Reqnroll.Bindings.StepDefinitionType.Given => _scenarioNode.Value!.CreateNode<Given>(safeText),
                Reqnroll.Bindings.StepDefinitionType.When => _scenarioNode.Value!.CreateNode<When>(safeText),
                Reqnroll.Bindings.StepDefinitionType.Then => _scenarioNode.Value!.CreateNode<Then>(safeText),
                _ => _scenarioNode.Value!.CreateNode<And>(safeText)
            };

            // Flush buffered Info logs from step classes into this step node
            FlushBufferedLogsTo(stepNode);

            if (scenario.TestError is null)
            {
                stepNode.Pass("Passed");
            }
            else
            {
                string path = "";
                try
                {
                    var driver = _container.Resolve<IWebDriver>();
                    var sc = ((ITakesScreenshot)driver).GetScreenshot();
                    path = SaveScreenshot(text);
                    sc.SaveAsFile(path);

                    var rel = Path.GetRelativePath(_reportDir, path);
                    stepNode.Fail(scenario.TestError, MediaEntityBuilder.CreateScreenCaptureFromPath(rel).Build());
                }
                catch
                {
                    stepNode.Fail(scenario.TestError);
                }
                Console.WriteLine($"[REPORT] Fail screenshot: {path}");
            }
        }

        // --------------------- AFTER TEST RUN ---------------------
        [AfterTestRun(Order = int.MaxValue)]
        public static void FlushReport()
        {
            lock (_lock)
            {
                Console.WriteLine($"[REPORT] Flushing: {_reportPath}");
                try
                {
                    _extent?.Flush();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[REPORT] Flush failed: {ex.Message}");
                }
                finally
                {
                    _spark = null;
                    _extent = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }

        // --------------------- Helpers ---------------------
        private static string SaveScreenshot(string stepText)
        {
            var baseDir = AppContext.BaseDirectory;
            var screens = Path.Combine(baseDir, "Screenshots");
            Directory.CreateDirectory(screens);

            var file = $"SCR_{Sanitize(stepText)}_{DateTime.UtcNow:yyyyMMdd_HHmmss_fff}_{Thread.CurrentThread.ManagedThreadId}.png";
            return Path.Combine(screens, file);
        }

        private static string Sanitize(string s)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                s = s.Replace(c, '_');
            return s.Length > 60 ? s.Substring(0, 60) : s;
        }

        private void FlushBufferedLogsTo(ExtentTest node)
        {
            TryPipe<qa_dotnet_cucumber.Steps.ProfileLanguagesCrudSteps>(node);
            TryPipe<qa_dotnet_cucumber.Steps.ProfileSkillsCrudSteps>(node);
        }

        private void TryPipe<T>(ExtentTest node)
        {
            try
            {
                var obj = _container.Resolve<T>();
                var logs = (obj as dynamic).GetLogs() as System.Collections.Generic.IReadOnlyList<string>;
                if (logs != null && logs.Count > 0)
                {
                    foreach (var line in logs)
                        node.Info(HtmlSafe(line));  
                    (obj as dynamic).ClearLogs();
                }
            }
            catch {/* not all step classes are in every scenario */  }
        }
    }
}
