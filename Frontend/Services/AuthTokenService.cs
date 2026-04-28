using MinBudget.Service;

namespace MinBudget.Services;

/// <summary>
/// Handles JWT token storage and retrieval from localStorage
/// </summary>
public class AuthTokenService
{
    private readonly LocalStorageService _localStorage;
    private const string TokenKey = "authToken";
    private const string UserIdKey = "userId";
    private const string EmailKey = "email";

    public AuthTokenService(LocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    /// <summary>
    /// Save token and user info to localStorage
    /// </summary>
    public async Task SaveTokenAsync(string token, string userId, string email)
    {
        await _localStorage.SaveListAsync(TokenKey, new List<string> { token });
        await _localStorage.SaveListAsync(UserIdKey, new List<string> { userId });
        await _localStorage.SaveListAsync(EmailKey, new List<string> { email });
    }

    /// <summary>
    /// Get stored JWT token
    /// </summary>
    public async Task<string?> GetTokenAsync()
    {
        var tokens = await _localStorage.ReadListAsync<string>(TokenKey);
        return tokens.FirstOrDefault();
    }

    /// <summary>
    /// Get stored user ID
    /// </summary>
    public async Task<string?> GetUserIdAsync()
    {
        var userIds = await _localStorage.ReadListAsync<string>(UserIdKey);
        return userIds.FirstOrDefault();
    }

    /// <summary>
    /// Get stored email
    /// </summary>
    public async Task<string?> GetEmailAsync()
    {
        var emails = await _localStorage.ReadListAsync<string>(EmailKey);
        return emails.FirstOrDefault();
    }

    /// <summary>
    /// Check if user is authenticated
    /// </summary>
    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    /// <summary>
    /// Clear all auth data (logout)
    /// </summary>
    public async Task ClearAsync()
    {
        await _localStorage.SaveListAsync(TokenKey, new List<string>());
        await _localStorage.SaveListAsync(UserIdKey, new List<string>());
        await _localStorage.SaveListAsync(EmailKey, new List<string>());
    }
}

