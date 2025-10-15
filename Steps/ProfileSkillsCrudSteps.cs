using NUnit.Framework;
using OpenQA.Selenium;

using qa_dotnet_cucumber.Pages;
using qa_dotnet_cucumber.Support;
using Reqnroll;
using Reqnroll.BoDi;

namespace qa_dotnet_cucumber.Steps
{
    [Binding]
    public class ProfileSkillsCrudSteps : StepBase
    {
        /// ===== Fields =====
        private readonly IObjectContainer _container;
        private readonly IWebDriver _driver;
        private readonly ProfileSkillsCrudPage page;

        public List<(string Skill, string Level)> Added = new();
        private bool _invalidLevelExistedInDropdown = false;


        private string _lastSubmittedSkill = "";
        private string _lastSubmittedLevel = "";

        private const string SECURITY_POLICY =
            "POLICY EXPECTATION: Unsafe input must be rejected server-side; no record created or displayed. " +
            "Enforce strict allowlist validation and HTML-encode on render.";

        // Creates step context and resolves page + driver
        public ProfileSkillsCrudSteps(IObjectContainer container)
        {
            _container = container;
            _driver = container.Resolve<IWebDriver>();
            page = _container.Resolve<ProfileSkillsCrudPage>();
        }

        // Opens the Skills tab
        [Given(@"I open the Skills Tab")]
        public void GivenIOpenTheSkillsTab()
        {
            page.OpenSkillsTab();
        }

        // Adds a single skill with level
        [When("I add skill {string} with level {string}")]
        public void WhenIAddSkillWithLevel(string skill, string level)
        {
            page.AddSkill(skill, level);
            Added.Add((skill, level));
        }

        // Updates a skill to a new level
        [When("I update skill {string} to level {string}")]
        public void WhenIUpdateSkillToLevel(string skill, string newLevel)
        {
            page.EditSkillLevel(skill, newLevel);
            Added.Add((skill, newLevel));
        }

        // Deletes a specific skill+level
        [When("I delete skill {string} with level {string}")]
        public void WhenIDeleteSkillWithLevel(string skill, string level)
        {
            page.Delete(skill, level);
        }

        // Adds multiple skills from a table
        [When("I add the following skills")]
        public void WhenIAddTheFollowingSkills(Table table)
        {
            foreach (var row in table.Rows)
            {
                var skill = row["Skill"];
                var level = row["Level"];
                page.AddSkill(skill, level);
                Added.Add((skill, level));
            }
        }

        // Attempts to add skill with an invalid level option
        [When("I attempt to add skill {string} with invalid level {string}")]
        public void WhenIAttemptToAddSkillWithInvalidLevel(string skill, string invalidLevel)
        {
            skill = TestDataHelper.NormalizeTestInput(skill);
            invalidLevel = TestDataHelper.NormalizeTestInput(invalidLevel);

            _invalidLevelExistedInDropdown = page.AddSkillAllowingInvalidLevel(skill, invalidLevel); 
            Added.Add((skill, invalidLevel));
        }

        // Submits unsafe skill payload without waits (for XSS checks)
        [When(@"I add potentially unsafe skill ""(.*)"" with level ""(.*)""")]
        public void WhenIAddPotentiallyUnsafeSkill(string skill, string level)
        {
            skill = TestDataHelper.NormalizeTestInput(skill);
            level = TestDataHelper.NormalizeTestInput(level);

            _lastSubmittedSkill = skill;
            _lastSubmittedLevel = level;

            page.SubmitSkillRaw(skill, level); // leaves any JS alert open
            Added.Add((skill, level));
        }

        // Asserts a success toast is shown
        [Then("I should see a success message toast")]
        public void ThenIShouldSeeASuccessMessageToast()
        {
            var toast = page.GetSuccessToastText();
            Assert.That(toast, Is.Not.Null.And.Not.Empty, "Expected a success toast.");
            Info($"Success toast: {toast}");
            TestContext.Progress.WriteLine($"Success toast: {toast}");
        }

        // Asserts an error toast is shown
        [Then("I should see an error message toast")]
        public void ThenIShouldSeeAnErrorMessageToast()
        {
            var toast = page.GetErrorToastText();
            Assert.That(toast, Is.Not.Null.And.Not.Empty, "Expected an error toast but none appeared.");
            Info($"Error toast: {toast}");
            TestContext.WriteLine($"Error toast: {toast}");
        }
        [Then("I should see an error toast for invalid skill or level")]
        public void ThenIShouldSeeAnErrorToastForInvalidSkillOrLevel()
        {
            var toast = page.GetErrorToastText();
            Assert.That(toast, Is.Not.Null.And.Not.Empty,
                "Expected an error toast for invalid language/level, but none appeared.");

            var acceptable =
                toast.Contains("Please enter skill and experience level", StringComparison.OrdinalIgnoreCase);

            Assert.That(acceptable, Is.True,
                $"Unexpected error message. Got: '{toast}'. " +
                "Expected one of: 'Please enter skill and experience level'.");

            if (_invalidLevelExistedInDropdown)
                TestContext.Progress.WriteLine("Note: The 'invalid' level existed in the dropdown. Ensure server-side allowlist.");
            else
                TestContext.Progress.WriteLine("Level option did not exist in the dropdown, as expected.");
        }


