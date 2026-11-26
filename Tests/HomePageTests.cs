using System;
using System.Linq;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using SoftwareTestingExample.Pages;
using SoftwareTestingExample.Utilities;

namespace SoftwareTestingExample.Tests;

public class HomePageTests : UiTest
{
    [Test]
    [Category("Smoke")]
    public async Task Homepage_ShouldLoadSuccessfully()
    {
        var homePage = new HomePage(Page);

        await homePage.NavigateAsync();

        var title = await homePage.GetPageTitleAsync();

        Assert.That(
            title,
            Is.Not.Empty,
            "Homepage should have a page title."
        );
    }

    [Test]
    public async Task TC02_ProductsPageSearchBoxIsVisible()
    {
        var productsPage = new ProductsPage(Page);

        await productsPage.NavigateAsync();

        Assert.That(
            await productsPage.IsSearchInputVisibleAsync(),
            Is.True,
            "Search box should be visible on the Products page."
        );
    }

    [Test]
    public async Task TC03_WomenCategoryExpandsSuccessfully()
    {
        var homePage = new HomePage(Page);

        await homePage.NavigateAsync();
        await homePage.ExpandWomenCategoryAsync();

        var subcategories = await homePage.GetWomenSubcategoryNamesAsync();

        Assert.That(
            subcategories.Any(name =>
                string.Equals(name.Trim(), "Dress", StringComparison.OrdinalIgnoreCase)),
            Is.True,
            "The Women category should contain Dress."
        );

        Assert.That(
            subcategories.Any(name =>
                string.Equals(name.Trim(), "Tops", StringComparison.OrdinalIgnoreCase)),
            Is.True,
            "The Women category should contain Tops."
        );

        Assert.That(
            subcategories.Any(name =>
                string.Equals(name.Trim(), "Saree", StringComparison.OrdinalIgnoreCase)),
            Is.True,
            "The Women category should contain Saree."
        );

        Assert.That(subcategories.Count, Is.EqualTo(3));
    }

    [Test]
    public async Task TC04_HomepageUrlIsCorrect()
    {
        var homePage = new HomePage(Page);

        await homePage.NavigateAsync();

        var expectedUrl = new Uri(TestConfig.BaseUrl).AbsoluteUri;
        var actualUrl = new Uri(
            await homePage.GetCurrentUrlAsync()
        ).AbsoluteUri;

        Assert.That(
            actualUrl,
            Is.EqualTo(expectedUrl),
            "Homepage URL should match the configured BASE_URL."
        );
    }

    [Test]
    public async Task TC05_HomepageDisplaysProducts()
    {
        var homePage = new HomePage(Page);

        await homePage.NavigateAsync();

        var productCount =
            await homePage.GetProductCardCountAsync();

        Assert.That(
            productCount,
            Is.GreaterThan(0),
            "Homepage should display at least one product."
        );
    }
}
