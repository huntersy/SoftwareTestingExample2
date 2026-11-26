using Microsoft.Playwright.NUnit;using NUnit.Framework;using SoftwareTestingExample.Pages;using SoftwareTestingExample.TestData;
namespace SoftwareTestingExample.Tests;
public class HomePageTests:UiTest
{
 async Task<ProductsPage> Open(){var l=new LoginPage(Page);await l.NavigateAsync();await l.LoginAsync(TestUsers.Standard,TestUsers.Password);var p=new ProductsPage(Page);await p.IsInventoryVisibleAsync();return p;}
 [Test,Category("Smoke")] public async Task TC01_StandardUserCanOpenInventory(){var p=await Open();Assert.That(await p.IsInventoryVisibleAsync(),Is.True);}
 [Test] public async Task TC02_InventoryDisplaysProductsHeading(){Assert.That(await (await Open()).GetTitleAsync(),Is.EqualTo("Products"));}
 [Test] public async Task TC03_InventoryDisplaysSixProducts(){Assert.That(await (await Open()).CountAsync(),Is.EqualTo(6));}
 [Test] public async Task TC04_InventoryUrlIsCorrect(){await Open();Assert.That(Page.Url,Does.EndWith("/inventory.html"));}
 [Test] public async Task TC05_CartBadgeIsHiddenInitially(){Assert.That(await (await Open()).BadgeVisibleAsync(),Is.False);}
}