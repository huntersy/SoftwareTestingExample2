
using Microsoft.Playwright;

namespace SoftwareTestingExample.Pages;

public class ProductDetailsPage
{
    private readonly IPage _page;

    private ILocator ProductInformation =>
        _page.Locator(".product-information");

    private ILocator ProductName =>
        ProductInformation.Locator("h2");

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
}
