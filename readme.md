Profile & Login Automation (C# + Selenium + Reqnroll)

UI test automation for Login and Profile CRUD (Overview, Languages, Skills). Clean Page Objects, resilient waits, deterministic cleanup, and Extent HTML reporting. Built to showcase QA engineering quality to portfolio reviewers.

Tech Stack

Language: C#

Runner: NUnit

BDD: Reqnroll (SpecFlow-compatible)

Automation: Selenium WebDriver

Pattern: Page Object Model (POM)

Reporting: ExtentReports (HTML)

Project Structure
qa-dotnet-cucumber/
├─ Config/
│  └─ config.cs
├─ Features/
│  ├─ Login.feature
│  ├─ ProfileLanguagesCRUD.feature
│  └─ ProfileSkillsCRUD.feature
├─ Hooks/
│  ├─ ProfileDataCleanupHooks.cs   # data hygiene before/after scenarios
│  ├─ ReportHook.cs                # Extent report + screenshots
│  └─ WebDriverDIHook.cs           # driver init + DI container
├─ Pages/
│  ├─ LoginPage.cs
│  ├─ ProfileLanguagesCrudPage.cs
│  ├─ ProfileOverviewPage.cs
│  └─ ProfileSkillsCrudPage.cs
├─ Steps/
│  ├─ AuthSteps.cs
│  ├─ LoginSteps.cs
│  ├─ ProfileLanguagesCrudSteps.cs
│  └─ ProfileSkillsCrudSteps.cs
├─ Support/
│  ├─ AlertHelpers.cs
│  ├─ AuthHelper.cs
│  ├─ NavigationHelper.cs
│  ├─ StepBase.cs
│  ├─ TestDataHelper.cs            # {DQ} and {EQ:n} decoding for payloads
│  └─ UiTextHelper.cs              # HTML-decode + case-insensitive compare
├─ Tests/
│  └─ CucumberRunner.cs
├─ Devlog.md
├─ readme.md
├─ reqnroll.json
├─ settings.json
├─ parallel.runsettings


Coverage
Login

Valid login.

Invalid username, invalid password, empty credentials.

Whitespace around credentials.

Repeated failures with optional cooldown lockout check.

Assertions read inline prompts and popup toasts.

Profile: Languages

Add, edit, delete, and list with level dropdown.

Success toasts and table assertions.

Negative/XSS: raw submit path to avoid masking alerts.

Deterministic cleanup via hooks.

Profile: Skills

Add, edit, delete, and list on Skills tab.

Success toasts and table assertions.

Negative/XSS: raw submit path.

Deterministic cleanup via hooks.

Setup
Prereqs

.NET 8 SDK+

Chrome + matching ChromeDriver on PATH (or adjust driver in DI hook)

Local site running at the base URL configured in settings.json or config.cs

Restore
dotnet restore

Run
All tests
dotnet test

By tag
dotnet test --filter TestCategory=smoke
dotnet test --filter TestCategory=negative

Parallel (if enabled by parallel.runsettings)
dotnet test --settings parallel.runsettings

Reporting

ReportHook.cs writes an Extent HTML report to the test output folder, and the project may keep a convenience copy as TestReport.html.

On failure, screenshots are saved under the test output (e.g., bin/Debug/net8.0/Screenshots/SCR_<step>_<timestamp>.png) and attached to the failing step node.

Data Hygiene

ProfileDataCleanupHooks wipes Languages/Skills before scenarios tagged @languages or @skills.

After each scenario, it deletes only rows recorded by the step trackers; falls back to full wipe if no tracker is available.

Security & Negative Testing

Raw submit methods bypass toast waits to expose alerts or server-side validation issues.

TestDataHelper decodes tokens:

{DQ} → "

{EQ:n} → repeated = n times

UiTextHelper normalizes HTML and casing for stable assertions.
