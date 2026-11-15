# SoftwareTestingExample2 – Playwright NUnit Automation Framework

[![Playwright NUnit CI](https://github.com/huntersy/SoftwareTestingExample2/actions/workflows/playwright.yml/badge.svg)](https://github.com/huntersy/SoftwareTestingExample2/actions/workflows/playwright.yml)

A C# browser automation project demonstrating Microsoft Playwright for .NET, NUnit, and the Page Object Model (POM). The framework tests the [Automation Exercise](https://automationexercise.com/) web application.

**Repository:** https://github.com/huntersy/SoftwareTestingExample2

**Target application:** https://automationexercise.com/

## Project overview

This project demonstrates maintainable browser-based test automation using:

- C# and .NET 8
- Microsoft Playwright for .NET
- NUnit test framework
- Page Object Model (POM)
- Git and GitHub
- GitHub Actions continuous integration

The framework separates test scenarios from page-specific interactions, uses reusable page objects, centralizes environment configuration, and validates application behavior through NUnit assertions.

## Technology stack

| Technology | Version / purpose |
|---|---|
| .NET | 8.0 |
| C# | Test implementation language |
| Microsoft.Playwright.NUnit | 1.64.1 |
| NUnit | 3.14.0 |
| NUnit3TestAdapter | 4.5.0 |
| Microsoft.NET.Test.Sdk | 17.8.0 |
| NUnit.Analyzers | 3.9.0 |
| coverlet.collector | 6.0.0 |
| Browser | Chromium |
| IDE | Visual Studio Code |
| CI | GitHub Actions |

## Project structure

```text
SoftwareTestingExample2/
│
├── .github/
│   └── workflows/
│       └── playwright.yml
│
├── Pages/
│   ├── HomePage.cs
│   ├── ProductsPage.cs
│   └── ProductDetailsPage.cs
│
├── Tests/
│   ├── HomePageTests.cs
│   ├── ProductSearchTests.cs
│   ├── ProductDetailsTests.cs
│   ├── ProductFilterTests.cs
│   ├── LoginTests.cs
│   ├── RegistrationTests.cs
│   └── CheckoutTests.cs
│
├── Utilities/
│   └── TestConfig.cs
│
├── SoftwareTestingExample.csproj
├── README.md
└── .gitignore
```

The login, registration, and checkout test files are placeholders unless implemented with NUnit test methods. They are not included in the current implemented test count.

## Page Object Model design

The framework separates test logic from browser interaction logic.

- **Pages:** Store page locators and reusable actions.
- **Tests:** Execute scenarios and perform assertions.
- **Utilities:** Store shared configuration, including the target base URL.

The current page objects are:

| Page object | Responsibility |
|---|---|
| `HomePage` | Homepage navigation, product display, and category interactions |
| `ProductsPage` | Product search and search-result information |
| `ProductDetailsPage` | Product name, category, price, availability, condition, and brand |

This structure reduces duplicated locator definitions and makes application changes easier to maintain.

## Implemented test coverage

The current implementation contains **12 NUnit test methods across four test classes**.

| Test class | Count | Implemented scenarios |
|---|---:|---|
| `HomePageTests` | 5 | Homepage title, Products page search-box visibility, Women category expansion, homepage URL, homepage product display |
| `ProductSearchTests` | 5 | Matching search, no-match search, partial-name search, dress search, product count validation |
| `ProductDetailsTests` | 1 | Open Blue Top from search results and verify product name |
| `ProductFilterTests` | 1 | Navigate to Women Tops category and verify products are displayed |
| **Total** | **12** | **Four test classes** |

### Test case identifiers

- Homepage: `Homepage_ShouldLoadSuccessfully`, TC02–TC05
- Product search: TC06–TC10
- Product details: TC11
- Product category: TC12

The identifiers describe the implemented scenarios. They do not imply that additional test cases have been completed.

## Environment setup

The following instructions use Windows PowerShell and the .NET 8 SDK.

### Prerequisites

Install:

1. [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2. [Git for Windows](https://git-scm.com/downloads)
3. [Visual Studio Code](https://code.visualstudio.com/)

Verify the .NET installation:

```powershell
dotnet --version
```

### 1. Clone the repository

```powershell
git clone https://github.com/huntersy/SoftwareTestingExample2.git
cd SoftwareTestingExample2
```

### 2. Restore dependencies

```powershell
dotnet restore
```

This restores the NuGet packages declared in the project file.

### 3. Build the project

```powershell
dotnet build
```

### 4. Install Playwright Chromium

For a Debug build:

```powershell
.\bin\Debug\net8.0\playwright.ps1 install chromium
```

For a Release build:

```powershell
dotnet build --configuration Release
.\bin\Release\net8.0\playwright.ps1 install chromium
```

### 5. Run the tests

```powershell
dotnet test
```

Run a specific test:

```powershell
dotnet test --filter "FullyQualifiedName~TC06_SearchReturnsMatchingProducts"
```

List discovered tests:

```powershell
dotnet test --list-tests
```

## Environment configuration

The target application URL is centralized in:

```text
Utilities/TestConfig.cs
```

The default URL is:

```text
https://automationexercise.com/
```

The framework supports overriding the target through the `BASE_URL` environment variable.

For example:

```powershell
$env:BASE_URL = "https://your-authorized-test-environment.example"
dotnet test
```

The environment variable applies to the current PowerShell session.

Using a centralized URL makes it easier to change test environments without modifying individual page objects.

## GitHub Actions continuous integration

The workflow is located at:

```text
.github/workflows/playwright.yml
```

It is configured to run on:

- Pushes to `main`
- Pull requests targeting `main`
- Manual execution through `workflow_dispatch`

The workflow performs these operations:

1. Checks out the repository.
2. Configures .NET 8.
3. Restores NuGet dependencies.
4. Builds the project in Release configuration.
5. Installs Playwright Chromium.
6. Executes the NUnit tests.
7. Uploads the TRX test-results artifact.

### CI validation

The workflow has been committed to the repository. An earlier execution tested commit `58f6089`, before the current page-object and test corrections.

That execution reported one passing test and eleven failures. The failure diagnostics included an outdated selector in the source revision executed by GitHub Actions.

The current local source has since been corrected and successfully compiled.

**Current validation status:**

| Validation | Status |
|---|---|
| Local project build | Successful; 0 warnings and 0 errors |
| Current local NUnit execution | Not yet verified |
| Updated source committed to GitHub | Pending |
| Updated GitHub Actions test execution | Pending |
| All 12 tests passing | Not yet demonstrated |

A successful build confirms that the project compiles. It does not, by itself, prove that browser tests pass.

### Test execution restrictions

The development computer has Windows Application Control restrictions that previously prevented local execution of the compiled test assembly. Windows security protections have intentionally been left enabled.

GitHub Actions provides a hosted Windows runner for CI execution. The public target application's availability and any authorization requirements must be considered when using hosted automation.

Use only an application and test environment that you own or are authorized to test.

## Current limitations

- The implemented suite currently contains 12 test methods.
- Login, registration, and checkout scenarios are not yet implemented.
- Chromium is the configured browser for the current workflow.
- Product-search and category assertions depend on the target application's current catalog and interface.
- The latest local build succeeded, but the corrected suite still requires a new test execution.
- Successful end-to-end CI execution has not yet been established for the corrected source revision.

## Potential future enhancements

After the current suite is validated, possible improvements include:

- Add login and registration test scenarios.
- Add shopping cart and checkout coverage.
- Add quantity and price validation.
- Capture screenshots and Playwright traces on failure.
- Add richer NUnit test reporting.
- Introduce test data management.
- Add Firefox and WebKit browser execution.
- Expand API testing and UI/API integration coverage.
- Improve test execution and reporting in GitHub Actions.

## Project objective

This repository demonstrates practical QA automation engineering using C#, Playwright, NUnit, and the Page Object Model.

It showcases reusable automation components, structured test scenarios, environment configuration, version control, and continuous integration.

The project distinguishes successful compilation from successful test execution and reports validation results based on actual test evidence.