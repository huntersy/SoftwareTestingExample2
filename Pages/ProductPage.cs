
using Microsoft.Playwright;

namespace SoftwareTestingExample.Pages;

public class ProductsPage
{
    private readonly IPage _page;

    // Search elements
    private ILocator SearchInput =>
        _page.Locator("#search_product");

    private ILocator SearchButton =>
        _page.Locator("#submit_search");

    // Search results
    private ILocator SearchedProductsHeading =>
        _page.GetByRole(
            AriaRole.Heading,
            new() { Name = "Searched Products" }
        );

    // Product cards
    private ILocator ProductCards =>
        _page.Locator(".product-image-wrapper");

    private ILocator ProductNames =>
        _page.Locator(".productinfo p");

    private ILocator ProductPrices =>
        _page.Locator(".productinfo h2");

    public ProductsPage(IPage page)
    {
        _page = page;
    }

    // Navigate to the Products page
    public async Task NavigateAsync()
    {
        await _page.GotoAsync(
            "https://automationexercise.com/products"
        );

        await SearchInput.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30000
        });
    }

    // Verify search field visibility
    public async Task<bool> IsSearchInputVisibleAsync()
    {
        return await SearchInput.IsVisibleAsync();
    }

    // Search using the magnifying-glass button
    public async Task SearchForProductAsync(string searchTerm)
    {
        await SearchInput.FillAsync(searchTerm);
        await SearchButton.ClickAsync();

        await SearchedProductsHeading.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });
    }

    // Retrieve product names
    public async Task<IReadOnlyList<string>> GetProductNamesAsync()
    {
        return await ProductNames.AllInnerTextsAsync();
    }

    // Count displayed product cards
    public async Task<int> GetProductCountAsync()
    {
        return await ProductCards.CountAsync();
    }

    // Retrieve product prices
    public async Task<IReadOnlyList<string>> GetProductPricesAsync()
    {
        return await ProductPrices.AllInnerTextsAsync();
    }

    // Verify searched products heading
    public async Task<bool> IsSearchedProductsHeadingVisibleAsync()
    {
        return await SearchedProductsHeading.IsVisibleAsync();
    }
}
