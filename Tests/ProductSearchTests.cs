
using System;
using System.Linq;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using SoftwareTestingExample.Pages;

namespace SoftwareTestingExample.Tests;

public class ProductSearchTests : UiTest
{
    [Test]
    [Category("Smoke")]
    public async Task TC06_SearchReturnsMatchingProducts()
    {
        var productsPage = new ProductsPage(Page);

        await productsPage.NavigateAsync();
        await productsPage.SearchForProductAsync("top");

        var productNames = await productsPage.GetProductNamesAsync();

        Assert.That(productNames.Count, Is.GreaterThan(0));

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

        Assert.That(productNames.Count, Is.GreaterThan(0));

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

        Assert.That(productNames.Count, Is.GreaterThan(0));

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
            "Product cards should match the number of displayed product names."
        );
    }

    // TC13: Verify the search results heading appears.
    [Test]
    public async Task TC13_SearchResultsHeadingIsDisplayed()
    {
        var productsPage = new ProductsPage(Page);

        await productsPage.NavigateAsync();
        await productsPage.SearchForProductAsync("top");

        Assert.That(
            await productsPage.IsSearchedProductsHeadingVisibleAsync(),
            Is.True,
            "The Searched Products heading should appear after a search."
        );
    }

    // TC14: Verify search works with uppercase input.
    [Test]
    public async Task TC14_SearchReturnsMatchingProductsForUppercaseInput()
    {
        var productsPage = new ProductsPage(Page);

        await productsPage.NavigateAsync();
        await productsPage.SearchForProductAsync("TOP");

        var productNames = await productsPage.GetProductNamesAsync();

        Assert.That(
            productNames.Count,
            Is.GreaterThan(0),
            "An uppercase search should return matching products."
        );

        Assert.That(
            productNames.Any(name =>
                name.Contains("top", StringComparison.OrdinalIgnoreCase)),
            Is.True,
            "At least one result should match TOP regardless of letter case."
        );
    }

    // TC15: Verify searching for dress returns relevant products.
    [Test]
    public async Task TC15_SearchForDressReturnsRelevantProducts()
    {
        var productsPage = new ProductsPage(Page);

        await productsPage.NavigateAsync();
        await productsPage.SearchForProductAsync("dress");

        var productNames = await productsPage.GetProductNamesAsync();

        Assert.That(
            productNames.Count,
            Is.GreaterThan(0),
            "The dress search should return products."
        );

        Assert.That(
            productNames.Any(name =>
                name.Contains("dress", StringComparison.OrdinalIgnoreCase)),
            Is.True,
            $"At least one result should contain 'dress' in its name. " +
            $"Actual product names: {string.Join(", ", productNames)}"
        );
    }

    // TC16: Verify searching for saree returns matching products.
    [Test]
    public async Task TC16_SearchReturnsSareeProducts()
    {
        var productsPage = new ProductsPage(Page);

        await productsPage.NavigateAsync();
        await productsPage.SearchForProductAsync("saree");

        var productNames = await productsPage.GetProductNamesAsync();

        Assert.That(
            productNames.Count,
            Is.GreaterThan(0),
            "Searching for saree should return products."
        );

        Assert.That(
            productNames.All(name =>
                name.Contains("saree", StringComparison.OrdinalIgnoreCase)),
            Is.True,
            "Every displayed result should contain saree."
        );
    }

    // TC17: Verify displayed product prices use the expected currency format.
    [Test]
    public async Task TC17_SearchResultsDisplayPrices()
    {
        var productsPage = new ProductsPage(Page);

        await productsPage.NavigateAsync();
        await productsPage.SearchForProductAsync("top");

        var prices = await productsPage.GetProductPricesAsync();

        Assert.That(
            prices.Count,
            Is.GreaterThan(0),
            "Search results should display product prices."
        );

        Assert.That(
            prices.All(price => price.Trim().StartsWith("Rs.", StringComparison.OrdinalIgnoreCase)),
            Is.True,
            "Every displayed product price should use the Rs. currency prefix."
        );
    }


    // TC21: Verify an empty search term is rejected.
    [Test]
    public async Task TC21_EmptySearchTermIsRejected()
    {
        var productsPage = new ProductsPage(Page);

        await productsPage.NavigateAsync();

        Assert.ThrowsAsync<ArgumentException>(
            async () => await productsPage.SearchForProductAsync("")
        );
    }

    // TC22: Verify a whitespace-only search term is rejected.
    [Test]
    public async Task TC22_WhitespaceSearchTermIsRejected()
    {
        var productsPage = new ProductsPage(Page);

        await productsPage.NavigateAsync();

        Assert.ThrowsAsync<ArgumentException>(
            async () => await productsPage.SearchForProductAsync("   ")
        );
    }

}
