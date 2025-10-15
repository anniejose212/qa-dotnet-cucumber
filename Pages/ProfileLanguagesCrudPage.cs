// FILE: ProfileLanguagesCrudPage.cs
// ROLE: POM for Languages tab — CRUD actions, toast accessors, and small helpers. 

using System;
using System.Collections.Generic;
using System.Net;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using static qa_dotnet_cucumber.Support.UiTextHelper;

namespace qa_dotnet_cucumber.Pages
{
    public class ProfileLanguagesCrudPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Toasts
        private readonly By SuccessToastMessage = By.XPath("//div[contains(@class,'ns-box') and contains(@class,'ns-type-success')]");
        private readonly By ErrorToastMessage = By.XPath("//div[contains(@class,'ns-box') and contains(@class,'ns-type-error')]");

        // Languages controls
        private readonly By LangInput = By.CssSelector("div[data-tab='first'] input[placeholder='Add Language']");
        private readonly By LangLevelSelect = By.CssSelector("div[data-tab='first'] select[name='level']");
        private readonly By LangAddNewBtn = By.XPath("//div[@data-tab='first']//div[contains(@class,'ui') and contains(@class,'button') and normalize-space(.)='Add New']");
        private readonly By LangAddBtn = By.CssSelector("div[data-tab='first'] input.ui.teal.button[value='Add']");
        private readonly By LangRows = By.XPath("//div[@data-tab='first']//table//tbody/tr");
        private readonly By LangFirstRowEditIcon = By.XPath("//div[@data-tab='first']//table//tbody/tr[1]//i[contains(@class,'write icon')]");
        private readonly By RowDeleteIcon = By.CssSelector("i.remove.icon");
        private readonly By LangUpdateBtn = By.CssSelector("div[data-tab='first'] input[value='Update']");

        public ProfileLanguagesCrudPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            _wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException), typeof(UnhandledAlertException));
        }

        // Adds language (happy path)
        public void AddLanguage(string language, string level)
        {
            _driver.FindElement(LangAddNewBtn).Click();
            _driver.FindElement(LangInput).Clear();
            _driver.FindElement(LangInput).SendKeys(language);

            var select = new SelectElement(_driver.FindElement(LangLevelSelect));
            select.SelectByText(level);

            _driver.FindElement(LangAddBtn).Click();

            _wait.Until(ExpectedConditions.ElementIsVisible(SuccessToastMessage));
            WaitUntilLanguageAppears(language);
        }

        // Row DTO
        public class LanguageItem
        {
            public string Language { get; set; }
            public string Level { get; set; }
        }

        // Gets all languages
        public List<LanguageItem> GetLanguages()
        {
            List<LanguageItem> list = new();
            var rows = _driver.FindElements(LangRows);

            foreach (var row in rows)
            {
                var cells = row.FindElements(By.TagName("td"));

                LanguageItem item = new LanguageItem();

                if (cells.Count > 0)
                    item.Language = cells[0].Text.Trim();

                if (cells.Count > 1)
                    item.Level = cells[1].Text.Trim();

                list.Add(item);
            }

            return list;
        }

        // Checks if 'Add New' button is visible
        public bool IsAddNewButtonDisplayed()
        {
            foreach (var el in _driver.FindElements(LangAddNewBtn))
                if (el.Displayed) return true;
            return false;
        }

        // Edits first row language level
        public void EditLanguageLevel(string language, string newLevel)
        {
            _driver.FindElement(LangFirstRowEditIcon).Click();
            _driver.FindElement(LangInput).Clear();
            _driver.FindElement(LangInput).SendKeys(language);

            var select = new SelectElement(_driver.FindElement(LangLevelSelect));
            select.SelectByText(newLevel);

            _driver.FindElement(LangUpdateBtn).Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(SuccessToastMessage));
        }

        // Submits language without waits (XSS/negative)
        public void SubmitLanguageRaw(string language, string level)
        {
            _driver.FindElement(LangAddNewBtn).Click();
            _driver.FindElement(LangInput).Clear();
            _driver.FindElement(LangInput).SendKeys(language);

            new SelectElement(_driver.FindElement(LangLevelSelect)).SelectByText(level);
            _driver.FindElement(LangAddBtn).Click();
            // Intentionally no toast wait and no alert handling.
        }

        // Deletes specific language+level
        public void Delete(string language, string level)
        {
            var rows = _driver.FindElements(LangRows);
            foreach (var row in rows)
            {
                var cells = row.FindElements(By.TagName("td"));
                if (cells.Count < 2) continue;

                var langText = cells[0].Text.Trim();
                var levelText = cells[1].Text.Trim();

                if (EqNorm(langText, language) && EqNorm(levelText, level))
                {
                    var deleteIcon = row.FindElement(RowDeleteIcon);
                    deleteIcon.Click();
                    _wait.Until(ExpectedConditions.ElementIsVisible(SuccessToastMessage));
                    break;
                }
            }
        }

        // Deletes all languages
        public void DeleteAllLanguages()
        {
            while (true)
            {
                var rows = _driver.FindElements(LangRows);
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

        // Waits until language appears
        public void WaitUntilLanguageAppears(string language)
        {
            _wait.Until(driver =>
            {
                foreach (var row in driver.FindElements(LangRows))
                {
                    var cell = row.FindElement(By.TagName("td"));
                    if (EqNorm(cell.Text, language))
                        return true;
                }
                return false;
            });
        }

        // Counts language with level
        public int CountLanguageWithLevel(string language, string level)
        {
            int count = 0;
            var rows = GetLanguages();

            foreach (var row in rows)
            {
                if (EqNorm(row.Language, language) && EqNorm(row.Level, level))
                {
                    count++;
                }
            }

            return count;
        }

        // Selects level if exists
        public bool SelectLevelIfExists(string levelText)
        {
            try
            {
                var select = new SelectElement(_driver.FindElement(LangLevelSelect));
                select.SelectByText(levelText, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Adds language allowing invalid level
        public bool AddLanguageAllowingInvalidLevel(string language, string level)
        {
            _driver.FindElement(LangAddNewBtn).Click();

            _driver.FindElement(LangInput).Clear();
            _driver.FindElement(LangInput).SendKeys(language);

            bool levelSelected = SelectLevelIfExists(level);

            _driver.FindElement(LangAddBtn).Click();

            return levelSelected;
        }

        // Returns full languages summary
        public string GetLanguagesDetails()
        {
            var rows = GetLanguages();
            string details = "";
            foreach (var r in rows)
            {
                if (details != "") details += ", ";
                details += r.Language + ":" + r.Level;
            }
            return details;
        }
    }
}
