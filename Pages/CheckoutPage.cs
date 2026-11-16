using Microsoft.Playwright;

namespace SoftwareTestingExample.Pages;

public class CheckoutPage
{
    private readonly IPage _page;

    // Guest checkout prompt.
    private ILocator RegisterLoginPrompt =>
    _page.GetByRole(
        AriaRole.Link,
        new() { Name = "Register / Login" }
    );

    // Delivery and billing address sections.
    private ILocator DeliveryAddress =>
        _page.Locator("#address_delivery");

    private ILocator BillingAddress =>
        _page.Locator("#address_invoice");

    // Product rows displayed during checkout.
    private ILocator OrderRows =>
        _page.Locator("#cart_info tbody tr[id^='product-']");

    // Order comment.
    private ILocator OrderComment =>
        _page.Locator("textarea[name='message']");

    // Place order button.
    private ILocator PlaceOrderButton =>
        _page.Locator("a[href='/payment']");

    // Payment fields.
    private ILocator NameOnCard =>
        _page.Locator("[data-qa='name-on-card']");

    private ILocator CardNumber =>
        _page.Locator("[data-qa='card-number']");

    private ILocator Cvc =>
        _page.Locator("[data-qa='cvc']");

    private ILocator ExpiryMonth =>
        _page.Locator("[data-qa='expiry-month']");

    private ILocator ExpiryYear =>
        _page.Locator("[data-qa='expiry-year']");

    private ILocator PayButton =>
        _page.Locator("[data-qa='pay-button']");

    // Order confirmation.
    private ILocator OrderConfirmationMessage =>
    _page.GetByText(
        "Congratulations! Your order has been confirmed!",
        new() { Exact = false }
    );

    // Invoice download.
    private ILocator DownloadInvoiceButton =>
        _page.GetByRole(
            AriaRole.Link,
            new() { Name = "Download Invoice" }
        );

    public CheckoutPage(IPage page)
    {
        _page = page;
    }

    public async Task<bool> IsRegisterLoginPromptVisibleAsync() =>
        await RegisterLoginPrompt.IsVisibleAsync();

    public async Task WaitForCheckoutDetailsAsync()
    {
        await DeliveryAddress.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });

        await BillingAddress.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });

        await OrderRows.First.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });
    }

    public async Task<string> GetDeliveryAddressAsync()
    {
        await DeliveryAddress.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });

        return await DeliveryAddress.InnerTextAsync();
    }

    public async Task<string> GetBillingAddressAsync()
    {
        await BillingAddress.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });

        return await BillingAddress.InnerTextAsync();
    }

    private ILocator GetOrderProductRow(string productName) =>
        OrderRows.Filter(new()
        {
            HasText = productName
        });

    public async Task<IReadOnlyList<string>> GetOrderProductNamesAsync()
    {
        await OrderRows.First.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });

        return await OrderRows
            .Locator("td.cart_description h4 a")
            .AllInnerTextsAsync();
    }

    public async Task<string> GetOrderProductPriceAsync(
        string productName)
    {
        var row = GetOrderProductRow(productName);

        await row.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });

        return (await row.Locator("td.cart_price p")
            .InnerTextAsync()).Trim();
    }

    public async Task<string> GetOrderProductTotalAsync(
        string productName)
    {
        var row = GetOrderProductRow(productName);

        await row.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });

        return (await row.Locator("td.cart_total p")
            .InnerTextAsync()).Trim();
    }

    public async Task<int> GetOrderProductQuantityAsync(
        string productName)
    {
        var row = GetOrderProductRow(productName);

        var quantityText = (await row
            .Locator("td.cart_quantity button")
            .InnerTextAsync()).Trim();

        if (!int.TryParse(quantityText, out var quantity))
        {
            throw new InvalidOperationException(
                $"Unable to read checkout quantity for {productName}. " +
                $"Actual value: {quantityText}"
            );
        }

        return quantity;
    }

    public async Task ProceedToPaymentAsync(string comment)
    {
        await OrderComment.FillAsync(comment);

        await PlaceOrderButton.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });

        await PlaceOrderButton.ClickAsync();

        await NameOnCard.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });
    }

    public async Task PayAndConfirmOrderAsync(
        string nameOnCard,
        string cardNumber,
        string cvc,
        string expiryMonth,
        string expiryYear)
    {
        await NameOnCard.FillAsync(nameOnCard);
        await CardNumber.FillAsync(cardNumber);
        await Cvc.FillAsync(cvc);
        await ExpiryMonth.FillAsync(expiryMonth);
        await ExpiryYear.FillAsync(expiryYear);

        await PayButton.ClickAsync();

        await OrderConfirmationMessage.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30000
        });
    }

    public async Task<bool> IsOrderConfirmationVisibleAsync() =>
        await OrderConfirmationMessage.IsVisibleAsync();

    public async Task<string> DownloadInvoiceAsync()
    {
        var download = await _page.RunAndWaitForDownloadAsync(
            async () =>
            {
                await DownloadInvoiceButton.ClickAsync();
            }
        );

        var destinationPath = Path.Combine(
            Path.GetTempPath(),
            $"{Guid.NewGuid():N}-{download.SuggestedFilename}"
        );

        await download.SaveAsAsync(destinationPath);

        return destinationPath;
    }
}