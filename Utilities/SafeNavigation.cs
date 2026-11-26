using Microsoft.Playwright;

namespace SoftwareTestingExample.Utilities;

/// <summary>
/// Retries only safe, idempotent page GET navigations when the site
/// explicitly responds with HTTP 429 or a 5xx server error.
/// </summary>
public static class SafeNavigation
{
    private const int MaxAttempts = 3;

    public static async Task NavigateByGetAsync(
        IPage page,
        string destinationUrl,
        ILocator readyLocator,
        string navigationName)
    {
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            var response = await page.GotoAsync(destinationUrl, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
            });

            if (response is null)
            {
                throw new InvalidOperationException(
                    $"{navigationName} navigation completed without an HTTP response.");
            }

            if (response.Status == 429 || response.Status >= 500)
            {
                if (attempt == MaxAttempts)
                {
                    throw new InvalidOperationException(
                        $"{navigationName} returned HTTP {response.Status} after {MaxAttempts} attempts.");
                }

                await page.WaitForTimeoutAsync(750 * attempt);
                continue;
            }

            if (response.Status < 200 || response.Status >= 400)
            {
                throw new InvalidOperationException(
                    $"{navigationName} failed with HTTP {response.Status}.");
            }

            await readyLocator.WaitForAsync(new()
            {
                State = WaitForSelectorState.Visible,
            });

            return;
        }
    }

    public static async Task NavigateByClickAsync(
        IPage page,
        ILocator navigationLink,
        string destinationUrl,
        ILocator readyLocator,
        string navigationName)
    {
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            IResponse? response;

            if (attempt == 1)
            {
                var expectedPath = new Uri(destinationUrl).AbsolutePath;
                var responseTask = page.WaitForResponseAsync(
                    candidate =>
                    {
                        if (!string.Equals(candidate.Request.Method, "GET",
                            StringComparison.OrdinalIgnoreCase) ||
                            !Uri.TryCreate(candidate.Url, UriKind.Absolute, out var responseUri))
                        {
                            return false;
                        }

                        return string.Equals(responseUri.AbsolutePath, expectedPath,
                            StringComparison.OrdinalIgnoreCase);
                    },
                    new PageWaitForResponseOptions
                    {
                        Timeout = TestConfig.DefaultTimeoutMilliseconds,
                    });

                // Force the click to avoid the site's known overlay/ad
                // interception, while still observing the resulting GET.
                await navigationLink.ClickAsync(new() { Force = true });
                response = await responseTask;
            }
            else
            {
                // Retry only the destination GET, never the original UI action.
                response = await page.GotoAsync(destinationUrl, new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.DOMContentLoaded,
                });
            }

            if (response is null)
            {
                throw new InvalidOperationException(
                    $"{navigationName} navigation completed without an HTTP response.");
            }

            if (response.Status == 429 || response.Status >= 500)
            {
                if (attempt == MaxAttempts)
                {
                    throw new InvalidOperationException(
                        $"{navigationName} returned HTTP {response.Status} after {MaxAttempts} attempts.");
                }

                await page.WaitForTimeoutAsync(750 * attempt);
                continue;
            }

            if (response.Status < 200 || response.Status >= 400)
            {
                throw new InvalidOperationException(
                    $"{navigationName} failed with HTTP {response.Status}.");
            }

            await readyLocator.WaitForAsync(new()
            {
                State = WaitForSelectorState.Visible,
            });

            return;
        }
    }
}