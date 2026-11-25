
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

    // Product cards displayed in the main product listing.
    private ILocator ProductCards =>
        _page.Locator(".features_items .product-image-wrapper");

    // Product names displayed in the main product listing.
    private ILocator ProductNames =>
        _page.Locator(".features_items .productinfo p");

    // Product prices displayed in the main product listing.
    private ILocator ProductPrices =>
        _page.Locator(".features_items .productinfo h2");

    // Add-to-cart confirmation modal.
    private ILocator AddToCartModal =>
        _page.Locator("#cartModal");

    private ILocator ModalViewCartLink =>
        AddToCartModal.Locator("a[href='/view_cart']");

    private ILocator ContinueShoppingButton =>
        AddToCartModal.Locator("button.close-modal");

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
        });
    }

    public async Task<IReadOnlyList<string>> GetProductNamesAsync()
    {
        await SearchedProductsHeading.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
        });

        return await ProductNames.AllInnerTextsAsync();
    }

    public async Task<int> GetProductCountAsync()
    {
        await SearchedProductsHeading.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
        });

        return await ProductCards.CountAsync();
    }

    public async Task<IReadOnlyList<string>> GetProductPricesAsync()
    {
        await SearchedProductsHeading.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
        });

        return await ProductPrices.AllInnerTextsAsync();
    }

    public async Task<bool> IsSearchedProductsHeadingVisibleAsync() =>
        await SearchedProductsHeading.IsVisibleAsync();


    public async Task AddProductToCartAsync(string productId)
    {
        if (!int.TryParse(productId, out var id) || id <= 0)
        {
            throw new ArgumentException(
                "Product ID must be a positive number.",
                nameof(productId)
            );
        }

        var productCard = _page.Locator(
            $".features_items .product-image-wrapper:has(a[data-product-id='{id}'])"
        );

        await productCard.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
        });

        // Use the regular product-information button instead of
        // the animated hover overlay.
        var addToCartButton = productCard.Locator(
            $".productinfo a.add-to-cart[data-product-id='{id}']"
        );

        await addToCartButton.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
        });

        await addToCartButton.ScrollIntoViewIfNeededAsync();

        // The practice website's third-party advertisements can
        // intercept pointer events. Force the click on the visible
        // product-information button rather than the hover overlay.
        await addToCartButton.ClickAsync(new()
        {
            Force = true
        });

        await AddToCartModal.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
        });
    }


    public async Task<bool> IsAddToCartModalVisibleAsync() =>
        await AddToCartModal.IsVisibleAsync();


    public async Task OpenCartFromAddToCartModalAsync()
    {
        await ModalViewCartLink.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
        });

        await ModalViewCartLink.ScrollIntoViewIfNeededAsync();

        // The practice website's advertisements can intercept
        // pointer events on the modal's View Cart link.
        await ModalViewCartLink.ClickAsync(new()
        {
            Force = true
        });

        await _page.Locator("#cart_info_table").WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
        });
    }


    public async Task ContinueShoppingAsync()
    {
        await ContinueShoppingButton.ClickAsync();

        await AddToCartModal.WaitForAsync(new()
        {
            State = WaitForSelectorState.Hidden,
        });
    }


    public async Task OpenProductDetailsAsync(string productId)
    {
        if (!int.TryParse(productId, out var id) || id <= 0)
        {
            throw new ArgumentException(
                "Product ID must be a positive number.",
                nameof(productId)
            );
        }

        var productLink = _page.Locator(
            $".features_items a[href='/product_details/{id}']"
        ).First;

        await productLink.ClickAsync();
    }

}
