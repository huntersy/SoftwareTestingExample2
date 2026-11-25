
namespace SoftwareTestingExample.Utilities;

public static class TestConfig
{
    public const float DefaultTimeoutMilliseconds = 20_000;
    public static string BaseUrl =>
        Environment.GetEnvironmentVariable("BASE_URL")
        ?? "https://automationexercise.com/";
}