        // Asserts a skill+level is visible
        [Then("I should see skill {string} with level {string}")]
        public void ThenIShouldSeeSkillWithLevel(string skill, string level)
        {
            AssertSkillVisibility(skill, level, true);
        }

        // Asserts count of occurrences for skill+level
        [Then("I should see {int} occurrences of skill {string} with level {string}")]
        public void ThenIShouldSeeOccurrencesOfSkillWithLevel(int expectedCount, string skill, string level)
        {
            var actual = page.CountSkillWithLevel(skill, level);
            Assert.That(actual, Is.EqualTo(expectedCount),
                $"Expected {expectedCount} occurrence(s) of (\"{skill}\", \"{level}\") but found {actual}.");
        }

        // Asserts a skill+level is not visible
        [Then("I should not see skill {string} with level {string}")]
        public void ThenIShouldNotSeeSkillWithLevel(string skill, string level)
        {
            AssertSkillVisibility(skill, level, false);
        }


        // Asserts the total number of skills in list
        [Then("I should see {int} skills in the list")]
        public void ThenIShouldSeeSkillsInTheList(int expected)
        {
            var actual = page.GetSkills().Count;
            Assert.That(actual, Is.EqualTo(expected),
                $"Expected {expected} skill(s) but found {actual}.");
        }

        // Logs alert presence and row persistence for unsafe input
        [Then(@"check alert or row visibility for unsafe input")]
        public void ThenCheckAlertOrRowVisibilityForUnsafeInput()
        {
            var alert = _driver.TryGetAlert(2);

            if (alert != null)
            {
                var text = alert.Text ?? string.Empty;
                TestContext.WriteLine($"[ALERT DETECTED] Alert Text: '{text}'");
                alert.Accept();
            }
            else
            {
                TestContext.WriteLine("[NO ALERT DETECTED]");
            }

            var count = page.CountSkillWithLevel(_lastSubmittedSkill, _lastSubmittedLevel);
            var rows = page.GetSkills();
            var details = string.Join(", ", rows.ConvertAll(r => $"{r.Skill}:{r.Level}"));

            if (count > 0)
            {
                TestContext.WriteLine(
                    $"[ROW FOUND] Unsafe input was persisted: '{_lastSubmittedSkill}' / '{_lastSubmittedLevel}'.");
            }
            else
            {
                TestContext.WriteLine(
                    $"[NO ROW FOUND] Unsafe input did NOT persist: '{_lastSubmittedSkill}' / '{_lastSubmittedLevel}'.");
            }

            TestContext.WriteLine($"[FULL TABLE] {details}");
        }

        // Enforces policy: no alert and no persisted unsafe record
        [Then(@"the system should reject unsafe or malicious skill input")]
        public void ThenTheSystemShouldRejectUnsafeOrMaliciousSkillInput()
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

            var count = page.CountSkillWithLevel(_lastSubmittedSkill, _lastSubmittedLevel);
            if (count > 0)
            {
                var rows = page.GetSkills();
                var details = string.Join(", ", rows.ConvertAll(r => $"{r.Skill}:{r.Level}"));
                var msg = string.Join(Environment.NewLine, new[]
                {
                    SECURITY_POLICY,
                    "Violation: Unsafe input was persisted.",
                    $"Actual: found '{_lastSubmittedSkill}' @ '{_lastSubmittedLevel}' {count} time(s).",
                    $"Current rows: [{details}]",
                    "Expected: 0 matching records."
                });
                TestContext.WriteLine(msg);
                Assert.Fail(msg);
            }

            TestContext.WriteLine("Policy OK: unsafe input rejected (no alert, no record persisted).");
        }

        // Asserts presence/absence of a skill+level
        private void AssertSkillVisibility(string skill, string level, bool shouldExist)
        {
            int count = page.CountSkillWithLevel(skill, level);
            string details = page.GetSkillsDetails();

            if (shouldExist && count == 0)
                Assert.Fail($"Missing (\"{skill}\", \"{level}\"). Table: [{details}]");

            if (!shouldExist && count > 0)
                Assert.Fail($"Found (\"{skill}\", \"{level}\") {count} time(s). Table: [{details}]");
        }
    }
}
