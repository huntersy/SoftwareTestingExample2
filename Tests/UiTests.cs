
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace SoftwareTestingExample.Tests;

public class UiTest : PageTest
{
    [SetUp]
    public async Task StartTracingAsync()
    {
        await Context.Tracing.StartAsync(new()
        {
            Title =
                $"{TestContext.CurrentContext.Test.ClassName}." +
                $"{TestContext.CurrentContext.Test.Name}",

            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    [TearDown]
    public async Task CaptureFailureDiagnosticsAsync()
    {
        var failed =
            TestContext.CurrentContext.Result.Outcome.Status
            == TestStatus.Failed;

        var artifactDirectory = Path.Combine(
            TestContext.CurrentContext.WorkDirectory,
            "TestResults",
            "FailureArtifacts"
        );

        string testName =
            $"{TestContext.CurrentContext.Test.ClassName}." +
            $"{TestContext.CurrentContext.Test.Name}";

        try
        {
            if (failed)
            {
                Directory.CreateDirectory(artifactDirectory);

                var screenshotPath = Path.Combine(
                    artifactDirectory,
                    $"{testName}.png"
                );

                try
                {
                    await Page.ScreenshotAsync(new()
                    {
                        Path = screenshotPath,
                        FullPage = true
                    });

                    TestContext.Progress.WriteLine(
                        $"Failure screenshot saved: {screenshotPath}"
                    );
                }
                catch (Exception exception)
                {
                    TestContext.Progress.WriteLine(
                        $"Screenshot capture failed: {exception.Message}"
                    );
                }

                var tracePath = Path.Combine(
                    artifactDirectory,
                    $"{testName}.zip"
                );

                await Context.Tracing.StopAsync(new()
                {
                    Path = tracePath
                });

                TestContext.Progress.WriteLine(
                    $"Playwright trace saved: {tracePath}"
                );
            }
            else
            {
                // Do not create trace files for successful tests.
                await Context.Tracing.StopAsync();
            }
        }
        catch (Exception exception)
        {
            TestContext.Progress.WriteLine(
                $"Failure diagnostics could not be completed: {exception.Message}"
            );
        }
    }
}
