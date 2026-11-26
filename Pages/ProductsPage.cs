
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

        await SafeNavigation.NavigateByGetAsync(
            _page,
            productsUrl,
            SearchInput,
            "Products page"
        );
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

        // The practice website can temporarily return HTTP 503 from
        // its add-to-cart endpoint. Retry only explicit transient server
        // responses; do not repeat a request that succeeded or whose
        // outcome is unknown.
        const int maxAttempts = 3;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            // Register the response wait before clicking so a fast response
            // cannot be missed.
            var addToCartResponseTask = _page.WaitForResponseAsync(
                response => response.Url.EndsWith(
                    $"/add_to_cart/{id}",
                    StringComparison.OrdinalIgnoreCase
                ),
                new PageWaitForResponseOptions
                {
                    Timeout = TestConfig.DefaultTimeoutMilliseconds,
                }
            );

            // The practice website's advertisements can intercept pointer
            // events, so click the visible product-information button.
            await addToCartButton.ClickAsync(new()
            {
                Force = true
            });

            var response = await addToCartResponseTask;

            if (response.Status == 429 || response.Status >= 500)
            {
                if (attempt == maxAttempts)
                {
                    throw new InvalidOperationException(
                        $"Add-to-cart request for product {id} returned HTTP " +
                        $"{response.Status} after {maxAttempts} attempts."
                    );
                }

                // Short bounded backoff gives the external service time to
                // recover without adding a large delay to normal test runs.
                await _page.WaitForTimeoutAsync(750 * attempt);
                continue;
            }

            if (response.Status < 200 || response.Status >= 300)
            {
                throw new InvalidOperationException(
                    $"Add-to-cart request for product {id} failed with HTTP " +
                    $"{response.Status}."
                );
            }

            await AddToCartModal.WaitForAsync(new()
            {
                State = WaitForSelectorState.Visible,
            });

            return;
        }
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

        var cartUrl =
            $"{TestConfig.BaseUrl.TrimEnd('/')}/view_cart";

        // This is a safe GET navigation; retry only explicit transient
        // HTTP responses. The original click is never replayed.
        await SafeNavigation.NavigateByClickAsync(
            _page,
            ModalViewCartLink,
            cartUrl,
            _page.Locator("#cart_info_table"),
            "Cart navigation from add-to-cart confirmation"
        );
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
