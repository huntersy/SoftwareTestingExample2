using Microsoft.Playwright.NUnit;using NUnit.Framework;using SoftwareTestingExample.Pages;using SoftwareTestingExample.TestData;using System.Globalization;
namespace SoftwareTestingExample.Tests;
public class ProductFilterTests:UiTest
{
 async Task<ProductsPage> Open(){var l=new LoginPage(Page);await l.NavigateAsync();await l.LoginAsync(TestUsers.Standard,TestUsers.Password);var p=new ProductsPage(Page);await p.IsInventoryVisibleAsync();return p;}
 [Test,Category("Smoke")] public async Task TC18_SortNamesAscending(){var p=await Open();await p.SortAsync("az");Assert.That(await p.NamesAsync(),Is.Ordered.Using(StringComparer.Ordinal));}
 [Test] public async Task TC19_SortNamesDescending(){var p=await Open();await p.SortAsync("za");Assert.That(await p.NamesAsync(),Is.Ordered.Descending.Using(StringComparer.Ordinal));}
 [Test] public async Task TC20_SortPricesAscending(){var p=await Open();await p.SortAsync("lohi");var a=(await p.PricesAsync()).Select(x=>decimal.Parse(x.TrimStart('$'),CultureInfo.InvariantCulture));Assert.That(a,Is.Ordered.Ascending);}
 [Test] public async Task TC21_SortPricesDescending(){var p=await Open();await p.SortAsync("hilo");var a=(await p.PricesAsync()).Select(x=>decimal.Parse(x.TrimStart('$'),CultureInfo.InvariantCulture));Assert.That(a,Is.Ordered.Descending);}
}