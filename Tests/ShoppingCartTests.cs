using Microsoft.Playwright.NUnit;using NUnit.Framework;using SoftwareTestingExample.Pages;using SoftwareTestingExample.TestData;
namespace SoftwareTestingExample.Tests;
public class ShoppingCartTests:UiTest
{
 async Task<ProductsPage> Open(){var l=new LoginPage(Page);await l.NavigateAsync();await l.LoginAsync(TestUsers.Standard,TestUsers.Password);var p=new ProductsPage(Page);await p.IsInventoryVisibleAsync();return p;}
 [Test,Category("Smoke")] public async Task TC22_AddingItemUpdatesBadge(){var p=await Open();await p.AddAsync(TestUsers.BackpackSlug);Assert.That(await p.CartCountAsync(),Is.EqualTo(1));}
 [Test] public async Task TC23_AddedProductAppearsInCart(){var p=await Open();await p.AddAsync(TestUsers.BackpackSlug);await p.OpenCartAsync();Assert.That(await new CartPage(Page).NamesAsync(),Does.Contain(TestUsers.Backpack));}
 [Test] public async Task TC24_CartShowsCorrectPrice(){var p=await Open();await p.AddAsync(TestUsers.BackpackSlug);await p.OpenCartAsync();Assert.That(await new CartPage(Page).PriceAsync(TestUsers.Backpack),Is.EqualTo("$29.99"));}
 [Test] public async Task TC25_CartQuantityDefaultsToOne(){var p=await Open();await p.AddAsync(TestUsers.BackpackSlug);await p.OpenCartAsync();Assert.That(await new CartPage(Page).QuantityAsync(TestUsers.Backpack),Is.EqualTo("1"));}
 [Test] public async Task TC26_ItemCanBeRemovedFromInventory(){var p=await Open();await p.AddAsync(TestUsers.BackpackSlug);await p.RemoveAsync(TestUsers.BackpackSlug);Assert.That(await p.BadgeVisibleAsync(),Is.False);}
 [Test] public async Task TC27_ItemCanBeRemovedFromCart(){var p=await Open();await p.AddAsync(TestUsers.BackpackSlug);await p.OpenCartAsync();var c=new CartPage(Page);await c.RemoveAsync(TestUsers.BackpackSlug);Assert.That(await c.CountAsync(),Is.Zero);}
 [Test] public async Task TC28_MultipleItemsCanBeAdded(){var p=await Open();await p.AddAsync(TestUsers.BackpackSlug);await p.AddAsync(TestUsers.BikeLightSlug);Assert.That(await p.CartCountAsync(),Is.EqualTo(2));}
 [Test] public async Task TC29_CartStatePersistsWhenContinuingShopping(){var p=await Open();await p.AddAsync(TestUsers.BackpackSlug);await p.OpenCartAsync();await new CartPage(Page).ContinueAsync();Assert.That(await p.CartCountAsync(),Is.EqualTo(1));}
}