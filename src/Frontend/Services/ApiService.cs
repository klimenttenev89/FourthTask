using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using NorthwindTraders.Frontend.Models;

namespace NorthwindTraders.Frontend.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _contextAccessor;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiService(HttpClient http, IHttpContextAccessor contextAccessor)
    {
        _http = http;
        _contextAccessor = contextAccessor;
    }

    private void AttachToken()
    {
        var token = _contextAccessor.HttpContext?.Session.GetString("jwt");
        if (!string.IsNullOrEmpty(token))
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<string?> LoginAsync(string username, string password)
    {
        var payload = JsonSerializer.Serialize(new { username, password });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _http.PostAsync("/api/auth/login", content);

        if (!response.IsSuccessStatusCode) return null;

        var body = await response.Content.ReadAsStringAsync();
        var doc = JsonSerializer.Deserialize<JsonElement>(body, JsonOptions);
        return doc.GetProperty("token").GetString();
    }

    public async Task<IEnumerable<CustomerListItemModel>?> GetCustomersAsync(string? search)
    {
        AttachToken();
        var url = "/api/customers";
        if (!string.IsNullOrEmpty(search))
            url += $"?search={Uri.EscapeDataString(search)}";

        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<IEnumerable<CustomerListItemModel>>(JsonOptions);
    }

    public async Task<CustomerDetailModel?> GetCustomerAsync(string id)
    {
        AttachToken();
        var response = await _http.GetAsync($"/api/customers/{Uri.EscapeDataString(id)}");
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<CustomerDetailModel>(JsonOptions);
    }
}
