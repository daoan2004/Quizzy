using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace ProjectBase.Tests;

internal static class CsrfTestClient
{
    public static async Task<HttpResponseMessage> PostAsJsonWithCsrfAsync<T>(
        this HttpClient client,
        string requestUri,
        T value)
    {
        var token = await client.GetCsrfTokenAsync();
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = JsonContent.Create(value)
        };
        request.Headers.Add("X-CSRF-TOKEN", token);
        return await client.SendAsync(request);
    }

    private static async Task<string> GetCsrfTokenAsync(this HttpClient client)
    {
        using var response = await client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        var match = Regex.Match(
            html,
            "<meta name=\"csrf-token\" content=\"([^\"]+)\"");
        if (!match.Success)
        {
            throw new InvalidOperationException("The layout did not render a CSRF token.");
        }

        return WebUtility.HtmlDecode(match.Groups[1].Value);
    }
}
