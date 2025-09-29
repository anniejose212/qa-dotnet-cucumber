using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace qa_dotnet_cucumber.Pages
{
    public class ProfileLanguagesCrudPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        //  Toasts 
        private readonly By SuccessToastMessage = By.XPath("//div[contains(@class,'ns-box') and contains(@class,'ns-type-success')]");

        // Languages controls 
        private readonly By LanguagesPane = By.CssSelector("div[data-tab='first']");
        private readonly By LangRows = By.XPath("//div[@data-tab='first']//table//tbody/tr");
        private readonly By LangAddNewBtn = By.XPath("//div[@data-tab='first']//div[normalize-space()='Add New']");
        private readonly By LangInput = By.CssSelector("input[placeholder='Add Language']");
        private readonly By LangLevelSelect = By.CssSelector("select.ui.dropdown[name='level']");
        private readonly By LangAddBtn = By.CssSelector("div[data-tab='first'] input[value='Add']");
        private readonly By LangUpdateBtn = By.CssSelector("div[data-tab='first'] input[value='Update']");
        private readonly By LangFirstRowEditIcon = By.XPath("//div[@data-tab='first']//table//tbody/tr[1]//i[contains(@class,'write icon')]");
        private readonly By LangFirstRowDeleteIcon = By.XPath("//div[@data-tab='first']//table//tbody/tr[1]//i[contains(@class,'remove icon')]");

        public ProfileLanguagesCrudPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

        }


        // CREATE 
        public void AddLanguage(string language, string level)
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(LanguagesPane));
            _wait.Until(ExpectedConditions.ElementToBeClickable(LangAddNewBtn)).Click();

            var input = _wait.Until(ExpectedConditions.ElementIsVisible(LangInput));
            input.Clear();
            input.SendKeys(language);

            var select = new SelectElement(_wait.Until(ExpectedConditions.ElementIsVisible(LangLevelSelect)));
            select.SelectByText(level);

            _wait.Until(ExpectedConditions.ElementToBeClickable(LangAddBtn)).Click();

            WaitUntilLanguageAppears(language);
        }

        // UPDATE 
        public void EditLanguageLevel(string newLevel)
        {
            var rows = _driver.FindElements(LangRows);
            if (rows.Count == 0)
                throw new Exception("No languages to edit.");

            _wait.Until(ExpectedConditions.ElementToBeClickable(LangFirstRowEditIcon)).Click();
            var input = _wait.Until(ExpectedConditions.ElementIsVisible(LangInput));
            new SelectElement(_wait.Until(ExpectedConditions.ElementExists(LangLevelSelect))).SelectByText(newLevel);
            _wait.Until(ExpectedConditions.ElementToBeClickable(LangUpdateBtn)).Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(SuccessToastMessage)); ;
        }

        // DELETE
        public void DeleteLanguage()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(LangFirstRowDeleteIcon)).Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(SuccessToastMessage));

           
        }

        // READ 
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

        // ---------------- Helpers ----------------
        private void WaitUntilLanguageAppears(string language)
        {
            _wait.Until(_ =>
            {
                var rows = _driver.FindElements(LangRows);
                foreach (var r in rows)
                {
                    var cells = r.FindElements(By.TagName("td"));
                    if (cells.Count > 0 &&
                        cells[0].Text.Trim().Equals(language.Trim(), StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                return false;
            });
        }
    }
}
