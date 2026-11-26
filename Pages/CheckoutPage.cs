using Microsoft.Playwright;
namespace SoftwareTestingExample.Pages;
public class CheckoutPage:BasePage
{
 public CheckoutPage(IPage p):base(p){}
 ILocator Error=>Page.Locator("[data-test='error']");
 public async Task FillAsync(string first,string last,string zip){await Page.Locator("[data-test='firstName']").FillAsync(first);await Page.Locator("[data-test='lastName']").FillAsync(last);await Page.Locator("[data-test='postalCode']").FillAsync(zip);}
 public Task ContinueAsync()=>Page.Locator("[data-test='continue']").ClickAsync();
 public Task<bool> ErrorVisibleAsync()=>Error.IsVisibleAsync();
 public Task<string> ErrorTextAsync()=>Error.InnerTextAsync();
 public Task<int> SummaryCountAsync()=>Page.Locator(".cart_item").CountAsync();
 public Task<string> TotalAsync()=>Page.Locator(".summary_total_label").InnerTextAsync();
 public Task FinishAsync()=>Page.Locator("[data-test='finish']").ClickAsync();
 public Task<bool> ConfirmationVisibleAsync()=>Page.Locator(".complete-header").IsVisibleAsync();
 public Task<string> ConfirmationAsync()=>Page.Locator(".complete-header").InnerTextAsync();
}