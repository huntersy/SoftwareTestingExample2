
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
        ProductInformation.Locator("> p").First;

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

    public async Task<string> GetProductNameAsync()
    {
        await ProductName.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });

        return (await ProductName.InnerTextAsync()).Trim();
    }

    public async Task<string> GetProductCategoryAsync() =>
        (await ProductCategory.InnerTextAsync()).Trim();

    public async Task<string> GetProductPriceAsync() =>
        (await ProductPrice.InnerTextAsync()).Trim();

    public async Task<string> GetAvailabilityAsync() =>
        (await Availability.InnerTextAsync()).Trim();

    public async Task<string> GetConditionAsync() =>
        (await Condition.InnerTextAsync()).Trim();

    public async Task<string> GetBrandAsync() =>
        (await Brand.InnerTextAsync()).Trim();
}
