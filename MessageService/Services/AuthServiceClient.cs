using MessengerLib.Models;
using System.Net.Http.Headers;

namespace MessageService.Services;
public class AuthServiceClient
{
    private readonly HttpClient _httpClient;

    public AuthServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Get current user using access token
    public async Task<ApplicationUser?> GetCurrentUserAsync(string accessToken)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            return await _httpClient.GetFromJsonAsync<ApplicationUser>("/api/users/me");
        }
        catch
        {
            return null;
        }
    }
}