using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace qa_dotnet_cucumber.Pages
{
    public class ProfileSkillsCrudPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // --- Toasts ---
        private readonly By SuccessToastMessage = By.XPath("//div[contains(@class,'ns-box') and contains(@class,'ns-type-success')]");

        // --- Skills controls ---
        private readonly By SkillsPane = By.CssSelector("div[data-tab='second']");
        private readonly By SkillRows = By.XPath("//div[@data-tab='second']//table//tbody/tr");
        private readonly By SkillAddNewBtn = By.XPath("//div[@data-tab='second']//div[normalize-space()='Add New']");
        private readonly By SkillNameInput = By.CssSelector("input[placeholder='Add Skill']");
        private readonly By SkillLevelSelect = By.CssSelector("select.ui.fluid.dropdown[name='level']");
        private readonly By SkillAddBtn = By.CssSelector("div[data-tab='second'] input[value='Add']");
        private readonly By SkillUpdateBtn = By.CssSelector("div[data-tab='second'] input[value='Update']");
        private readonly By SkillFirstRowEditIcon = By.XPath("//div[@data-tab='second']//table//tbody/tr[1]//i[contains(@class,'write icon')]");
        private readonly By SkillFirstRowDeleteIcon = By.XPath("//div[@data-tab='second']//table//tbody/tr[1]//i[contains(@class,'remove icon')]");
        private readonly By SkillsTab = By.CssSelector("a[data-tab='second']");
        public ProfileSkillsCrudPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        public void OpenSkillsTab(WebDriverWait wait)
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(SkillsTab)).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(SkillsPane));
        }

        // CREATE 
        public void AddSkill(string skill, string level)
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(SkillsPane));
            _wait.Until(ExpectedConditions.ElementToBeClickable(SkillAddNewBtn)).Click();

            var input = _wait.Until(ExpectedConditions.ElementIsVisible(SkillNameInput));
            input.Clear();
            input.SendKeys(skill);

            var select = new SelectElement(_wait.Until(ExpectedConditions.ElementIsVisible(SkillLevelSelect)));
            select.SelectByText(level);

            _wait.Until(ExpectedConditions.ElementToBeClickable(SkillAddBtn)).Click();

            WaitUntilSkillAppears(skill);
        }

        // UPDATE 
        public void EditSkillLevel(string newLevel)
        {
            var rows = _driver.FindElements(SkillRows);
            if (rows.Count == 0)
                throw new Exception("No skills to edit.");

            _wait.Until(ExpectedConditions.ElementToBeClickable(SkillFirstRowEditIcon)).Click();
            var input = _wait.Until(ExpectedConditions.ElementIsVisible(SkillNameInput));
            new SelectElement(_wait.Until(ExpectedConditions.ElementExists(SkillLevelSelect))).SelectByText(newLevel);
            _wait.Until(ExpectedConditions.ElementToBeClickable(SkillUpdateBtn)).Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(SuccessToastMessage)); ;
        }

        // DELETE
        public void DeleteSkill()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(SkillFirstRowDeleteIcon)).Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(SuccessToastMessage));

      
        }

        // READ
        public List<(string Skill, string Level)> GetSkills()
        {
            var skills = new List<(string Skill, string Level)>();
            var rows = _driver.FindElements(SkillRows);

            foreach (var row in rows)
            {
                var cells = row.FindElements(By.TagName("td"));

                string skill = "";
                string level = "";

                if (cells.Count > 0)
                {
                    skill = cells[0].Text.Trim();
                }

                if (cells.Count > 1)
                {
                    level = cells[1].Text.Trim();
                }

                skills.Add((skill, level));
            }

            return skills;
        }

        public string GetSuccessToastText()
        {
            try
            {
                var el = _wait.Until(ExpectedConditions.ElementIsVisible(SuccessToastMessage));
                return el.Text?.Trim() ?? "";
            }
            catch
            {
                return "";
            }
        }

        //  Helpers 
        private void WaitUntilSkillAppears(string skill)
        {
            _wait.Until(_ =>
            {
                var rows = _driver.FindElements(SkillRows);
                foreach (var r in rows)
                {
                    var cells = r.FindElements(By.TagName("td"));
                    if (cells.Count > 0 &&
                        cells[0].Text.Trim().Equals(skill.Trim(), StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                return false;
            });
        }
    }
}
