using System.Text.Json;
using NUnit.Framework;

namespace SoftwareTestingExample2.Utilities;

/// <summary>
/// Writes one JSON record per framework event to NUnit's test output.
/// </summary>
public static class TestLogger
{
    public static void Log(
        string eventName,
        string level = "Information",
        object? properties = null,
        Exception? exception = null)
    {
        var test = TestContext.CurrentContext.Test;

        var record = new
        {
            timestamp = DateTimeOffset.UtcNow,
            level,
            eventName,
            test = new
            {
                className = test.ClassName,
                name = test.Name
            },
            properties,
            exception = exception is null
                ? null
                : new
                {
                    type = exception.GetType().FullName,
                    exception.Message,
                    exception.StackTrace
                }
        };

        TestContext.Progress.WriteLine(JsonSerializer.Serialize(record));
    }
}
