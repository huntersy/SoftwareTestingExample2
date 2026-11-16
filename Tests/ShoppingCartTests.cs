
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using SoftwareTestingExample.Pages;

namespace SoftwareTestingExample.Tests;

public class ShoppingCartTests : PageTest
{
    private async Task<CartPage> AddBlueTopAndOpenCartAsync()
    {
        var productsPage = new ProductsPage(Page);

        await productsPage.NavigateAsync();

        await productsPage.AddProductToCartAsync("1");

        await productsPage.OpenCartFromAddToCartModalAsync();

        var cartPage = new CartPage(Page);

        await cartPage.WaitForCartItemsAsync();

        return cartPage;
    }

    // TC23: Verify that a product can be added to the cart.
    [Test]
    public async Task TC23_AddProductToCartDisplaysConfirmation()
    {
        var productsPage = new ProductsPage(Page);

        await productsPage.NavigateAsync();

        await productsPage.AddProductToCartAsync("1");

        Assert.That(
            await productsPage.IsAddToCartModalVisibleAsync(),
            Is.True,
            "The add-to-cart confirmation should be displayed."
        );
    }

    // TC24: Verify that the added product appears in the cart.
    [Test]
    public async Task TC24_AddedProductAppearsInCart()
    {
        var cartPage = await AddBlueTopAndOpenCartAsync();

        var productNames =
            await cartPage.GetProductNamesAsync();

        Assert.That(
            productNames,
            Does.Contain("Blue Top"),
            "The added Blue Top product should appear in the cart."
        );
    }

    // TC25: Verify product name, price, quantity, and total.
    [Test]
    public async Task TC25_CartDisplaysCorrectProductDetails()
    {
        var cartPage = await AddBlueTopAndOpenCartAsync();

        var productNames =
            await cartPage.GetProductNamesAsync();

        var price =
            await cartPage.GetProductPriceByNameAsync("Blue Top");

        var quantity =
            await cartPage.GetProductQuantityByNameAsync("Blue Top");

        var total =
            await cartPage.GetProductTotalByNameAsync("Blue Top");

        Assert.That(
            productNames,
            Does.Contain("Blue Top"),
            "The cart should display the correct product."
        );

        Assert.That(
            price,
            Does.StartWith("Rs."),
            "The product price should display the currency."
        );

        Assert.That(
            quantity,
            Is.EqualTo(1),
            "The default product quantity should be one."
        );

        Assert.That(
            total,
            Is.EqualTo(price),
            "For a quantity of one, the total should equal the unit price."
        );
    }

    // TC26: Verify that product quantity can be set before adding it.
    [Test]
    public async Task TC26_ProductQuantityCanBeSetBeforeAddingToCart()
    {
        var productsPage = new ProductsPage(Page);

        await productsPage.NavigateAsync();

        await productsPage.OpenProductDetailsAsync("1");

        var productDetailsPage =
            new ProductDetailsPage(Page);

        var productName =
            await productDetailsPage.GetProductNameAsync();

        Assert.That(
            productName,
            Is.EqualTo("Blue Top"),
            "The selected product details should be displayed."
        );

        await productDetailsPage.SetQuantityAsync(4);

        await productDetailsPage.AddToCartAsync();

        await productDetailsPage.OpenCartFromAddToCartModalAsync();

        var cartPage = new CartPage(Page);

        await cartPage.WaitForCartItemsAsync();

        var quantity =
            await cartPage.GetProductQuantityByNameAsync("Blue Top");

        Assert.That(
            quantity,
            Is.EqualTo(4),
            "The cart should display the requested quantity of four."
        );
    }

    // TC27: Verify that a product can be removed from the cart.
    [Test]
    public async Task TC27_ProductCanBeRemovedFromCart()
    {
        var cartPage = await AddBlueTopAndOpenCartAsync();

        Assert.That(
            await cartPage.GetCartItemCountAsync(),
            Is.EqualTo(1),
            "The cart should initially contain one product."
        );

        await cartPage.RemoveProductByNameAsync("Blue Top");

        Assert.That(
            await cartPage.GetCartItemCountAsync(),
            Is.EqualTo(0),
            "The cart should contain no products after removal."
        );

        Assert.That(
            await cartPage.IsEmptyCartMessageVisibleAsync(),
            Is.True,
            "The empty cart message should be displayed."
        );
    }
}
