using System.Globalization;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using SoftwareTestingExample.Pages;
using SoftwareTestingExample.TestData;

namespace SoftwareTestingExample.Tests;

public class ProductFilterTests : UiTest
{
    private async Task<ProductsPage> OpenInventoryAsync()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync(TestUsers.Standard, TestUsers.Password);

        var productsPage = new ProductsPage(Page);
        await productsPage.WaitForInventoryAsync();
        return productsPage;
    }

    [Test, Category("Smoke")]
    public async Task TC18_SortNamesAscending()
    {
        var productsPage = await OpenInventoryAsync();
        await productsPage.SortAsync("az");

        var actualNames = await productsPage.NamesAsync();
        var expectedNames = actualNames.OrderBy(name => name, StringComparer.Ordinal).ToArray();

        Assert.That(actualNames, Is.EqualTo(expectedNames),
            "Product names should be sorted alphabetically from A to Z.");
    }

    [Test]
    public async Task TC19_SortNamesDescending()
    {
        var productsPage = await OpenInventoryAsync();
        await productsPage.SortAsync("za");

        var actualNames = await productsPage.NamesAsync();
        var expectedNames = actualNames.OrderByDescending(name => name, StringComparer.Ordinal).ToArray();

        Assert.That(actualNames, Is.EqualTo(expectedNames),
            "Product names should be sorted alphabetically from Z to A.");
    }

    [Test]
    public async Task TC20_SortPricesAscending()
    {
        var productsPage = await OpenInventoryAsync();
        await productsPage.SortAsync("lohi");

        var actualPrices = (await productsPage.PricesAsync())
            .Select(price => decimal.Parse(price.TrimStart('$'), CultureInfo.InvariantCulture))
            .ToArray();

        Assert.That(actualPrices, Is.Ordered.Ascending,
            "Product prices should be sorted from lowest to highest.");
    }

    [Test]
    public async Task TC21_SortPricesDescending()
    {
        var productsPage = await OpenInventoryAsync();
        await productsPage.SortAsync("hilo");

        var actualPrices = (await productsPage.PricesAsync())
            .Select(price => decimal.Parse(price.TrimStart('$'), CultureInfo.InvariantCulture))
            .ToArray();

        Assert.That(actualPrices, Is.Ordered.Descending,
            "Product prices should be sorted from highest to lowest.");
    }
}
