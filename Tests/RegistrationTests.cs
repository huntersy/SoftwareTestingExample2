using Microsoft.Playwright.NUnit;using NUnit.Framework;using SoftwareTestingExample.Pages;using SoftwareTestingExample.TestData;
namespace SoftwareTestingExample.Tests;
public class RegistrationTests:UiTest
{
 [Test] public async Task TC35_AuthenticatedUserCanLogout(){var l=new LoginPage(Page);await l.NavigateAsync();await l.LoginAsync(TestUsers.Standard,TestUsers.Password);var p=new ProductsPage(Page);await p.OpenMenuAsync();await p.LogoutAsync();Assert.That(Page.Url,Does.EndWith("/"));Assert.That(await l.IsUsernameVisibleAsync(),Is.True);}
}