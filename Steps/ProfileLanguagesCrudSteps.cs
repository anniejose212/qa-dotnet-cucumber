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
    public class ProfileLanguagesCrudSteps 
    {
        private readonly IObjectContainer _container;
        private readonly IWebDriver _driver;
        private readonly List<string> _logBuffer = new();
        private readonly ProfileLanguagesCrudPage page;

        public ProfileLanguagesCrudSteps(IObjectContainer container)
        {
            _container = container;
            _driver = container.Resolve<IWebDriver>();
            page = _container.Resolve<ProfileLanguagesCrudPage>(); 
        }
        

        //WHEN

        [When(@"I add a language ""(.*)"" with level ""(.*)""")]
        public void WhenIAddALanguageWithLevel(string language, string level)
        {
            page.AddLanguage(language, level);
            Info($"Added language: {language} / {level}");
        }

        [When(@"I change the language level to ""(.*)""")]
        public void WhenIChangeTheLanguageLevelTo(string newLevel)
        {
            
            page.EditLanguageLevel(newLevel);
            Info($"Changed  language level to: {newLevel}");
        }

        [When(@"I delete the language")]
        public void WhenIDeleteTheLanguage()
        {
           
            page.DeleteLanguage();
            Info("Deleted language.");
        }

        //THEN

        [Then(@"I should see a success toast")]
        public void ThenIShouldSeeASuccessToast()
        {
            
            var toast = page.GetSuccessToastText();
            Assert.That(toast, Is.Not.Null.And.Not.Empty, "Expected a success toast.");
            Info($"Success toast: {toast}");
        }

        [Then(@"the language ""(.*)"" with level ""(.*)"" should exist")]
        public void ThenTheLanguageWithLevelShouldExist(string language, string level)
        {
            
            var all = page.GetLanguages();

            bool found = false;
            foreach (var row in all)
            {
                if (row.Language.Trim().Equals(language.Trim(), StringComparison.OrdinalIgnoreCase) &&
                    row.Level.Trim().Equals(level.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                    break;
                }
            }

            Assert.That(found, Is.True, $"Could not find language '{language}' with level '{level}'.");
            Info($"Verified present: {language} / {level}");
        }

        [Then(@"print the language")]
        public void ThenPrintTheLanguage()
        {
          
            var all = page.GetLanguages();

            if (all.Count == 0)
            {
                Info("(no languages to print)");
                return;
            }

            var last = all[^1];
            var line = $"LANGUAGE -> {last.Language} | LEVEL -> {last.Level}";
            Console.WriteLine(line);  // Test Explorer
            Info(line);               // Extent (flushed by Hooks.AfterStep)
        }

        [Then(@"the language ""(.*)"" should not appear in my profile")]
        public void ThenTheLanguageShouldNotAppearInMyProfile(string language)
        {
            
            var all = page.GetLanguages();

            foreach (var row in all)
            {
                Assert.That(!row.Language.Trim().Equals(language.Trim(), StringComparison.OrdinalIgnoreCase),
                    $"Language '{language}' is still present after delete.");
            }

            Info($"Verified absent: {language}");
        }

        // Logging helpers (exposed to Hooks)
        public IReadOnlyList<string> GetLogs() => _logBuffer;
        public void ClearLogs() => _logBuffer.Clear();

        private void Info(string message)
        {
            _logBuffer.Add(message);
            
            Console.WriteLine($"[INFO] {message}");
        }
    }
}
