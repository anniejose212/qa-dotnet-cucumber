// FILE: ProfileDataCleanupHooks.cs
// ROLE: Cleanup only. No auth. No TestUtils. Beginner-friendly.

using System;
using NUnit.Framework;
using qa_dotnet_cucumber.Pages;
using qa_dotnet_cucumber.Steps;
using Reqnroll;
using Reqnroll.BoDi;

namespace qa_dotnet_cucumber.Hooks
{
    [Binding]
    public class ProfileDataCleanupHooks
    {
        private readonly IObjectContainer _c;

        // Resolves container for page/steps access
        public ProfileDataCleanupHooks(IObjectContainer c) => _c = c;

        // ===== LANGUAGES =====

        // Pre-cleans Languages tab before @languages scenarios
        [BeforeScenario("languages", Order = 10)]
        public void PreCleanLanguages()
        {
            var langs = _c.Resolve<ProfileLanguagesCrudPage>();
            TestContext.WriteLine("PRE-CLEAN(languages): DeleteAllLanguages()");
            try
            {
                langs.DeleteAllLanguages();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[languages pre-clean] {ex.Message}");
            }
        }

        // Post-cleans Languages tab after @languages scenarios (tracked rows then fallback wipe)
        [AfterScenario("languages", Order = 900)]
        public void PostCleanLanguages()
        {
            var langs = _c.Resolve<ProfileLanguagesCrudPage>();

            // If step tracker exists, delete only what the scenario added.
            if (_c.IsRegistered<ProfileLanguagesCrudSteps>())
            {
                try
                {
                    var steps = _c.Resolve<ProfileLanguagesCrudSteps>();
                    TestContext.WriteLine($"TEARDOWN(languages): tracker count = {steps.Added.Count}");
                    foreach (var (language, level) in steps.Added)
                    {
                        try
                        {
                            TestContext.WriteLine($"TEARDOWN(languages): Delete '{language}' ({level})");
                            langs.Delete(language, level);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[languages teardown item] {language}/{level}: {ex.Message}");
                        }
                    }
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[languages teardown tracker] {ex.Message}");
                }
            }

            // Fallback wipe.
            TestContext.WriteLine("TEARDOWN(languages): Fallback DeleteAllLanguages()");
            try
            {
                langs.DeleteAllLanguages();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[languages fallback wipe] {ex.Message}");
            }
        }

        // ===== SKILLS =====

        // Pre-cleans Skills tab before @skills scenarios
        [BeforeScenario("skills", Order = 10)]
        public void PreCleanSkills()
        {
            var skills = _c.Resolve<ProfileSkillsCrudPage>();
            TestContext.WriteLine("PRE-CLEAN(skills): DeleteAllSkills()");
            try
            {
                skills.DeleteAllSkills();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[skills pre-clean] {ex.Message}");
            }
        }

        // Post-cleans Skills tab after @skills scenarios (tracked rows then fallback wipe)
        [AfterScenario("skills", Order = 900)]
        public void PostCleanSkills()
        {
            var skills = _c.Resolve<ProfileSkillsCrudPage>();

            // If step tracker exists, delete only what the scenario added.
            if (_c.IsRegistered<ProfileSkillsCrudSteps>())
            {
                try
                {
                    var steps = _c.Resolve<ProfileSkillsCrudSteps>();
                    TestContext.WriteLine($"TEARDOWN(skills): tracker count = {steps.Added.Count}");
                    foreach (var (skill, level) in steps.Added)
                    {
                        try
                        {
                            TestContext.WriteLine($"TEARDOWN(skills): Delete '{skill}' ({level})");
                            skills.Delete(skill, level);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[skills teardown item] {skill}/{level}: {ex.Message}");
                        }
                    }
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[skills teardown tracker] {ex.Message}");
                }
            }

            // Fallback wipe.
            TestContext.WriteLine("TEARDOWN(skills): Fallback DeleteAllSkills()");
            try
            {
                skills.DeleteAllSkills();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[skills fallback wipe] {ex.Message}");
            }
        }
    }
}
