using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using SoftwareTestingExample2.Utilities;

namespace SoftwareTestingExample.Tests;

public class UiTest : PageTest
{
    [SetUp]
    public async Task StartTracingAsync()
    {
        Page.SetDefaultTimeout(SoftwareTestingExample.Utilities.TestConfig.DefaultTimeoutMilliseconds);
        Page.SetDefaultNavigationTimeout(SoftwareTestingExample.Utilities.TestConfig.DefaultTimeoutMilliseconds);

        TestLogger.Log("test.started");

        await Context.Tracing.StartAsync(new()
        {
            Title =
                $"{TestContext.CurrentContext.Test.ClassName}." +
                $"{TestContext.CurrentContext.Test.Name}",

            Screenshots = true,
            Snapshots = true,
            Sources = true
        });

        TestLogger.Log(
            "playwright.trace.started",
            properties: new { screenshots = true, snapshots = true, sources = true }
        );
    }

    [TearDown]
    public async Task CaptureFailureDiagnosticsAsync()
    {
        var result = TestContext.CurrentContext.Result;
        var failed = result.Outcome.Status == TestStatus.Failed;

        var artifactDirectory = Path.Combine(
            TestContext.CurrentContext.WorkDirectory,
            "TestResults",
            "FailureArtifacts"
        );

        string testName =
            $"{TestContext.CurrentContext.Test.ClassName}." +
            $"{TestContext.CurrentContext.Test.Name}";

        TestLogger.Log(
            failed ? "test.failed" : "test.passed",
            failed ? "Error" : "Information",
            new
            {
                result.Outcome.Status,
                result.Message
            }
        );

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

                    TestLogger.Log(
                        "failure.screenshot.saved",
                        properties: new { path = screenshotPath }
                    );
                }
                catch (Exception exception)
                {
                    TestLogger.Log(
                        "failure.screenshot.capture_failed",
                        "Warning",
                        exception: exception
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

                TestLogger.Log(
                    "failure.trace.saved",
                    properties: new { path = tracePath }
                );
            }
            else
            {
                await Context.Tracing.StopAsync();
                TestLogger.Log("playwright.trace.stopped");
            }
        }
        catch (Exception exception)
        {
            TestLogger.Log(
                "failure.diagnostics.incomplete",
                "Warning",
                exception: exception
            );
        }
    }
}
