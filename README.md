# SoftwareTestingExample2 — SauceDemo Playwright NUnit Framework

C# end-to-end UI automation using .NET 8, Microsoft Playwright, NUnit, and the Page Object Model.

- **Application:** https://www.saucedemo.com/
- **Framework:** Playwright .NET 1.64.1 / NUnit 3.14
- **Coverage:** 40 automated UI tests across 8 classes
- **Browser:** Chromium; tests execute sequentially
- **CI:** GitHub Actions smoke and full-regression workflows

## Test accounts

| Username | Password | Expected behavior |
|---|---|---|
| standard_user | secret_sauce | Normal inventory, cart, checkout flows |
| locked_out_user | secret_sauce | Login blocked with explanatory message |

SauceDemo supplies seeded accounts. The project does not create customer accounts, use registration endpoints, or rely on account-provisioning APIs.

## Test coverage (40)

| Class | Count | Scenarios |
|---|---:|---|
| HomePageTests | 5 | Inventory access, heading, catalog count, URL, initial cart badge |
| ProductSearchTests | 8 | Catalog content, product data, descriptions, prices, images, controls |
| ProductDetailsTests | 4 | Name, description, price, return navigation |
| ProductFilterTests | 4 | Name A–Z/Z–A and price low–high/high–low sorting |
| ShoppingCartTests | 8 | Add/remove, badge, contents, price, quantity, multiple items, navigation |
| LoginTests | 5 | Valid, locked-out, invalid username/password, empty credentials |
| RegistrationTests | 1 | Authenticated logout (registration is not a SauceDemo feature) |
| CheckoutTests | 5 | Required information, field validation, successful purchase |
| **Total** | **40** | |

The former Automation Exercise search, category, registration, invoice, and account API tests were replaced with functionality supported by SauceDemo.

## Smoke and regression

Six tests are tagged `Smoke`: inventory access, backpack listing, product details, alphabetical sorting, add-to-cart badge, and checkout validation.

```powershell
dotnet test --configuration Release --filter "TestCategory=Smoke"
dotnet test --configuration Release
```

| Workflow | Trigger | Scope |
|---|---|---|
| [Smoke E2E](.github/workflows/smoke.yml) | Push/PR to main, manual | 6 tests |
| [Full Regression](.github/workflows/playwright.yml) | Nightly 06:00 UTC, manual | 40 tests |

Both workflows restore/build, cache NuGet packages, install Chromium, publish NUnit TRX reports, and upload failure diagnostics for 14 days.

## Architecture

```text
Pages/       BasePage, LoginPage, ProductsPage, ProductDetailsPage, CartPage, CheckoutPage
TestData/    TestUsers
Tests/       8 NUnit test classes + UiTests lifecycle
Utilities/   TestConfig, TestLogger
.github/     smoke.yml, playwright.yml
```

## Local setup (Windows PowerShell)

Prerequisites: Git and .NET 8 SDK.

```powershell
git clone https://github.com/huntersy/SoftwareTestingExample2.git
cd SoftwareTestingExample2
dotnet restore
dotnet build
.\bin\Debug\net8.0\playwright.ps1 install chromium
dotnet test
```

## Configuration

The default URL is `https://www.saucedemo.com`, centralized in `Utilities/TestConfig.cs`. Override with `BASE_URL` when using an authorized compatible environment:

```powershell
$env:BASE_URL = "https://www.saucedemo.com"
dotnet test --configuration Release
```

The default Playwright timeout is 15 seconds. Tests use isolated browser contexts and execute sequentially to prevent shared demo-account state collisions.

## Diagnostics

A Playwright trace (screenshots, DOM snapshots, and source capture) starts for every test. Passing tests stop tracing without saving an archive. Failed tests save a full-page PNG and trace ZIP under `TestResults/FailureArtifacts`. Structured JSON lifecycle events are written to NUnit output. CI uploads TRX reports and available failure diagnostics.

View a trace:

```powershell
.\bin\Release\net8.0\playwright.ps1 show-trace path\to\trace.zip
```

## Reliability notes

SauceDemo is a public demo service; hosting and availability remain outside this repository's control. The suite removes the previous application's API setup and HTTP-retry workarounds. It uses seeded accounts, isolated browser contexts, and sequential execution. Checkout verifies the demo application's simulated flow; no real payment service or customer data is used. Firefox, WebKit, parallel execution, and API testing are not configured.
