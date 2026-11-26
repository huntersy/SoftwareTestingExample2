namespace SoftwareTestingExample.Utilities;
public static class TestConfig
{
 public const float DefaultTimeoutMilliseconds=15000;
 public static string BaseUrl=>(Environment.GetEnvironmentVariable("BASE_URL")??"https://www.saucedemo.com").TrimEnd('/');
}