using Microsoft.Playwright;
namespace SoftwareTestingExample.Pages;
public class ProductsPage:BasePage
{
 ILocator Cards=>Page.Locator(".inventory_item");
 ILocator Names=>Page.Locator(".inventory_item_name");
 ILocator Prices=>Page.Locator(".inventory_item_price");
 ILocator Sort=>Page.Locator("[data-test='product_sort_container']");
 ILocator Cart=>Page.Locator(".shopping_cart_link");
 ILocator Badge=>Page.Locator(".shopping_cart_badge");
 public ProductsPage(IPage p):base(p){}
 public async Task NavigateAsync(){await GoAsync("/inventory.html");await VisibleAsync(Cards.First);}
 public Task<bool> IsInventoryVisibleAsync()=>Cards.First.IsVisibleAsync();
 public Task<string> GetTitleAsync()=>Page.Locator(".title").InnerTextAsync();
 public Task<int> CountAsync()=>Cards.CountAsync();
 public Task<IReadOnlyList<string>> NamesAsync()=>Names.AllInnerTextsAsync();
 public Task<IReadOnlyList<string>> PricesAsync()=>Prices.AllInnerTextsAsync();
 public Task<bool> BadgeVisibleAsync()=>Badge.IsVisibleAsync();
 public async Task<int> CartCountAsync()=>int.Parse(await Badge.InnerTextAsync());
 public Task OpenCartAsync()=>Cart.ClickAsync();
 public Task SortAsync(string v)=>Sort.SelectOptionAsync(v);
 public Task<string> SelectedSortAsync()=>Sort.InputValueAsync();
 public Task<int> SortOptionsAsync()=>Sort.Locator("option").CountAsync();
 public async Task AddAsync(string slug){var b=Page.Locator($"[data-test='add-to-cart-{slug}']");await VisibleAsync(b);await b.ClickAsync();}
 public async Task RemoveAsync(string slug){var b=Page.Locator($"[data-test='remove-{slug}']");await VisibleAsync(b);await b.ClickAsync();}
 public async Task OpenDetailsAsync(string name){await Names.Filter(new(){HasText=name}).First.ClickAsync();await VisibleAsync(Page.Locator(".inventory_details_name"));}
 public async Task OpenMenuAsync(){await Page.Locator("#react-burger-menu-btn").ClickAsync();await VisibleAsync(Page.Locator("[data-test='logout-sidebar-link']"));}
 public Task LogoutAsync()=>Page.Locator("[data-test='logout-sidebar-link']").ClickAsync();
}