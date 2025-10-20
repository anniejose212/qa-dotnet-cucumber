using NUnit.Framework;
using OpenQA.Selenium;
using qa_dotnet_cucumber.Pages;
using qa_dotnet_cucumber.Support;
using Reqnroll;
using Reqnroll.BoDi;

namespace qa_dotnet_cucumber.Steps
{
    [Binding]
    public class ProfileLanguagesCrudSteps : StepBase
    {
        /// ===== Fields =====
        private readonly IObjectContainer _container;
        private readonly IWebDriver _driver;
        private readonly ProfileLanguagesCrudPage page;
        public List<(string Language, string Level)> Added = new();

        private bool _invalidLevelExistedInDropdown = false;

        private string _lastSubmittedLanguage = "";
        private string _lastSubmittedLevel = "";
        private const string SECURITY_POLICY =
"POLICY EXPECTATION: Unsafe input must be rejected server-side; no record created or displayed. " +
"Enforce strict allowlist validation and HTML-encode on render.";

        // Creates step context and resolves page + driver
        public ProfileLanguagesCrudSteps(IObjectContainer container)
        {
            _container = container;
            _driver = container.Resolve<IWebDriver>();
            page = _container.Resolve<ProfileLanguagesCrudPage>();
        }

        // Adds a single language with level
        [When("I add language {string} with level {string}")]
        public void WhenIAddALanguageWithLevel(string language, string level)
        {
            page.AddLanguage(language, level);
            Added.Add((language, level));
        }

        // Updates a language to a new level
        [When("I update language {string} to level {string}")]
        public void WhenIUpdateLanguageToLevel(string language, string newLevel)
        {
            page.EditLanguageLevel(language, newLevel);
            Added.Add((language, newLevel));
        }

        // Deletes a specific language+level
        [When("I delete language {string} with level {string}")]
        public void WhenIDeleteLanguage(string language, string level)
        {
            page.Delete(language, level);
        }

        // Adds multiple languages from a table
        [When("I add the following languages")]
        public void WhenIAddTheFollowingLanguages(Table table)
        {
            foreach (var row in table.Rows)
            {
                var language = row["Language"];
                var level = row["Level"];
                page.AddLanguage(language, level);
                Added.Add((language, level));
            }
        }

        // Attempts to add language with an invalid level option
        [When("I attempt to add language {string} with invalid level {string}")]
        public void WhenIAttemptToAddLanguageWithInvalidLevel(string language, string invalidLevel)
        {
            language = TestDataHelper.NormalizeTestInput(language);
            invalidLevel = TestDataHelper.NormalizeTestInput(invalidLevel);

            _invalidLevelExistedInDropdown = page.AddLanguageAllowingInvalidLevel(language, invalidLevel);
            Added.Add((language, invalidLevel));
        }

        // Submits unsafe language payload without waits (for XSS checks)
        [When(@"I add potentially unsafe language ""(.*)"" with level ""(.*)""")]
        public void WhenIAddPotentiallyUnsafeLanguage(string language, string level)
        {
            language = TestDataHelper.NormalizeTestInput(language);
            level = TestDataHelper.NormalizeTestInput(level);

            _lastSubmittedLanguage = language;
            _lastSubmittedLevel = level;

            page.SubmitLanguageRaw(language, level); // leaves any JS alert open
            Added.Add((language, level));
        }

        // Asserts a success toast is shown
        [Then("I should see a success toast")]
        public void ThenIShouldSeeASuccessToast()
        {
            var toast = page.GetSuccessToastText();
            Assert.That(toast, Is.Not.Null.And.Not.Empty, "Expected a success toast.");
            Info($"Success toast: {toast}");
            TestContext.Progress.WriteLine($"Success toast: {toast}");
        }

        // Asserts an error toast is shown
        [Then("I should see an error toast")]
        public void ThenIShouldSeeAnErrorToast()
        {
            var toast = page.GetErrorToastText();
            Assert.That(toast, Is.Not.Null.And.Not.Empty, "Expected an error toast but none appeared.");
            Info($"Error toast: {toast}");
            TestContext.WriteLine($"Error toast: {toast}");
        }

        // Asserts a language+level is visible
        [Then("I should see language {string} with level {string}")]
        public void ThenIShouldSeeLanguageWithLevel(string language, string level)
        {
            AssertLanguageVisibility(language, level, true);
        }

        // Asserts count of occurrences for language+level
        [Then("I should see {int} occurrences of language {string} with level {string}")]
        public void ThenIShouldSeeOccurrencesOfLanguageWithLevel(int expectedCount, string language, string level)
        {
            var actual = page.CountLanguageWithLevel(language, level);
            Assert.That(actual, Is.EqualTo(expectedCount),
                $"Expected {expectedCount} occurrence(s) of (\"{language}\", \"{level}\") but found {actual}.");
        }

        // Asserts a language+level is not visible
        [Then("I should not see language {string} with level {string}")]
        public void ThenIShouldNotSeeLanguageWithLevel(string language, string level)
        {
            AssertLanguageVisibility(language, level, false);
        }

