
🔑 Migration Notes – From Hooks.cs to ReportHook.cs

 1. Reporting code moved

Before: Hooks.cs contained ExtentReports setup (_extent, _htmlReporter, _test, _reportLock) and step logging inside AfterStep.

Now: All reporting logic is isolated in ReportHook.cs.
Hooks.cs is now only responsible for WebDriver setup/teardown and DI registrations.

 2. Report folder location

Before: Report path was built by walking up from bin/Debug/netX.Y/ to project root → Reports/....

Now: Report is written directly to the bin folder (e.g. bin\\Debug\\net8.0\\TestReport.html).

var baseDir = AppContext.BaseDirectory; 
_reportDir = baseDir; // report folder in bin

 3. Screenshots location

Before: Saved in current working directory with filenames like Screenshot_{ticks}_{thread}.png.

Now: Always saved in a dedicated folder:
bin\\Debug\\net8.0\\Screenshots\\SCR_<step>_<timestamp>.png

var screens = Path.Combine(baseDir, "Screenshots");
Directory.CreateDirectory(screens);

 4. ExtentReport structure

Before: Steps were logged as plain text with _test.Log(Status.Pass/Fail, ...).

Now: Each Gherkin step creates a real Extent node (Given/When/Then/And), and buffered logs (Info(...) calls in step classes) are piped into that step node.

 5. Failures with screenshots

Before: On failure, screenshot path was absolute, and sometimes not embedded properly.

Now: Screenshots are saved into the Screenshots folder, and the image is attached directly to the failing step node via MediaEntityBuilder.

 6. System info

Before: Environment and Browser type were set in BeforeTestRun.

Now: Same info, but additionally tries to pull browserVersion and platformName from WebDriver capabilities in BeforeScenario.



🔑 Key Changes to LoginPage

Locators expanded

Added SignInLink locator to support opening the sign-in modal/page.

Updated UsernameField locator to handle multiple possible attributes (type='email', placeholder='Email address', name='Email', or id='email').

Updated SuccessMessage locator to capture both Sign Out buttons (<button> or <a>).

Added explicit locators for inline error prompts:

PasswordErrorPrompt (password length validation).

EmailErrorPrompt (invalid email format).

ErrorToast (confirmation / general error popup).

New methods

OpenSignIn() – clicks the Sign In link before login.

GetEmailInlineError(), GetPasswordInlineError(), GetPopupError() – helper methods for fetching inline and popup errors.

IsAtLoginPage updated

Now waits for visibility of the username field instead of checking page title.

🔑 Key Changes to LoginSteps

Navigation

Changed from NavigateTo("/login") → NavigateTo("/") followed by _loginPage.OpenSignIn() to adapt to new flow.

Credentials

Updated valid credentials to "annie.jose1202@gmail.com" / "123456" (instead of the old tomsmith / SuperSecretPassword!).

Assertions for success

Changed success validation from checking "You logged into a secure area!" → checking that "Sign Out" is visible.

Error handling

Extended error validation:

First check for inline prompts under email or password fields.

If no inline errors, fallback to toast/popup error elements (.ns-box, .ui.error.message, .alert-danger, etc.).

Assertions now cover a broader set of error patterns: "invalid" | "incorrect" | "unauthor" | "confirm" | "verification" | "failed" | "required".

🔑 Key Changes to Config (TestSettings.json)

Base URL updated

From: http://the-internet.herokuapp.com

To: http://localhost:5003 (pointing tests to local environment).


🔑ProfileOverviewPage

Implemented navigation to the Profile section (OpenProfile).

Added editing of basic info: first name and last name, followed by save and toast validation.

Added support for verifying the display name.

Common locators for success/error toasts introduced.

🔑ProfileLanguagesCrudPage

Created full CRUD support for Languages:

AddLanguage with dynamic waits, input clearing, dropdown selection, and success toast validation.

EditLanguageLevel to change proficiency level of the first listed language.

DeleteLanguage with success toast confirmation.

GetLanguages returning a list of (Language, Level) pairs.

Added helper: WaitUntilLanguageAppears to ensure data consistency after insertion.

Added GetSuccessToastText with exception handling for resilience.

🔑ProfileSkillsCrudPage

Created full CRUD support for Skills:

AddSkill with waits, input clearing, dropdown selection, and success toast validation.

EditSkillLevel to update proficiency level of the first listed skill.

DeleteSkill with toast validation.

GetSkills returning (Skill, Level) pairs.

Added OpenSkillsTab to support multi-tab navigation.

Added helper: WaitUntilSkillAppears for validation.

Introduced GetSuccessToastText with safe handling if toast is missing.

🔑ProfileLanguagesCrudSteps

Defined BDD step bindings for language CRUD scenarios:

Add, edit, and delete actions mapped to Gherkin steps.

Assertions for presence and absence of languages.

Log buffering system for test visibility (console + retained logs).

Utility step to print last language entry for debugging.

🔑ProfileSkillsCrudSteps

Defined BDD step bindings for skill CRUD scenarios:

Add, edit, and delete actions mapped to Gherkin steps.

Assertions for presence and absence of skills.

Added step to explicitly open the Skills tab.

Utility step to print last skill entry for debugging.

Logging helpers similar to language steps.

🔑Feature Files

ProfileLanguagesCRUD.feature: Defined scenarios for adding, updating, deleting, and verifying languages.

ProfileSkillsCRUD.feature: Defined scenarios for skills with the same structure.