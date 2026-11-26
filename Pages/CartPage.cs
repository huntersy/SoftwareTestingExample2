
using Microsoft.Playwright;
using SoftwareTestingExample.Utilities;

namespace SoftwareTestingExample.Pages;

public class CartPage
{
    private readonly IPage _page;

    // Cart product rows.
    private ILocator CartRows =>
        _page.Locator("#cart_info_table tbody tr[id^='product-']");

    // Empty cart message.
    private ILocator EmptyCartMessage =>
        _page.GetByText(
            "Cart is empty! Click here to buy products.",
            new() { Exact = false }
        );

    public CartPage(IPage page)
    {
        _page = page;
    }

    private ILocator GetProductRow(string productName) =>
        CartRows.Filter(new()
        {
            HasText = productName
        });

    public async Task ProceedToCheckoutAsync()
    {
        await ProceedToCheckoutButton.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
        });

        // Guests are shown the site's registration/login prompt instead
        // of navigating to checkout. Preserve that existing UI behavior.
        var logoutLink = _page.Locator("a[href='/logout']");
        if (!await logoutLink.IsVisibleAsync())
        {
            await ProceedToCheckoutButton.ClickAsync();
            return;
        }

        var checkoutUrl =
            $"{TestConfig.BaseUrl.TrimEnd('/')}/checkout";

        await SafeNavigation.NavigateByClickAsync(
            _page,
            ProceedToCheckoutButton,
            checkoutUrl,
            _page.Locator("#address_delivery"),
            "Registered customer checkout navigation"
        );
    }

    private ILocator ProceedToCheckoutButton =>
    _page.Locator("a.check_out");

    public async Task WaitForCartItemsAsync()
    {
        await CartRows.First.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
        });
    }

    public async Task<IReadOnlyList<string>> GetProductNamesAsync()
    {
        await WaitForCartItemsAsync();

        return await CartRows
            .Locator("td.cart_description h4 a")
            .AllInnerTextsAsync();
    }

    public async Task<int> GetCartItemCountAsync() =>
        await CartRows.CountAsync();

    public async Task<string> GetProductPriceByNameAsync(
        string productName)
    {
        var row = GetProductRow(productName);

        await row.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
        });

        return (await row.Locator("td.cart_price p")
            .InnerTextAsync()).Trim();
    }

    public async Task<string> GetProductTotalByNameAsync(
        string productName)
    {
        var row = GetProductRow(productName);

        return (await row.Locator("td.cart_total p")
            .InnerTextAsync()).Trim();
    }

    public async Task<int> GetProductQuantityByNameAsync(
        string productName)
    {
        var row = GetProductRow(productName);

        var quantityText = (await row
            .Locator("td.cart_quantity button")
            .InnerTextAsync()).Trim();

        if (!int.TryParse(quantityText, out var quantity))
        {
            throw new InvalidOperationException(
                $"Unable to read cart quantity for {productName}. " +
                $"Actual value: {quantityText}"
            );
        }

        return quantity;
    }

    public async Task RemoveProductByNameAsync(
        string productName)
    {
        var row = GetProductRow(productName);

        await row.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
        });

        await row.Locator("a.cart_quantity_delete")
            .ClickAsync();

        await row.WaitForAsync(new()
        {
            State = WaitForSelectorState.Detached,
        });
    }

    public async Task<bool> IsEmptyCartMessageVisibleAsync() =>
        await EmptyCartMessage.IsVisibleAsync();
}
