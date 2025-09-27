using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace qa_dotnet_cucumber.Pages
{
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
        private readonly By LanguagesPane = By.CssSelector("div[data-tab='first']");
        private readonly By LangRows = By.XPath("//div[@data-tab='first']//table//tbody/tr");

        // --- Languages grid controls ---
        private readonly By LangAddNewBtn = By.XPath("//div[normalize-space()='Add New']");
        private readonly By LangInput = By.CssSelector("input[placeholder='Add Language']");
        private readonly By LangLevelSelect = By.CssSelector("select.ui.dropdown[name='level']");
        private readonly By LangAddBtn = By.CssSelector("input[value='Add']");
        private readonly By LangUpdateBtn = By.CssSelector("input[value='Update']");
        private readonly By LangFirstRowEditIcon = By.XPath("//div[@data-tab='first']//table//tbody/tr[1]//i[contains(@class,'write icon')]");
        private readonly By LangFirstRowDeleteIcon = By.XPath("//div[@data-tab='first']//table//tbody/tr[1]//i[contains(@class,'remove icon')]");

        // --- Skills grid  ---
        private readonly By SkillAddNewBtn = By.XPath("//div[@data-tab='second']//div[normalize-space()='Add New']");
        private readonly By SkillNameInput = By.CssSelector("input[placeholder='Add Skill']");
        private readonly By SkillLevelSelect = By.CssSelector("select.ui.fluid.dropdown[name='level']");
        private readonly By SkillAddBtn = By.CssSelector("input[value='Add']");
        private readonly By SkillUpdateBtn = By.CssSelector("input[value='Update']");
        private readonly By SkillFirstRowEditIcon = By.XPath("//div[@data-tab='second']//table//tbody/tr[1]//i[contains(@class,'write icon')]");
        private readonly By SkillFirstRowDeleteIcon = By.XPath("//div[@data-tab='second']//table//tbody/tr[1]//i[contains(@class,'remove icon')]");
        private readonly By SkillRows = By.XPath("//div[@data-tab='second']//table//tbody/tr");
        private readonly By SkillsPane = By.CssSelector("div[data-tab='second']");

        public ProfileLanguagesSkillsPages(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }


        // ---------- Read languages table ----------
        public List<(string Language, string Level)> GetLanguages()
        {
            
            var languages = new List<(string Language, string Level)>();

            
            var rows = _driver.FindElements(LangRows);

           
            foreach (var row in rows)
            {
                
                var cells = row.FindElements(By.TagName("td"));

                
                string language = "";
                string level = "";

                
                if (cells.Count > 0)
                {
                    language = cells[0].Text.Trim();
                }

                if (cells.Count > 1)
                {
                    level = cells[1].Text.Trim();
                }

            
                languages.Add((language, level));
            }

            
            return languages;
        }

        // ---------- Read skills table ----------

        public List<(string Skill, string Level)> GetSkills()
        {
            var skills = new List<(string Skill, string Level)>();

            
            var rows = _driver.FindElements(SkillRows);

            foreach (var row in rows)
            {
           
                var cells = row.FindElements(By.TagName("td"));

                
                string skill = "";
                if (cells.Count > 0)
                {
                    skill = cells[0].Text.Trim();
                }

                
                string level = "";
                if (cells.Count > 1)
                {
                    level = cells[1].Text.Trim();
                }

                
                skills.Add((skill, level));
            }

            return skills;
        }

    }

}

