using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using qa_dotnet_cucumber.Pages;
using Reqnroll;
using Reqnroll.BoDi;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;

namespace qa_dotnet_cucumber.Steps
{
    [Binding]
    public class ProfileSkillsCrudSteps
    {
        private readonly IObjectContainer _container;
        private readonly IWebDriver _driver;
        private readonly List<string> _logBuffer = new();
        private readonly ProfileSkillsCrudPage page;

        public ProfileSkillsCrudSteps(IObjectContainer container)
        {
            _container = container;
            _driver = container.Resolve<IWebDriver>();
            page = _container.Resolve<ProfileSkillsCrudPage>(); 
        }
       
        
        //WHEN

        [When(@"I add a skill ""(.*)"" with level ""(.*)""")]
        public void WhenIAddASkillWithLevel(string skill, string level)
        { 
        
            page.AddSkill(skill, level);
            Info($"Added skill: {skill} / {level}");
        }

        [When(@"I change the skill level to ""(.*)""")]
        public void WhenIChangeTheSkillLevelTo(string newLevel)
        {
            page.EditSkillLevel(newLevel);
            Info($"Changed skill level to: {newLevel}");
        }

        [When(@"I delete the skill")]
        public void WhenIDeleteTheSkill()
        {
            page.DeleteSkill();
            Info("Deleted skill.");
        }

        //THEN
        [Then(@"I open the Skills Tab")]
        public void ThenIOpenTheSkillsTab()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            page.OpenSkillsTab(wait);
        }

        [Then(@"I should see a success toast for skills")]
        public void ThenIShouldSeeASuccessToast()
        {
            var toast = page.GetSuccessToastText();
            Assert.That(toast, Is.Not.Null.And.Not.Empty, "Expected a success toast.");
            Info($"Success toast: {toast}");
        }

        [Then(@"the skill ""(.*)"" with level ""(.*)"" should exist")]
        public void ThenTheSkillWithLevelShouldExist(string skill, string level)
        {
            var all = page.GetSkills();

            bool found = false;
            foreach (var row in all)
            {
                if (row.Skill.Trim().Equals(skill.Trim(), StringComparison.OrdinalIgnoreCase) &&
                    row.Level.Trim().Equals(level.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                    break;
                }
            }

            Assert.That(found, Is.True, $"Could not find skill '{skill}' with level '{level}'.");
            Info($"Verified present: {skill} / {level}");
        }

        [Then(@"print the skill")]
        public void ThenPrintTheSkill()
        {
            var all = page.GetSkills();

            if (all.Count == 0)
            {
                Info("(no skills to print)");
                return;
            }

            var last = all[^1];
            var line = $"SKILL -> {last.Skill} | LEVEL -> {last.Level}";
            Console.WriteLine(line);  // Test Explorer
            Info(line);               // Extent (flushed by Hooks.AfterStep)
        }

        [Then(@"the skill ""(.*)"" should not appear in my profile")]
        public void ThenTheSkillShouldNotAppearInMyProfile(string skill)
        {
            var all = page.GetSkills();

            foreach (var row in all)
            {
                Assert.That(!row.Skill.Trim().Equals(skill.Trim(), StringComparison.OrdinalIgnoreCase),
                    $"Skill '{skill}' is still present after delete.");
            }

            Info($"Verified absent: {skill}");
        }

        //Logging helpers (exposed to Hooks)
        public IReadOnlyList<string> GetLogs() => _logBuffer;
        public void ClearLogs() => _logBuffer.Clear();

        private void Info(string message)
        {
            _logBuffer.Add(message);
           
            Console.WriteLine($"[INFO] {message}");
        }
    }
}
