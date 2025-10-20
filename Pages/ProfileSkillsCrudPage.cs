// FILE: ProfileSkillsCrudPage.cs
// ROLE: Page Object for Skills tab (CRUD, toasts, list utilities).

using System;
using System.Collections.Generic;
using System.Net;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using static qa_dotnet_cucumber.Support.UiTextHelper;

namespace qa_dotnet_cucumber.Pages
{
    public class ProfileSkillsCrudPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Toast locators
        private readonly By SuccessToastMessage = By.XPath("//div[contains(@class,'ns-box') and contains(@class,'ns-type-success')]");
        private readonly By ErrorToastMessage = By.XPath("//div[contains(@class,'ns-box') and contains(@class,'ns-type-error')]");

        // Skills UI locators
        private readonly By SkillsPane = By.CssSelector("div[data-tab='second']");
        private readonly By SkillRows = By.XPath("//div[@data-tab='second']//table//tbody/tr");
        private readonly By SkillAddNewBtn = By.XPath("//div[@data-tab='second']//div[contains(@class,'ui') and contains(@class,'button') and normalize-space(.)='Add New']");
        private readonly By SkillNameInput = By.CssSelector("div[data-tab='second'] input[placeholder='Add Skill']");
        private readonly By SkillLevelSelect = By.CssSelector("div[data-tab='second'] select[name='level']");
        private readonly By SkillAddBtn = By.CssSelector("div[data-tab='second'] input.ui.teal.button[value='Add']");
        private readonly By SkillUpdateBtn = By.CssSelector("div[data-tab='second'] input.ui.teal.button[value='Update']");
        private readonly By SkillFirstRowEditIcon = By.XPath("//div[@data-tab='second']//table//tbody/tr[1]//i[contains(@class,'write icon')]");
        private readonly By SkillsTab = By.CssSelector("a[data-tab='second']");
        private readonly By RowDeleteIcon = By.CssSelector("i.remove.icon");

        public ProfileSkillsCrudPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            _wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException), typeof(UnhandledAlertException));
        }

        // Opens Skills tab
        public void OpenSkillsTab()
        {
            _driver.FindElement(SkillsTab).Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(SkillsPane));
        }

        // Adds skill (normal flow)
        public void AddSkill(string skill, string level)
        {
            _driver.FindElement(SkillAddNewBtn).Click();
            _driver.FindElement(SkillNameInput).Clear();
            _driver.FindElement(SkillNameInput).SendKeys(skill);

            var select = new SelectElement(_driver.FindElement(SkillLevelSelect));
            select.SelectByText(level);

            _driver.FindElement(SkillAddBtn).Click();
            WaitUntilSkillAppears(skill);
        }

        // Submits skill without waits (XSS)
        public void SubmitSkillRaw(string skill, string level)
        {
            _driver.FindElement(SkillAddNewBtn).Click();
            _driver.FindElement(SkillNameInput).Clear();
            _driver.FindElement(SkillNameInput).SendKeys(skill);

            new SelectElement(_driver.FindElement(SkillLevelSelect)).SelectByText(level);
            _driver.FindElement(SkillAddBtn).Click();
        }

        // Skill row model
        public class SkillItem
        {
            public string Skill { get; set; }
            public string Level { get; set; }
        }

        // Gets all skills
        public List<SkillItem> GetSkills()
        {
            List<SkillItem> list = new();
            var rows = _driver.FindElements(SkillRows);

            foreach (var row in rows)
            {
                var cells = row.FindElements(By.TagName("td"));

                SkillItem item = new SkillItem();

                if (cells.Count > 0)
                    item.Skill = cells[0].Text.Trim();

                if (cells.Count > 1)
                    item.Level = cells[1].Text.Trim();

                list.Add(item);
            }

            return list;
        }

        // Edits first row skill level
        public void EditSkillLevel(string skill, string newLevel)
        {
            _driver.FindElement(SkillFirstRowEditIcon).Click();
            _driver.FindElement(SkillNameInput).Clear();
            _driver.FindElement(SkillNameInput).SendKeys(skill);

            var select = new SelectElement(_driver.FindElement(SkillLevelSelect));
            select.SelectByText(newLevel);

            _driver.FindElement(SkillUpdateBtn).Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(SuccessToastMessage));
        }

        // Deletes specific skill+level
        public void Delete(string skill, string level)
        {
            var rows = _driver.FindElements(SkillRows);

            foreach (var row in rows)
            {
                var cells = row.FindElements(By.TagName("td"));
                if (cells.Count < 2) continue;

                var skillText = cells[0].Text.Trim();
                var levelText = cells[1].Text.Trim();

                if (EqNorm(skillText, skill) && EqNorm(levelText, level))
                {
                    var deleteIcon = row.FindElement(RowDeleteIcon);
                    deleteIcon.Click();
                    _wait.Until(ExpectedConditions.ElementIsVisible(SuccessToastMessage));
                    break;
                }
            }
        }

        // Deletes all skills
        public void DeleteAllSkills()
        {
            while (true)
            {
                var rows = _driver.FindElements(SkillRows);
                if (rows.Count == 0) break;

                try
                {
                    var deleteIcon = _wait.Until(ExpectedConditions.ElementToBeClickable(RowDeleteIcon));
                    deleteIcon.Click();
                    _wait.Until(ExpectedConditions.ElementIsVisible(SuccessToastMessage));
                }
                catch (WebDriverTimeoutException)
                {
                    break;
                }
            }
        }

        // Gets success toast text
        public string GetSuccessToastText()
        {
            try
            {
                var el = _wait.Until(ExpectedConditions.ElementIsVisible(SuccessToastMessage));
                return el.Text.Trim();
            }
            catch
            {
                return "";
            }
        }

        // Gets error toast text
        public string GetErrorToastText()
        {
            try
            {
                var el = _wait.Until(ExpectedConditions.ElementIsVisible(ErrorToastMessage));
                return el.Text.Trim();
            }
            catch
            {
                return "";
            }
        }

        // Waits until skill appears
        private void WaitUntilSkillAppears(string skill)
        {
            _wait.Until(driver =>
            {
                var rows = driver.FindElements(SkillRows);
                foreach (var r in rows)
                {
                    var cells = r.FindElements(By.TagName("td"));
                    if (cells.Count > 0 && EqNorm(cells[0].Text, skill))
                        return true;
                }
                return false;
            });
        }

        // Counts skill with level
        public int CountSkillWithLevel(string skill, string level)
        {
            int count = 0;
            var rows = GetSkills();

            foreach (var row in rows)
            {
                if (EqNorm(row.Skill, skill) && EqNorm(row.Level, level))
                    count++;
            }

            return count;
        }

        // Selects level if valid
        public bool SelectLevelIfExists(string levelText)
        {
            try
            {
                var select = new SelectElement(_driver.FindElement(SkillLevelSelect));
                select.SelectByText(levelText, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Adds skill allowing invalid level
        public bool AddSkillAllowingInvalidLevel(string skill, string level)
        {
            _driver.FindElement(SkillAddNewBtn).Click();

            _driver.FindElement(SkillNameInput).Clear();
            _driver.FindElement(SkillNameInput).SendKeys(skill);

            bool levelSelected = SelectLevelIfExists(level);

            _driver.FindElement(SkillAddBtn).Click();

            return levelSelected;
        }

        // Returns full skill summary
        public string GetSkillsDetails()
        {
            var rows = GetSkills();
            string details = "";
            foreach (var r in rows)
            {
                if (details != "") details += ", ";
                details += r.Skill + ":" + r.Level;
            }
            return details;
        }
    }
}
