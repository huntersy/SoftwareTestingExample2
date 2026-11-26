
using Microsoft.Playwright;
using SoftwareTestingExample.Utilities;

namespace SoftwareTestingExample.Pages;

public class HomePage
{
    private readonly IPage _page;

    private ILocator ProductCards =>
        _page.Locator(".product-image-wrapper");

    private ILocator ProductNames =>
        _page.Locator(".productinfo p");

    private ILocator WomenCategoryLink =>
        _page.Locator("a[href='#Women']");

    private ILocator WomenCategorySection =>
        _page.Locator("#Women");

    private ILocator WomenSubcategoryLinks =>
        WomenCategorySection.Locator("a[href^='/category_products/']");

    private ILocator WomenTopsLink =>
        WomenCategorySection.Locator("a[href='/category_products/2']");

    private ILocator AdvertisementClosePrompt =>
        _page.Locator(".continue-prompt-text");

    public HomePage(IPage page)
    {
        _page = page;
    }

    public async Task NavigateAsync()
    {
        var response = await _page.GotoAsync(
            TestConfig.BaseUrl,
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
            }
        );

        if (response is null ||
            response.Status < 200 ||
            response.Status >= 400)
        {
            throw new InvalidOperationException(
                $"Homepage navigation failed. HTTP status: {response?.Status}"
            );
        }

        await ProductCards.First.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
        });
    }

    public async Task<string> GetPageTitleAsync() =>
        await _page.TitleAsync();

    public Task<string> GetCurrentUrlAsync() =>
        Task.FromResult(_page.Url);

    public async Task<int> GetProductCardCountAsync()
    {
        await ProductCards.First.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
        });

        return await ProductCards.CountAsync();
    }

    public async Task<IReadOnlyList<string>> GetProductNamesAsync() =>
        await ProductNames.AllInnerTextsAsync();

    public async Task ExpandWomenCategoryAsync()
    {
        await CloseAdvertisementIfPresentAsync();

        await WomenCategoryLink.ClickAsync();

        await WomenSubcategoryLinks.First.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible,
        });
    }

    public async Task<int> GetWomenSubcategoryCountAsync() =>
        await WomenSubcategoryLinks.CountAsync();

    public async Task<IReadOnlyList<string>> GetWomenSubcategoryNamesAsync() =>
        await WomenSubcategoryLinks.AllInnerTextsAsync();

    public async Task SelectWomenTopsCategoryAsync()
    {
        await ExpandWomenCategoryAsync();

        var categoryUrl =
            $"{TestConfig.BaseUrl.TrimEnd('/')}/category_products/2";

        await SafeNavigation.NavigateByClickAsync(
            _page,
            WomenTopsLink,
            categoryUrl,
            ProductCards.First,
            "Women Tops category navigation"
        );
    }

    public async Task CloseAdvertisementIfPresentAsync()
    {
        if (await AdvertisementClosePrompt.IsVisibleAsync())
        {
            await AdvertisementClosePrompt.ClickAsync();
        }
    }
}
