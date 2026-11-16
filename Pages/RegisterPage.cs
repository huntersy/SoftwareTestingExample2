
using Microsoft.Playwright;
using SoftwareTestingExample.Utilities;

namespace SoftwareTestingExample.Pages;

public class RegisterPage
{
    private readonly IPage _page;

    private ILocator SignupName =>
        _page.Locator("[data-qa='signup-name']");

    private ILocator SignupEmail =>
        _page.Locator("[data-qa='signup-email']");

    private ILocator SignupButton =>
    _page.Locator("[data-qa='signup-button']");

    private ILocator AccountInformationHeading =>
        _page.GetByText(
            "ENTER ACCOUNT INFORMATION",
            new() { Exact = false }
        );

    private ILocator AccountCreatedMessage =>
        _page.GetByText(
            "ACCOUNT CREATED!",
            new() { Exact = false }
        );

    private ILocator ContinueButton =>
    _page.Locator("[data-qa='continue-button']");

    private ILocator DeleteAccountLink =>
        _page.Locator("a[href='/delete_account']");

    private ILocator AccountDeletedMessage =>
        _page.GetByText(
            "ACCOUNT DELETED!",
            new() { Exact = false }
        );

    public RegisterPage(IPage page)
    {
        _page = page;
    }

    public async Task NavigateAsync()
    {
        await _page.GotoAsync(
            $"{TestConfig.BaseUrl.TrimEnd('/')}/login",
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 30000
            }
        );

        await SignupName.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });
    }

    public async Task StartRegistrationAsync(
        string name,
        string email)
    {
        await SignupName.FillAsync(name);
        await SignupEmail.FillAsync(email);
        await SignupButton.ClickAsync();

        await AccountInformationHeading.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });
    }

    public async Task CompleteRegistrationAsync(
        string password)
    {
        await _page.Locator("#id_gender1").CheckAsync();

        await _page.Locator("#password")
            .FillAsync(password);

        await _page.Locator("#days")
            .SelectOptionAsync("1");

        await _page.Locator("#months")
            .SelectOptionAsync("1");

        await _page.Locator("#years")
            .SelectOptionAsync("1990");

        await _page.Locator("#first_name")
            .FillAsync("QA");

        await _page.Locator("#last_name")
            .FillAsync("Automation");

        await _page.Locator("#company")
            .FillAsync("QA Testing");

        await _page.Locator("#address1")
            .FillAsync("100 Test Street");

        await _page.Locator("#address2")
            .FillAsync("Suite 200");

        await _page.Locator("#country")
            .SelectOptionAsync(
                new SelectOptionValue
                {
                    Label = "United States"
                }
            );

        await _page.Locator("#state")
            .FillAsync("Texas");

        await _page.Locator("#city")
            .FillAsync("Houston");

        await _page.Locator("#zipcode")
            .FillAsync("77048");

        await _page.Locator("#mobile_number")
            .FillAsync("5551234567");

        await _page.Locator(
            "[data-qa='create-account']"
        ).ClickAsync();

        await AccountCreatedMessage.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });
    }

    public async Task ContinueAfterRegistrationAsync()
    {
        await ContinueButton.ClickAsync();
    }

    public async Task DeleteAccountAsync()
    {
        await DeleteAccountLink.ClickAsync();

        await AccountDeletedMessage.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });
    }

    public async Task<bool> IsAccountCreatedAsync() =>
        await AccountCreatedMessage.IsVisibleAsync();

    public async Task<bool> IsAccountDeletedAsync() =>
        await AccountDeletedMessage.IsVisibleAsync();
}
