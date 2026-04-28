using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MinBudget.Services;

/// <summary>
/// API response DTOs
/// </summary>
public class AuthResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = null!;

    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [JsonPropertyName("userId")]
    public string UserId { get; set; } = null!;
}

public class LoginRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [JsonPropertyName("password")]
    public string Password { get; set; } = null!;
}

public class RegisterRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [JsonPropertyName("password")]
    public string Password { get; set; } = null!;
}

/// <summary>
/// API Client for communicating with backend
/// </summary>
public class ApiClientService
{
    private readonly HttpClient _httpClient;
    private readonly AuthTokenService _authService;

    public ApiClientService(HttpClient httpClient, AuthTokenService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    /// <summary>
    /// Register new user
    /// </summary>
    public async Task<ApiResponse<AuthResponse>> RegisterAsync(string email, string password)
    {
        try
        {
            var request = new RegisterRequest { Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AuthResponse>(json);
                return new ApiResponse<AuthResponse> { Success = true, Data = result };
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return new ApiResponse<AuthResponse> { Success = false, Error = error };
            }
        }
        catch (Exception ex)
        {
            return new ApiResponse<AuthResponse> { Success = false, Error = ex.Message };
        }
    }

    /// <summary>
    /// Login user
    /// </summary>
    public async Task<ApiResponse<AuthResponse>> LoginAsync(string email, string password)
    {
        try
        {
            var request = new LoginRequest { Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AuthResponse>(json);
                // Save token
                await _authService.SaveTokenAsync(result!.Token, result.UserId, result.Email);
                return new ApiResponse<AuthResponse> { Success = true, Data = result };
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return new ApiResponse<AuthResponse> { Success = false, Error = error };
            }
        }
        catch (Exception ex)
        {
            return new ApiResponse<AuthResponse> { Success = false, Error = ex.Message };
        }
    }

    /// <summary>
    /// Make authenticated GET request to API
    /// </summary>
    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
    {
        try
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return new ApiResponse<T> { Success = false, Error = "Not authenticated" };

            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<T>(json);
                return new ApiResponse<T> { Success = true, Data = result };
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return new ApiResponse<T> { Success = false, Error = error };
            }
        }
        catch (Exception ex)
        {
            return new ApiResponse<T> { Success = false, Error = ex.Message };
        }
    }

    /// <summary>
    /// Make authenticated POST request to API
    /// </summary>
    public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return new ApiResponse<T> { Success = false, Error = "Not authenticated" };

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Content = JsonContent.Create(data);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<T>(json);
                return new ApiResponse<T> { Success = true, Data = result };
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return new ApiResponse<T> { Success = false, Error = error };
            }
        }
        catch (Exception ex)
        {
            return new ApiResponse<T> { Success = false, Error = ex.Message };
        }
    }

    /// <summary>
    /// Make authenticated DELETE request to API
    /// </summary>
    public async Task<ApiResponse<bool>> DeleteAsync(string endpoint)
    {
        try
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return new ApiResponse<bool> { Success = false, Error = "Not authenticated" };

            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return new ApiResponse<bool> { Success = true, Data = true };
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return new ApiResponse<bool> { Success = false, Error = error };
            }
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { Success = false, Error = ex.Message };
        }
    }
}

/// <summary>
/// Generic API response wrapper
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
}

