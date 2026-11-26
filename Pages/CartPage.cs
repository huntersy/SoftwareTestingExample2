using Microsoft.Playwright;
namespace SoftwareTestingExample.Pages;
public class CartPage:BasePage
{
 ILocator Items=>Page.Locator(".cart_item");
 ILocator Names=>Page.Locator(".inventory_item_name");
 public CartPage(IPage p):base(p){}
 public Task<int> CountAsync()=>Items.CountAsync();
 public Task<IReadOnlyList<string>> NamesAsync()=>Names.AllInnerTextsAsync();
 public Task<string> PriceAsync(string n)=>Items.Filter(new(){HasText=n}).Locator(".inventory_item_price").InnerTextAsync();
 public Task<string> QuantityAsync(string n)=>Items.Filter(new(){HasText=n}).Locator(".cart_quantity").InnerTextAsync();
 public async Task RemoveAsync(string slug){var b=Page.Locator($"[data-test='remove-{slug}']");await VisibleAsync(b);await b.ClickAsync();}
 public Task ContinueAsync()=>Page.Locator("[data-test='continue-shopping']").ClickAsync();
 public Task CheckoutAsync()=>Page.Locator("[data-test='checkout']").ClickAsync();
}