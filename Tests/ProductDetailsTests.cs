
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using SoftwareTestingExample.Pages;

namespace SoftwareTestingExample.Tests;

public class ProductDetailsTests : PageTest
{
    [Test]
    public async Task TC11_OpenProductFromSearchResults()
    {
        var productsPage = new ProductsPage(Page);
        var productDetailsPage = new ProductDetailsPage(Page);

        await productsPage.NavigateAsync();
        await productsPage.SearchForProductAsync("top");

        await Page.Locator(".product-image-wrapper")
            .Filter(new()
            {
                HasText = "Blue Top"
            })
            .Locator("a[href='/product_details/1']")
            .ClickAsync();

        var productName =
            await productDetailsPage.GetProductNameAsync();

        Assert.That(
            productName,
            Is.EqualTo("Blue Top"),
            "The product details page should display the selected product name."
        );
    }
}
