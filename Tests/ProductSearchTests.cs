
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using SoftwareTestingExample.Pages;

namespace SoftwareTestingExample.Tests;

public class ProductSearchTests : PageTest
{
    [Test]
    public async Task TC06_SearchReturnsMatchingProducts()
    {
        var productsPage = new ProductsPage(Page);

        await productsPage.NavigateAsync();
        await productsPage.SearchForProductAsync("top");

        var productNames = await productsPage.GetProductNamesAsync();

        Assert.That(
            productNames.Count,
            Is.GreaterThan(0),
            "Searching for top should return at least one product."
        );

        Assert.That(
            productNames.Any(name =>
                name.Contains("top", StringComparison.OrdinalIgnoreCase)),
            Is.True,
            "At least one returned product should contain the search term."
        );
    }

    [Test]
    public async Task TC07_SearchReturnsNoMatchingProducts()
    {
        var productsPage = new ProductsPage(Page);

        await productsPage.NavigateAsync();
        await productsPage.SearchForProductAsync("XYZ123NONEXISTENT");

        var productCount = await productsPage.GetProductCountAsync();

        Assert.That(
            productCount,
            Is.EqualTo(0),
            "An unmatched search should return no product cards."
        );
    }

    [Test]
    public async Task TC08_SearchReturnsProductsUsingPartialName()
    {
        var productsPage = new ProductsPage(Page);

        await productsPage.NavigateAsync();
        await productsPage.SearchForProductAsync("blu");

        var productNames = await productsPage.GetProductNamesAsync();

        Assert.That(
            productNames.Count,
            Is.GreaterThan(0),
            "A partial search for blu should return matching products."
        );

        Assert.That(
            productNames.Any(name =>
                name.Contains("blu", StringComparison.OrdinalIgnoreCase)),
            Is.True,
            "At least one result should contain the partial search term."
        );
    }

    [Test]
    public async Task TC09_SearchReturnsDressProducts()
    {
        var productsPage = new ProductsPage(Page);

        await productsPage.NavigateAsync();
        await productsPage.SearchForProductAsync("dress");

        var productNames = await productsPage.GetProductNamesAsync();

        Assert.That(
            productNames.Count,
            Is.GreaterThan(0),
            "Searching for dress should return matching products."
        );

        Assert.That(
            productNames.Any(name =>
                name.Contains("dress", StringComparison.OrdinalIgnoreCase)),
            Is.True,
            "Search results should contain a dress product."
        );
    }

    [Test]
    public async Task TC10_SearchResultsCountMatchesDisplayedProducts()
    {
        var productsPage = new ProductsPage(Page);

        await productsPage.NavigateAsync();
        await productsPage.SearchForProductAsync("top");

        var productNames = await productsPage.GetProductNamesAsync();
        var productCount = await productsPage.GetProductCountAsync();

        Assert.That(
            productCount,
            Is.EqualTo(productNames.Count),
            "The number of product cards should match the number of displayed product names."
        );
    }
}
