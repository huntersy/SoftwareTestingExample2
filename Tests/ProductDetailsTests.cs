
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using SoftwareTestingExample.Pages;

namespace SoftwareTestingExample.Tests;

public class ProductDetailsTests : PageTest
{
    [Test]
    public async Task TC11_OpenProductFromSearchResults()
    {
        // Arrange
        var productsPage = new ProductsPage(Page);
        var productDetailsPage = new ProductDetailsPage(Page);

        // Act: Navigate to the Products page and search for a product
        await productsPage.NavigateAsync();
        await productsPage.SearchForProductAsync("top");

        // Select the Blue Top product
        await Page.Locator(
            ".product-image-wrapper"
        ).Filter(new()
        {
            Has = Page.Locator(".productinfo p")
                .GetByText("Blue Top", new() { Exact = true })
        })
        .Locator("a[href='/product_details/1']")
        .ClickAsync();

        // Assert: Verify the product details page displays the product
        var productName =
            await productDetailsPage.GetProductNameAsync();

        Assert.That(
            productName,
            Is.EqualTo("Blue Top"),
            "The product details page should display the selected product name."
        );
    }
}
