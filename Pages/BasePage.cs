using Microsoft.Playwright;
using SoftwareTestingExample.Utilities;
namespace SoftwareTestingExample.Pages;
public abstract class BasePage
{
 protected readonly IPage Page;
 protected BasePage(IPage page)=>Page=page;
 protected async Task GoAsync(string path)
 {
  var r=await Page.GotoAsync(TestConfig.BaseUrl+path,new(){WaitUntil=WaitUntilState.DOMContentLoaded});
  if(r is null||r.Status>=400) throw new InvalidOperationException($"Navigation to {path} failed: HTTP {r?.Status}");
 }
 protected static Task VisibleAsync(ILocator l)=>l.WaitForAsync(new(){State=WaitForSelectorState.Visible});
}