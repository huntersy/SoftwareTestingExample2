using System.Net.Http;
using System.Text.Json;

namespace SoftwareTestingExample.Utilities;

public sealed class TestAccountApi : IDisposable
{
    private readonly HttpClient _httpClient;

    public TestAccountApi()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri($"{TestConfig.BaseUrl.TrimEnd('/')}/"),
            Timeout = TimeSpan.FromMilliseconds(
                TestConfig.DefaultTimeoutMilliseconds)
        };
    }

    public async Task CreateAccountAsync(string email, string password)
    {
        using var content = new MultipartFormDataContent();

        AddField(content, "name", "QA Automation");
        AddField(content, "email", email);
        AddField(content, "password", password);
        AddField(content, "title", "Mr");
        AddField(content, "birth_date", "1");
        AddField(content, "birth_month", "1");
        AddField(content, "birth_year", "1990");
        AddField(content, "firstname", "QA");
        AddField(content, "lastname", "Automation");
        AddField(content, "company", "QA Testing");
        AddField(content, "address1", "100 Test Street");
        AddField(content, "address2", "Suite 200");
        AddField(content, "country", "United States");
        AddField(content, "zipcode", "77048");
        AddField(content, "state", "Texas");
        AddField(content, "city", "Houston");
        AddField(content, "mobile_number", "5551234567");

        using var response = await _httpClient.PostAsync(
            "api/createAccount",
            content);

        await ValidateResponseAsync(
            response,
            expectedResponseCode: 201,
            operation: "Create test account");
    }

    public async Task DeleteAccountAsync(string email, string password)
    {
        using var content = new MultipartFormDataContent();

        AddField(content, "email", email);
        AddField(content, "password", password);

        using var request = new HttpRequestMessage(
            HttpMethod.Delete,
            "api/deleteAccount")
        {
            Content = content
        };

        using var response = await _httpClient.SendAsync(request);

        await ValidateResponseAsync(
            response,
            expectedResponseCode: 200,
            operation: "Delete test account");
    }

    private static void AddField(
        MultipartFormDataContent content,
        string name,
        string value)
    {
        content.Add(new StringContent(value), name);
    }

    private static async Task ValidateResponseAsync(
        HttpResponseMessage response,
        int expectedResponseCode,
        string operation)
    {
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"{operation} failed. HTTP status: {(int)response.StatusCode}. " +
                $"Response: {responseBody}");
        }

        try
        {
            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;

            if (!root.TryGetProperty("responseCode", out var codeElement) ||
                !codeElement.TryGetInt32(out var actualResponseCode) ||
                actualResponseCode != expectedResponseCode)
            {
                var message = root.TryGetProperty("message", out var messageElement)
                    ? messageElement.GetString()
                    : "No API message was returned.";

                throw new InvalidOperationException(
                    $"{operation} returned an unexpected API response. " +
                    $"Expected responseCode {expectedResponseCode}; " +
                    $"actual response: {responseBody}. Message: {message}");
            }
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException(
                $"{operation} returned invalid JSON: {responseBody}",
                exception);
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
