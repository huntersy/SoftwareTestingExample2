using Microsoft.Playwright;
using NUnit.Framework;
using SoftwareTestingExample.Pages;
using SoftwareTestingExample.Utilities;

namespace SoftwareTestingExample.Tests;

public class CheckoutTests : UiTest
{
    private const string TestPassword = "QaTest123!";

    private const string TestCardholder = "QA Automation";

    // Synthetic payment data for the practice website.
    private const string TestCardNumber = "4111111111111111";
    private const string TestCvc = "123";
    private const string TestExpiryMonth = "12";
    private const string TestExpiryYear = "2030";

    private static string GenerateUniqueEmail() =>
        $"qa{Guid.NewGuid():N}@example.com";

    private async Task PrepareRegisteredCheckoutAsync(
        TestAccountApi accountApi,
        string email,
        Action markAccountCreated)
    {
        // Use the documented API for disposable test-data setup.
        // The login, shopping cart, and checkout journey remain UI-driven.
        await accountApi.CreateAccountAsync(email, TestPassword);
        markAccountCreated();

        var loginPage = new LoginPage(Page);

        await loginPage.NavigateAsync();
        await loginPage.LoginAsync(email, TestPassword);
        await loginPage.WaitForLoggedInAsync();

        Assert.That(
            await loginPage.IsLoggedInAsync(),
            Is.True,
            "The API-created test account should be able to log in through the UI."
        );

        var productsPage = new ProductsPage(Page);

        await productsPage.NavigateAsync();

        await productsPage.AddProductToCartAsync("1");

        await productsPage.OpenCartFromAddToCartModalAsync();

        var cartPage = new CartPage(Page);

        await cartPage.WaitForCartItemsAsync();

        await cartPage.ProceedToCheckoutAsync();

        var checkoutPage = new CheckoutPage(Page);

        await checkoutPage.WaitForCheckoutDetailsAsync();
    }

    private async Task CompleteTestOrderAsync(
        CheckoutPage checkoutPage)
    {
        await checkoutPage.ProceedToPaymentAsync(
            "Automated checkout test order."
        );

        await checkoutPage.PayAndConfirmOrderAsync(
            TestCardholder,
            TestCardNumber,
            TestCvc,
            TestExpiryMonth,
            TestExpiryYear
        );
    }

    private async Task CleanupTestAccountAsync(
        TestAccountApi accountApi,
        string email,
        bool accountCreated)
    {
        if (!accountCreated)
        {
            return;
        }

        try
        {
            await accountApi.DeleteAccountAsync(email, TestPassword);

            TestContext.Progress.WriteLine(
                "Test account cleanup completed through the documented API."
            );
        }
        catch (Exception exception)
        {
            // Do not hide the original test failure if cleanup fails.
            TestContext.Progress.WriteLine(
                $"Test account API cleanup could not complete: {exception.Message}"
            );
        }
    }

    // TC35: Verify that guest checkout requires registration or login.
    [Test]
    public async Task TC35_GuestCheckoutRequiresRegistrationOrLogin()
    {
        var productsPage = new ProductsPage(Page);

        await productsPage.NavigateAsync();

        await productsPage.AddProductToCartAsync("1");

        await productsPage.OpenCartFromAddToCartModalAsync();

        var cartPage = new CartPage(Page);

        await cartPage.WaitForCartItemsAsync();

        await cartPage.ProceedToCheckoutAsync();

        var checkoutPage = new CheckoutPage(Page);

        Assert.That(
            await checkoutPage.IsRegisterLoginPromptVisibleAsync(),
            Is.True,
            "A guest should be prompted to register or log in before checkout."
        );
    }

    // TC36: Verify delivery and billing addresses during checkout.
    [Test]
    public async Task TC36_CheckoutDisplaysCorrectDeliveryAndBillingAddresses()
    {
        using var accountApi = new TestAccountApi();
        var email = GenerateUniqueEmail();
        var accountCreated = false;

        try
        {
            await PrepareRegisteredCheckoutAsync(
                accountApi,
                email,
                () => accountCreated = true
            );

            var checkoutPage = new CheckoutPage(Page);

            var deliveryAddress =
                await checkoutPage.GetDeliveryAddressAsync();

            var billingAddress =
                await checkoutPage.GetBillingAddressAsync();

            Assert.Multiple(() =>
            {
                Assert.That(
                    deliveryAddress,
                    Does.Contain("100 Test Street"),
                    "The delivery address should contain the registered street."
                );

                Assert.That(
                    deliveryAddress,
                    Does.Contain("Houston"),
                    "The delivery address should contain Houston."
                );

                Assert.That(
                    deliveryAddress,
                    Does.Contain("Texas"),
                    "The delivery address should contain Texas."
                );

                Assert.That(
                    billingAddress,
                    Does.Contain("100 Test Street"),
                    "The billing address should contain the registered street."
                );

                Assert.That(
                    billingAddress,
                    Does.Contain("Houston"),
                    "The billing address should contain Houston."
                );

                Assert.That(
                    billingAddress,
                    Does.Contain("Texas"),
                    "The billing address should contain Texas."
                );
            });
        }
        finally
        {
            await CleanupTestAccountAsync(
                accountApi,
                email,
                accountCreated
            );
        }
    }

