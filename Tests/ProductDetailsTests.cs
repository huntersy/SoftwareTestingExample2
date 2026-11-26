using Microsoft.Playwright.NUnit;using NUnit.Framework;using SoftwareTestingExample.Pages;using SoftwareTestingExample.TestData;
namespace SoftwareTestingExample.Tests;
public class ProductDetailsTests:UiTest
{
 async Task<ProductDetailsPage> Open(){var l=new LoginPage(Page);await l.NavigateAsync();await l.LoginAsync(TestUsers.Standard,TestUsers.Password);await new ProductsPage(Page).OpenDetailsAsync(TestUsers.Backpack);return new ProductDetailsPage(Page);}
 [Test,Category("Smoke")] public async Task TC14_DetailsShowSelectedProductName(){Assert.That(await (await Open()).GetNameAsync(),Is.EqualTo(TestUsers.Backpack));}
 [Test] public async Task TC15_DetailsShowDescription(){Assert.That(await (await Open()).GetDescriptionAsync(),Is.Not.Empty);}
 [Test] public async Task TC16_DetailsShowPrice(){Assert.That(await (await Open()).GetPriceAsync(),Is.EqualTo("$29.99"));}
 [Test] public async Task TC17_BackButtonReturnsToInventory(){var p=await Open();Assert.That(await p.BackVisibleAsync(),Is.True);await p.BackAsync();Assert.That(Page.Url,Does.EndWith("/inventory.html"));}
}