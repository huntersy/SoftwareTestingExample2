using Microsoft.Playwright.NUnit;using NUnit.Framework;using SoftwareTestingExample.Pages;using SoftwareTestingExample.TestData;
namespace SoftwareTestingExample.Tests;
public class LoginTests:UiTest
{
 async Task<LoginPage> Open(){var p=new LoginPage(Page);await p.NavigateAsync();return p;}
 [Test,Category("Smoke")] public async Task TC30_ValidCredentialsLogin(){await (await Open()).LoginAsync(TestUsers.Standard,TestUsers.Password);Assert.That(Page.Url,Does.EndWith("/inventory.html"));}
 [Test] public async Task TC31_LockedOutAccountIsRejected(){var p=await Open();await p.LoginAsync(TestUsers.LockedOut,TestUsers.Password);Assert.That(await p.GetErrorAsync(),Does.Contain("locked out"));}
 [Test] public async Task TC32_InvalidUsernameShowsError(){var p=await Open();await p.LoginAsync("bad_user",TestUsers.Password);Assert.That(await p.IsErrorVisibleAsync(),Is.True);}
 [Test] public async Task TC33_InvalidPasswordShowsError(){var p=await Open();await p.LoginAsync(TestUsers.Standard,"bad_password");Assert.That(await p.IsErrorVisibleAsync(),Is.True);}
 [Test] public async Task TC34_EmptyCredentialsAreRejected(){var p=await Open();await p.LoginAsync("","");Assert.That(await p.IsErrorVisibleAsync(),Is.True);}
}