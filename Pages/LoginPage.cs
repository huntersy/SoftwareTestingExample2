using Microsoft.Playwright;
namespace SoftwareTestingExample.Pages;
public class LoginPage:BasePage
{
 ILocator User=>Page.Locator("[data-test='username']");
 ILocator Pass=>Page.Locator("[data-test='password']");
 ILocator Submit=>Page.Locator("[data-test='login-button']");
 ILocator Error=>Page.Locator("[data-test='error']");
 public LoginPage(IPage p):base(p){}
 public async Task NavigateAsync(){await GoAsync("/");await VisibleAsync(User);}
 public Task<bool> IsUsernameVisibleAsync()=>User.IsVisibleAsync();
 public Task<bool> IsPasswordVisibleAsync()=>Pass.IsVisibleAsync();
 public Task<bool> IsLoginButtonVisibleAsync()=>Submit.IsVisibleAsync();
 public async Task LoginAsync(string u,string p){await User.FillAsync(u);await Pass.FillAsync(p);await Submit.ClickAsync();}
 public Task<bool> IsErrorVisibleAsync()=>Error.IsVisibleAsync();
 public Task<string> GetErrorAsync()=>Error.InnerTextAsync();
}