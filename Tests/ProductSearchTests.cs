using Microsoft.Playwright.NUnit;using NUnit.Framework;using SoftwareTestingExample.Pages;using SoftwareTestingExample.TestData;
namespace SoftwareTestingExample.Tests;
public class ProductSearchTests:UiTest
{
 async Task<ProductsPage> Open(){var l=new LoginPage(Page);await l.NavigateAsync();await l.LoginAsync(TestUsers.Standard,TestUsers.Password);var p=new ProductsPage(Page);await p.IsInventoryVisibleAsync();return p;}
 [Test,Category("Smoke")] public async Task TC06_BackpackIsListed(){Assert.That(await (await Open()).NamesAsync(),Does.Contain(TestUsers.Backpack));}
 [Test] public async Task TC07_BikeLightIsListed(){Assert.That(await (await Open()).NamesAsync(),Does.Contain(TestUsers.BikeLight));}
 [Test] public async Task TC08_BoltTShirtIsListed(){Assert.That(await (await Open()).NamesAsync(),Does.Contain("Sauce Labs Bolt T-Shirt"));}
 [Test] public async Task TC09_AllProductsHavePrices(){Assert.That(await Page.Locator(".inventory_item_price").CountAsync(),Is.EqualTo(0));var p=await Open();Assert.That(await p.PricesAsync(),Has.Count.EqualTo(6));}
 [Test] public async Task TC10_ProductNamesAreUnique(){var n=await (await Open()).NamesAsync();Assert.That(n.Distinct().Count(),Is.EqualTo(n.Count));}
 [Test] public async Task TC11_ProductDescriptionsAreDisplayed(){await Open();Assert.That(await Page.Locator(".inventory_item_desc").CountAsync(),Is.EqualTo(6));}
 [Test] public async Task TC12_ProductImagesAreDisplayed(){await Open();Assert.That(await Page.Locator(".inventory_item_img img").CountAsync(),Is.EqualTo(6));}
 [Test] public async Task TC13_AddToCartButtonsAreAvailable(){await Open();Assert.That(await Page.Locator("button[data-test^='add-to-cart-']").CountAsync(),Is.EqualTo(6));}
}