    // TC37: Verify product details in the checkout order review.
    [Test]
    public async Task TC37_CheckoutDisplaysCorrectOrderDetails()
    {
        using var accountApi = new TestAccountApi();
        var email = GenerateUniqueEmail();
        var accountCreated = false;

        try
        {
            await PrepareRegisteredCheckoutAsync(
                accountApi,
                email,
                () => accountCreated = true
            );

            var checkoutPage = new CheckoutPage(Page);

            var productNames =
                await checkoutPage.GetOrderProductNamesAsync();

            var price =
                await checkoutPage.GetOrderProductPriceAsync("Blue Top");

            var quantity =
                await checkoutPage.GetOrderProductQuantityAsync("Blue Top");

            var total =
                await checkoutPage.GetOrderProductTotalAsync("Blue Top");

            Assert.Multiple(() =>
            {
                Assert.That(
                    productNames,
                    Does.Contain("Blue Top"),
                    "The order review should display Blue Top."
                );

                Assert.That(
                    price,
                    Does.StartWith("Rs."),
                    "The product price should display the currency."
                );

                Assert.That(
                    quantity,
                    Is.EqualTo(1),
                    "The order quantity should be one."
                );

                Assert.That(
                    total,
                    Is.EqualTo(price),
                    "The total should equal the unit price for quantity one."
                );
            });
        }
        finally
        {
            await CleanupTestAccountAsync(
                accountApi,
                email,
                accountCreated
            );
        }
    }

    // TC38: Verify that a registered customer can place an order.
    [Test]
    public async Task TC38_RegisteredCustomerCanPlaceOrderSuccessfully()
    {
        using var accountApi = new TestAccountApi();
        var email = GenerateUniqueEmail();
        var accountCreated = false;

        try
        {
            await PrepareRegisteredCheckoutAsync(
                accountApi,
                email,
                () => accountCreated = true
            );

            var checkoutPage = new CheckoutPage(Page);

            await CompleteTestOrderAsync(checkoutPage);

            Assert.That(
                await checkoutPage.IsOrderConfirmationVisibleAsync(),
                Is.True,
                "The successful order confirmation should be displayed."
            );
        }
        finally
        {
            await CleanupTestAccountAsync(
                accountApi,
                email,
                accountCreated
            );
        }
    }

    // TC39: Verify that an invoice can be downloaded after purchase.
    [Test]
    public async Task TC39_CustomerCanDownloadInvoiceAfterPurchase()
    {
        using var accountApi = new TestAccountApi();
        var email = GenerateUniqueEmail();
        var accountCreated = false;
        string? invoicePath = null;

        try
        {
            await PrepareRegisteredCheckoutAsync(
                accountApi,
                email,
                () => accountCreated = true
            );

            var checkoutPage = new CheckoutPage(Page);

            await CompleteTestOrderAsync(checkoutPage);

            Assert.That(
                await checkoutPage.IsOrderConfirmationVisibleAsync(),
                Is.True,
                "The order should be confirmed before downloading the invoice."
            );

            invoicePath =
                await checkoutPage.DownloadInvoiceAsync();

            Assert.That(
                File.Exists(invoicePath),
                Is.True,
                "The invoice file should exist after downloading."
            );

            Assert.That(
                new FileInfo(invoicePath).Length,
                Is.GreaterThan(0),
                "The downloaded invoice should not be empty."
            );
        }
        finally
        {
            if (invoicePath is not null &&
                File.Exists(invoicePath))
            {
                File.Delete(invoicePath);
            }

            await CleanupTestAccountAsync(
                accountApi,
                email,
                accountCreated
            );
        }
    }

    // TC40: Verify that the cart is empty after a successful order.
    [Test]
    public async Task TC40_CartIsEmptyAfterSuccessfulOrder()
    {
        using var accountApi = new TestAccountApi();
        var email = GenerateUniqueEmail();
        var accountCreated = false;

        try
        {
            await PrepareRegisteredCheckoutAsync(
                accountApi,
                email,
                () => accountCreated = true
            );

            var checkoutPage = new CheckoutPage(Page);

            await CompleteTestOrderAsync(checkoutPage);

            Assert.That(
                await checkoutPage.IsOrderConfirmationVisibleAsync(),
                Is.True,
                "The order should be confirmed before checking the cart."
            );

            await Page.GotoAsync(
                $"{TestConfig.BaseUrl.TrimEnd('/')}/view_cart",
                new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.DOMContentLoaded,
                }
            );

            var emptyCartMessage = Page.GetByText(
                "Cart is empty! Click here to buy products.",
                new() { Exact = false }
            );

            await emptyCartMessage.WaitForAsync(new()
            {
                State = WaitForSelectorState.Visible,
            });

            var cartPage = new CartPage(Page);

            Assert.That(
                await cartPage.IsEmptyCartMessageVisibleAsync(),
                Is.True,
                "The cart should be empty after a successful purchase."
            );
        }
        finally
        {
            await CleanupTestAccountAsync(
                accountApi,
                email,
                accountCreated
            );
        }
    }
}