        // Asserts the total number of languages in list
        [Then("I should see {int} languages in the list")]
        public void ThenIShouldSeeLanguagesInTheList(int expected)
        {
            var actual = page.GetLanguages().Count;
            Assert.That(actual, Is.EqualTo(expected),
                $"Expected {expected} language(s) but found {actual}.");
        }

        // Asserts Add New button is hidden when rows exist
        [Then("the Add New button should be hidden for languages")]
        public void ThenTheAddNewButtonShouldBeHiddenForLanguages()
        {
            var rows = page.GetLanguages();
            int count = 0;
            foreach (var row in rows)
                if (!string.IsNullOrWhiteSpace(row.Language))
                    count++;

            Assert.That(page.IsAddNewButtonDisplayed(), Is.False,
                $"Expected 'Add New' button to be hidden, but it was visible. Found {count} languages.");
        }

        // Asserts an appropriate error toast for invalid language/level
        [Then(@"I should see an error toast for invalid language or level")]
        public void ThenIShouldSeeAnErrorToastForInvalidLanguageOrLevel()
        {
            var toast = page.GetErrorToastText();
            Assert.That(toast, Is.Not.Null.And.Not.Empty,
                "Expected an error toast for invalid language/level, but none appeared.");

            var acceptable =
                toast.Contains("Please enter language and level", StringComparison.OrdinalIgnoreCase);

            Assert.That(acceptable, Is.True,
                $"Unexpected error message. Got: '{toast}'. " +
                "Expected one of: 'Please enter language and level'.");

            if (_invalidLevelExistedInDropdown)
                TestContext.Progress.WriteLine("Note: The 'invalid' level existed in the dropdown. Ensure server-side allowlist.");
            else
                TestContext.Progress.WriteLine("Level option did not exist in the dropdown, as expected.");
        }

        // Logs alert presence and row persistence for unsafe input
        [Then(@"check alert or row visibility for unsafe language input")]
        public void ThenCheckAlertOrRowVisibilityForUnsafeLanguageInput()
        {
            var alert = _driver.TryGetAlert(2);

            if (alert != null)
            {
                var text = alert.Text ?? string.Empty;
                TestContext.WriteLine($"[ALERT DETECTED] Language Alert Text: '{text}'");
                alert.Accept();
            }
            else
            {
                TestContext.WriteLine("[NO ALERT DETECTED]");
            }

            var count = page.CountLanguageWithLevel(_lastSubmittedLanguage, _lastSubmittedLevel);
            var rows = page.GetLanguages();
            var details = string.Join(", ", rows.ConvertAll(r => $"{r.Language}:{r.Level}"));

            if (count > 0)
            {
                TestContext.WriteLine(
                    $"[ROW FOUND] Unsafe language was persisted: '{_lastSubmittedLanguage}' / '{_lastSubmittedLevel}'.");
            }
            else
            {
                TestContext.WriteLine(
                    $"[NO ROW FOUND] Unsafe language did NOT persist: '{_lastSubmittedLanguage}' / '{_lastSubmittedLevel}'.");
            }

            TestContext.WriteLine($"[FULL TABLE] {details}");
        }

        // Enforces policy: no alert and no persisted unsafe record
        [Then(@"the system should reject unsafe input")]
        public void ThenSystemShouldRejectUnsafeInput()
        {
            var alert = _driver.TryGetAlert(1);
            if (alert != null)
            {
                var text = alert.Text ?? string.Empty;

                alert.Accept();

                var msg = string.Join(Environment.NewLine, new[]
                {
                    SECURITY_POLICY,
                    "Violation: A script/alert popped during unsafe input submission (XSS).",
                    $"Alert text: '{text}'",
                    "Expected: No alert; input treated as data, not code."
                });
                TestContext.WriteLine(msg);
                Assert.Fail(msg);
            }

            var count = page.CountLanguageWithLevel(_lastSubmittedLanguage, _lastSubmittedLevel);
            if (count > 0)
            {
                var rows = page.GetLanguages();
                var details = string.Join(", ", rows.ConvertAll(r => $"{r.Language}:{r.Level}"));
                var msg = string.Join(Environment.NewLine, new[]
                {
                    SECURITY_POLICY,
                    $"Violation: Unsafe input was persisted.",
                    $"Actual: found '{_lastSubmittedLanguage}' @ '{_lastSubmittedLevel}' {count} time(s).",
                    $"Current rows: [{details}]",
                    "Expected: 0 matching records."
                });
                TestContext.WriteLine(msg);
                Assert.Fail(msg);
            }

            TestContext.WriteLine("Policy OK: unsafe input rejected (no alert, no record persisted).");
        }

        // Asserts presence/absence of a language+level
        private void AssertLanguageVisibility(string language, string level, bool shouldExist)
        {
            int count = page.CountLanguageWithLevel(language, level); 
            string details = page.GetLanguagesDetails();              

            if (shouldExist && count == 0)
                Assert.Fail($"Missing (\"{language}\", \"{level}\"). Table: [{details}]");

            if (!shouldExist && count > 0)
                Assert.Fail($"Found (\"{language}\", \"{level}\") {count} time(s). Table: [{details}]");
        }
    }
}
