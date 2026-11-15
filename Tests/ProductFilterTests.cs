
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using SoftwareTestingExample.Pages;

namespace SoftwareTestingExample.Tests;

[Parallelizable(ParallelScope.None)]
public class ProductFilterTests : PageTest
{
    [Test]
    public async Task TC12_WomenTopsCategoryDisplaysProducts()
    {
        var homePage = new HomePage(Page);

        await homePage.NavigateAsync();
        await homePage.SelectWomenTopsCategoryAsync();

        var currentUrl = Page.Url;

        var productCount =
            await Page.Locator(".product-image-wrapper").CountAsync();

        Assert.That(
            currentUrl,
            Does.Contain("/category_products/2"),
            "The browser should navigate to the Women Tops category."
        );

        Assert.That(
            productCount,
            Is.GreaterThan(0),
            "The Women Tops category should display products."
        );
    }
}
