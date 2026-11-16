using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using SoftwareTestingExample.Pages;

namespace SoftwareTestingExample.Tests;

public class ProductDetailsTests : UiTest
{
    private async Task<ProductDetailsPage> OpenBlueTopDetailsAsync()
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

        await productDetailsPage.GetProductNameAsync();

        return productDetailsPage;
    }

    [Test]
    public async Task TC11_OpenProductFromSearchResults()
    {
        var productDetailsPage = await OpenBlueTopDetailsAsync();

        var productName =
            await productDetailsPage.GetProductNameAsync();

        Assert.That(
            productName,
            Is.EqualTo("Blue Top"),
            "The product details page should display the selected product name."
        );
    }

    // TC18: Verify the product category is displayed.
    [Test]
    public async Task TC18_ProductDetailsDisplayCategory()
    {
        var productDetailsPage = await OpenBlueTopDetailsAsync();

        var category =
            await productDetailsPage.GetProductCategoryAsync();

        Assert.That(
            category,
            Does.Contain("Category:"),
            "The product details page should display its category."
        );

        Assert.That(
            category,
            Does.Contain("Women"),
            "Blue Top should belong to the Women category."
        );
    }

    // TC19: Verify the product price is displayed.
    [Test]
    public async Task TC19_ProductDetailsDisplayPrice()
    {
        var productDetailsPage = await OpenBlueTopDetailsAsync();

        var price =
            await productDetailsPage.GetProductPriceAsync();

        Assert.That(
            price,
            Is.Not.Empty,
            "The product details page should display a price."
        );

        Assert.That(
            price,
            Does.Contain("Rs."),
            "The product price should display the expected currency prefix."
        );
    }

    // TC20: Verify availability, condition, and brand information.
    [Test]
    public async Task TC20_ProductDetailsDisplayInventoryAndBrandInformation()
    {
        var productDetailsPage = await OpenBlueTopDetailsAsync();

        var availability =
            await productDetailsPage.GetAvailabilityAsync();

        var condition =
            await productDetailsPage.GetConditionAsync();

        var brand =
            await productDetailsPage.GetBrandAsync();

        Assert.That(
            availability,
            Does.Contain("Availability:"),
            "Product availability information should be displayed."
        );

        Assert.That(
            availability,
            Does.Contain("In Stock"),
            "Blue Top should show its in-stock status."
        );

        Assert.That(
            condition,
            Does.Contain("Condition:"),
            "Product condition information should be displayed."
        );

        Assert.That(
            condition,
            Does.Contain("New"),
            "Blue Top should show its condition."
        );

        Assert.That(
            brand,
            Does.StartWith("Brand:"),
            "Product brand information should display the Brand label."
        );

        Assert.That(
            brand,
            Does.Match(@"^Brand:\s*\S+"),
            "Product brand information should contain an actual brand value."
        );
    }
}
