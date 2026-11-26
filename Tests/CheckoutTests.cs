using Microsoft.Playwright.NUnit;using NUnit.Framework;using SoftwareTestingExample.Pages;using SoftwareTestingExample.TestData;
namespace SoftwareTestingExample.Tests;
public class CheckoutTests:UiTest
{
 async Task Start(){var l=new LoginPage(Page);await l.NavigateAsync();await l.LoginAsync(TestUsers.Standard,TestUsers.Password);var p=new ProductsPage(Page);await p.AddAsync(TestUsers.BackpackSlug);await p.OpenCartAsync();await new CartPage(Page).CheckoutAsync();}
 [Test,Category("Smoke")] public async Task TC36_CheckoutRequiresCustomerInformation(){await Start();var c=new CheckoutPage(Page);await c.ContinueAsync();Assert.That(await c.ErrorVisibleAsync(),Is.True);}
 [Test] public async Task TC37_MissingFirstNameIsRejected(){await Start();var c=new CheckoutPage(Page);await c.FillAsync("","Automation","77048");await c.ContinueAsync();Assert.That(await c.ErrorTextAsync(),Does.Contain("First Name"));}
 [Test] public async Task TC38_MissingLastNameIsRejected(){await Start();var c=new CheckoutPage(Page);await c.FillAsync("QA","","77048");await c.ContinueAsync();Assert.That(await c.ErrorTextAsync(),Does.Contain("Last Name"));}
 [Test] public async Task TC39_MissingPostalCodeIsRejected(){await Start();var c=new CheckoutPage(Page);await c.FillAsync("QA","Automation","");await c.ContinueAsync();Assert.That(await c.ErrorTextAsync(),Does.Contain("Postal Code"));}
 [Test] public async Task TC40_StandardUserCanCompleteOrder(){await Start();var c=new CheckoutPage(Page);await c.FillAsync(TestUsers.FirstName,TestUsers.LastName,TestUsers.PostalCode);await c.ContinueAsync();Assert.That(await c.SummaryCountAsync(),Is.EqualTo(1));Assert.That(await c.TotalAsync(),Does.StartWith("Total:"));await c.FinishAsync();Assert.That(await c.ConfirmationVisibleAsync(),Is.True);Assert.That(await c.ConfirmationAsync(),Does.Contain("Thank you for your order"));}
}