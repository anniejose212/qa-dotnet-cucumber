Profile & Login Automation (C# + Selenium + Reqnroll)

This project is a test automation suite for verifying Login and Profile CRUD operations (Overview, Languages, and Skills) using C#, Selenium WebDriver, NUnit, and Reqnroll (SpecFlow alternative).

📂 Project Structure
qa-dotnet-cucumber
│
├── Features/
│   ├── Login.feature                  # Login scenarios
│   ├── ProfileLanguagesCRUD.feature   # Language CRUD scenarios
│   └── ProfileSkillsCRUD.feature      # Skills CRUD scenarios
│
├── Pages/
│   ├── LoginPage.cs                   # Page Object for login functionality
│   ├── ProfileOverviewPage.cs         # Page Object for profile overview
│   ├── ProfileLanguagesCrudPage.cs    # Page Object for languages CRUD
│   └── ProfileSkillsCrudPage.cs       # Page Object for skills CRUD
│
├── Steps/
│   ├── LoginSteps.cs                  # Step definitions for login
│   ├── ProfileLanguagesCrudSteps.cs   # Step definitions for languages CRUD
│   └── ProfileSkillsCrudSteps.cs      # Step definitions for skills CRUD
│
├── Devlog.md                          # Development log of changes
├── readme.md                          # Project documentation
└── ...                                # Other framework/config files

⚙️ Tech Stack

Language: C#

Frameworks:

Selenium WebDriver
 – Browser automation

NUnit
 – Test framework

Reqnroll
 – BDD framework (SpecFlow-compatible)

Design Pattern: Page Object Model (POM)

Logging: Console + step logs (collected into buffers)

🚀 Features

Login

Navigate to login page and perform sign-in.

Test flows for valid credentials, invalid username, invalid password, and empty credentials.

Validate inline error messages and popup error toasts.

Verify successful login by checking presence of Sign Out element.

Profile Overview

Open profile tab and edit basic info (first/last name).

Validate success messages and check updated display name.

Languages CRUD

Add, edit, display delete, and validate languages with proficiency levels.

Success validation via toast messages.

Skills CRUD

Add, edit, display, delete, and validate skills with proficiency levels.

Support for multi-tab navigation.

Success validation via toast messages.

▶️ Running Tests

Install dependencies (NuGet):

dotnet add package Selenium.WebDriver
dotnet add package Selenium.Support
dotnet add package SeleniumExtras.WaitHelpers
dotnet add package NUnit
dotnet add package Reqnroll


Run tests with NUnit:

dotnet test