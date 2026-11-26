
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using SoftwareTestingExample.Pages;

namespace SoftwareTestingExample.Tests;

public class LoginTests : UiTest
{
    private static string GenerateUniqueEmail() =>
        $"qa{Guid.NewGuid():N}@example.com";

    [Test]
    [Category("Smoke")]
    public async Task TC31_LoginPageDisplaysRequiredFields()
    {
        var loginPage = new LoginPage(Page);

        await loginPage.NavigateAsync();

        Assert.That(
            await loginPage.IsLoginHeadingVisibleAsync(),
            Is.True,
            "The login heading should be displayed."
        );

        Assert.That(
            await loginPage.IsLoginEmailVisibleAsync(),
            Is.True,
            "The login email field should be visible."
        );

        Assert.That(
            await loginPage.IsLoginPasswordVisibleAsync(),
            Is.True,
            "The login password field should be visible."
        );
    }

    [Test]
    public async Task TC32_InvalidLoginDisplaysErrorMessage()
    {
        var loginPage = new LoginPage(Page);

        await loginPage.NavigateAsync();

        await loginPage.LoginAsync(
            "invalid.qa.user@example.com",
            "IncorrectPassword123!"
        );

        Assert.That(
            await loginPage.IsInvalidLoginMessageVisibleAsync(),
            Is.True,
            "An invalid login should display the appropriate error message."
        );
    }

    [Test]
    public async Task TC34_RegisteredUserCanLogoutAndLoginAgain()
    {
        var registerPage = new RegisterPage(Page);
        var loginPage = new LoginPage(Page);

        var name = "QA Automation";
        var email = GenerateUniqueEmail();
        var password = "QaTest123!";

        // Create a unique test account.
        await registerPage.NavigateAsync();

        await registerPage.StartRegistrationAsync(
            name,
            email
        );

        await registerPage.CompleteRegistrationAsync(
            password
        );

        Assert.That(
            await registerPage.IsAccountCreatedAsync(),
            Is.True
        );

        await registerPage.ContinueAfterRegistrationAsync();

        Assert.That(
            await loginPage.IsLoggedInAsync(),
            Is.True,
            "The newly registered user should be logged in."
        );

        // Log out.
        await loginPage.LogoutAsync();

        // Log back in with the same credentials.
        await loginPage.LoginAsync(email, password);

        Assert.That(
            await loginPage.IsLoggedInAsync(),
            Is.True,
            "The registered user should be able to log in again."
        );

        // Clean up the unique account.
        await registerPage.DeleteAccountAsync();

        Assert.That(
            await registerPage.IsAccountDeletedAsync(),
            Is.True
        );
    }
}
