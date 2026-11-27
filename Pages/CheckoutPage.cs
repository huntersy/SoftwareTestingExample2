using Microsoft.Playwright;
namespace SoftwareTestingExample.Pages;
public class CheckoutPage:BasePage
{
 public CheckoutPage(IPage p):base(p){}
 ILocator Error=>Page.Locator("[data-test='error']");
 ILocator SummaryItems=>Page.Locator(".cart_item");
 ILocator SummaryTotal=>Page.Locator(".summary_total_label");
 ILocator FinishButton=>Page.Locator("[data-test='finish']");
 ILocator Confirmation=>Page.Locator(".complete-header");
 public async Task FillAsync(string first,string last,string zip){await Page.Locator("[data-test='firstName']").FillAsync(first);await Page.Locator("[data-test='lastName']").FillAsync(last);await Page.Locator("[data-test='postalCode']").FillAsync(zip);}
 public Task ContinueAsync()=>Page.Locator("[data-test='continue']").ClickAsync();
 public Task<bool> ErrorVisibleAsync()=>Error.IsVisibleAsync();
 public Task<string> ErrorTextAsync()=>Error.InnerTextAsync();
 public async Task<int> SummaryCountAsync(){await VisibleAsync(SummaryItems.First);return await SummaryItems.CountAsync();}
 public async Task<string> TotalAsync(){await VisibleAsync(SummaryTotal);return await SummaryTotal.InnerTextAsync();}
 public Task FinishAsync()=>FinishButton.ClickAsync();
 public async Task<bool> ConfirmationVisibleAsync(){await VisibleAsync(Confirmation);return await Confirmation.IsVisibleAsync();}
 public Task<string> ConfirmationAsync()=>Confirmation.InnerTextAsync();
}