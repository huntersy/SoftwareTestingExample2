
using Microsoft.Playwright;
using SoftwareTestingExample.Utilities;

namespace SoftwareTestingExample.Pages;

public class LoginPage
{
    private readonly IPage _page;

    private ILocator LoginHeading =>
        _page.GetByRole(
            AriaRole.Heading,
            new() { Name = "Login to your account" }
        );

    private ILocator LoginEmail =>
        _page.Locator("[data-qa='login-email']");

    private ILocator LoginPassword =>
        _page.Locator("[data-qa='login-password']");

    private ILocator LoginButton =>
        _page.Locator("[data-qa='login']");

    private ILocator InvalidLoginMessage =>
        _page.GetByText(
            "Your email or password is incorrect!",
            new() { Exact = true }
        );

    private ILocator LoggedInAs =>
        _page.Locator("a").Filter(
            new() { HasText = "Logged in as" }
        );

    private ILocator LogoutLink =>
        _page.Locator("a[href='/logout']");

    public LoginPage(IPage page)
    {
        _page = page;
    }

    public async Task NavigateAsync()
    {
        var loginUrl =
            $"{TestConfig.BaseUrl.TrimEnd('/')}/login";

        await _page.GotoAsync(
            loginUrl,
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 30000
            }
        );

        await LoginHeading.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });
    }

    public async Task<bool> IsLoginHeadingVisibleAsync() =>
        await LoginHeading.IsVisibleAsync();

    public async Task<bool> IsLoginEmailVisibleAsync() =>
        await LoginEmail.IsVisibleAsync();

    public async Task<bool> IsLoginPasswordVisibleAsync() =>
        await LoginPassword.IsVisibleAsync();

    public async Task LoginAsync(
        string email,
        string password)
    {
        await LoginEmail.FillAsync(email);
        await LoginPassword.FillAsync(password);
        await LoginButton.ClickAsync();
    }

    public async Task<bool> IsInvalidLoginMessageVisibleAsync() =>
        await InvalidLoginMessage.IsVisibleAsync();

    public async Task<bool> IsLoggedInAsync() =>
        await LoggedInAs.IsVisibleAsync();

    public async Task LogoutAsync()
    {
        await LogoutLink.ClickAsync();

        await LoginHeading.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });
    }
}
