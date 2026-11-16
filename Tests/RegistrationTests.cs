
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using SoftwareTestingExample.Pages;

namespace SoftwareTestingExample.Tests;

public class RegistrationTests : UiTest
{
    private static string GenerateUniqueEmail() =>
        $"qa{Guid.NewGuid():N}@example.com";

    [Test]
    public async Task TC33_NewUserCanRegisterSuccessfully()
    {
        var registerPage = new RegisterPage(Page);

        var name = "QA Automation";
        var email = GenerateUniqueEmail();
        var password = "QaTest123!";

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
            Is.True,
            "The account creation confirmation should be displayed."
        );

        await registerPage.ContinueAfterRegistrationAsync();

        var loginPage = new LoginPage(Page);

        Assert.That(
            await loginPage.IsLoggedInAsync(),
            Is.True,
            "The newly registered user should be logged in."
        );

        await registerPage.DeleteAccountAsync();

        Assert.That(
            await registerPage.IsAccountDeletedAsync(),
            Is.True,
            "The test account should be deleted after registration testing."
        );
    }
}
