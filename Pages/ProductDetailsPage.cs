
using Microsoft.Playwright;

namespace SoftwareTestingExample.Pages;

public class ProductDetailsPage
{
    private readonly IPage _page;

    // Product information container
    private ILocator ProductInformation =>
        _page.Locator(".product-information");

    // Product detail locators
    private ILocator ProductName =>
        ProductInformation.Locator("h2");

    private ILocator ProductCategory =>
        ProductInformation.Locator("p")
            .Filter(new() { HasText = "Category:" });

    private ILocator ProductPrice =>
        ProductInformation.Locator("span span").First;

    private ILocator Availability =>
        ProductInformation.Locator("p")
            .Filter(new() { HasText = "Availability:" });

    private ILocator Condition =>
        ProductInformation.Locator("p")
            .Filter(new() { HasText = "Condition:" });

    private ILocator Brand =>
        ProductInformation.Locator("p")
            .Filter(new() { HasText = "Brand:" });

    public ProductDetailsPage(IPage page)
    {
        _page = page;
    }

    private static async Task<string> GetVisibleTextAsync(
        ILocator locator)
    {
        await locator.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });

        return (await locator.InnerTextAsync()).Trim();
    }

    public async Task<string> GetProductNameAsync() =>
        await GetVisibleTextAsync(ProductName);

    public async Task<string> GetProductCategoryAsync() =>
        await GetVisibleTextAsync(ProductCategory);

    public async Task<string> GetProductPriceAsync() =>
        await GetVisibleTextAsync(ProductPrice);

    public async Task<string> GetAvailabilityAsync() =>
        await GetVisibleTextAsync(Availability);

    public async Task<string> GetConditionAsync() =>
        await GetVisibleTextAsync(Condition);

    public async Task<string> GetBrandAsync() =>
        await GetVisibleTextAsync(Brand);
}
