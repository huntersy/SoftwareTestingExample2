using Microsoft.Playwright;
namespace SoftwareTestingExample.Pages;
public class ProductDetailsPage:BasePage
{
 ILocator Name=>Page.Locator(".inventory_details_name");
 public ProductDetailsPage(IPage p):base(p){}
 public Task<string> GetNameAsync()=>Name.InnerTextAsync();
 public Task<string> GetDescriptionAsync()=>Page.Locator(".inventory_details_desc").InnerTextAsync();
 public Task<string> GetPriceAsync()=>Page.Locator(".inventory_details_price").InnerTextAsync();
 public Task<bool> BackVisibleAsync()=>Page.Locator("[data-test='back-to-products']").IsVisibleAsync();
 public Task BackAsync()=>Page.Locator("[data-test='back-to-products']").ClickAsync();
}