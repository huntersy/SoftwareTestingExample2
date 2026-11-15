
using Microsoft.Playwright;
using SoftwareTestingExample.Utilities;

namespace SoftwareTestingExample.Pages;

public class ProductsPage
{
    private readonly IPage _page;

    private ILocator SearchInput =>
        _page.Locator("#search_product");

    private ILocator SearchButton =>
        _page.Locator("#submit_search");

    private ILocator SearchedProductsHeading =>
        _page.GetByRole(
            AriaRole.Heading,
            new() { Name = "Searched Products" }
        );

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

    public async Task NavigateAsync()
    {
        var productsUrl =
            $"{TestConfig.BaseUrl.TrimEnd('/')}/products";

        var response = await _page.GotoAsync(
            productsUrl,
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 30000
            }
        );

        if (response is null ||
            response.Status < 200 ||
            response.Status >= 400)
        {
            throw new InvalidOperationException(
                $"Products page navigation failed. HTTP status: {response?.Status}"
            );
        }

        await SearchInput.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30000
        });
    }

    public async Task<bool> IsSearchInputVisibleAsync() =>
        await SearchInput.IsVisibleAsync();

    public async Task SearchForProductAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            throw new ArgumentException(
                "Search term cannot be empty or whitespace.",
                nameof(searchTerm)
            );
        }

        await SearchInput.FillAsync(searchTerm.Trim());
        await SearchButton.ClickAsync();

        await SearchedProductsHeading.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });
    }

    public async Task<IReadOnlyList<string>> GetProductNamesAsync()
    {
        await SearchedProductsHeading.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });

        return await ProductNames.AllInnerTextsAsync();
    }

    public async Task<int> GetProductCountAsync()
    {
        await SearchedProductsHeading.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });

        return await ProductCards.CountAsync();
    }

    public async Task<IReadOnlyList<string>> GetProductPricesAsync()
    {
        await SearchedProductsHeading.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });

        return await ProductPrices.AllInnerTextsAsync();
    }

    public async Task<bool> IsSearchedProductsHeadingVisibleAsync() =>
        await SearchedProductsHeading.IsVisibleAsync();
}
