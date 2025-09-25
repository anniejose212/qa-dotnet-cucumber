using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace qa_dotnet_cucumber.Pages;
public class ProfileLanguagesSkillsPages
{
    private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // --- Common / Header ---
        private readonly By SuccessToastMessage = By.XPath("//div[contains(@class,'ns-box') and contains(@class,'ns-type-success')]");
        private readonly By ErrorToastMessage = By.XPath("//div[contains(@class,'ns-box') and contains(@class,'ns-type-error')]");

        // --- Tabs ---
        private readonly By LanguagesTab = By.CssSelector("a[data-tab='first']");
        private readonly By SkillsTab = By.CssSelector("a[data-tab='second']");

        // --- Languages grid ---
        private readonly By LangAddNewBtn = By.XPath("//div[normalize-space()='Add New']");
        private readonly By LangInput = By.CssSelector("input[placeholder='Add Language']");
        private readonly By LangLevelSelect = By.CssSelector("select.ui.dropdown[name='level']");
        private readonly By LangAddBtn = By.CssSelector("input[value='Add']");
        private readonly By LangUpdateBtn = By.CssSelector("input[value='Update']");
        private readonly By LangFirstRowEditIcon =By.XPath("//div[@data-tab='first']//table//tbody/tr[1]//i[contains(@class,'write icon')]");
        private readonly By LangFirstRowDeleteIcon = By.XPath("//div[@data-tab='first']//table//tbody/tr[1]//i[contains(@class,'remove icon')]");
        private readonly By LangRows = By.XPath("//div[@data-tab='first']//table//tbody/tr");

        // --- Skills grid ---
        private readonly By SkillAddNewBtn = By.XPath("//div[@data-tab='second']//div[normalize-space()='Add New']");
        private readonly By SkillNameInput = By.CssSelector("input[placeholder='Add Skill']");
        private readonly By SkillLevelSelect = By.CssSelector("select.ui.fluid.dropdown[name='level']");
        private readonly By SkillAddBtn = By.CssSelector("input[value='Add']");
        private readonly By SkillUpdateBtn = By.CssSelector("input[value='Update']");
        private readonly By SkillFirstRowEditIcon = By.XPath("//div[@data-tab='second']//table//tbody/tr[1]//i[contains(@class,'write icon')]");
        private readonly By SkillFirstRowDeleteIcon = By.XPath("//div[@data-tab='second']//table//tbody/tr[1]//i[contains(@class,'remove icon')]");
        private readonly By SkillRows = By.XPath("//div[@data-tab='second']//table//tbody/tr");


        public ProfileLanguagesSkillsPages (IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

        }

        public IReadOnlyList<(string Language, string Level)> GetLanguages()
        {
            OpenLanguagesTab();
            var rows = _driver.FindElements(LangRows);
            return rows.Select(r =>
            {
                var tds = r.FindElements(By.TagName("td"));
                return (tds.ElementAtOrDefault(0)?.Text ?? "", tds.ElementAtOrDefault(1)?.Text ?? "");
            }).ToList();
        }

        public IReadOnlyList<(string Skill, string Level)> GetSkills()
        {
            OpenSkillsTab();
            var rows = _driver.FindElements(SkillRows);
            return rows.Select(r =>
            {
                var tds = r.FindElements(By.TagName("td"));
                return (tds.ElementAtOrDefault(0)?.Text ?? "", tds.ElementAtOrDefault(1)?.Text ?? "");
            }).ToList();
        }
        // ---------- Editing: Languages ----------
        public void OpenLanguagesTab() =>
            _wait.Until(ExpectedConditions.ElementToBeClickable(LanguagesTab)).Click();

        public void AddLanguage(string language, string level)
        {
            OpenLanguagesTab();
            _wait.Until(ExpectedConditions.ElementToBeClickable(LangAddNewBtn)).Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(LangInput)).SendKeys(language);
            new SelectElement(_wait.Until(ExpectedConditions.ElementExists(LangLevelSelect))).SelectByText(level);
            _wait.Until(ExpectedConditions.ElementToBeClickable(LangAddBtn)).Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(SuccessToastMessage));
        }

        public void EditFirstLanguage(string newLanguage, string newLevel)
        {
            OpenLanguagesTab();
            _wait.Until(ExpectedConditions.ElementToBeClickable(LangFirstRowEditIcon)).Click();
            var input = _wait.Until(ExpectedConditions.ElementIsVisible(LangInput));
            input.Clear(); input.SendKeys(newLanguage);
            new SelectElement(_wait.Until(ExpectedConditions.ElementExists(LangLevelSelect))).SelectByText(newLevel);
            _wait.Until(ExpectedConditions.ElementToBeClickable(LangUpdateBtn)).Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(SuccessToastMessage));
        }

        public void DeleteFirstLanguage(string language)
        {
            OpenLanguagesTab();
            _wait.Until(ExpectedConditions.ElementToBeClickable(LangFirstRowDeleteIcon)).Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(SuccessToastMessage));
        }
        // ---------- Editing: Skills ----------
        public void OpenSkillsTab() =>
            _wait.Until(ExpectedConditions.ElementToBeClickable(SkillsTab)).Click();

        public void AddSkill(string skill, string level)
        {
            OpenSkillsTab();
            _wait.Until(ExpectedConditions.ElementToBeClickable(SkillAddNewBtn)).Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(SkillNameInput)).SendKeys(skill);
            new SelectElement(_wait.Until(ExpectedConditions.ElementExists(SkillLevelSelect))).SelectByText(level);
            _wait.Until(ExpectedConditions.ElementToBeClickable(SkillAddBtn)).Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(SuccessToastMessage));
        }

        public void EditFirstSkill(string newSkill, string newLevel)
        {
            OpenSkillsTab();
            _wait.Until(ExpectedConditions.ElementToBeClickable(SkillFirstRowEditIcon)).Click();
            var input = _wait.Until(ExpectedConditions.ElementIsVisible(SkillNameInput));
            input.Clear(); input.SendKeys(newSkill);
            new SelectElement(_wait.Until(ExpectedConditions.ElementExists(SkillLevelSelect))).SelectByText(newLevel);
            _wait.Until(ExpectedConditions.ElementToBeClickable(SkillUpdateBtn)).Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(SuccessToastMessage));
        }

        public void DeleteFirstSkill(string skill)
        {
            OpenSkillsTab();
            _wait.Until(ExpectedConditions.ElementToBeClickable(SkillFirstRowDeleteIcon)).Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(SuccessToastMessage));
        }


    }

    

