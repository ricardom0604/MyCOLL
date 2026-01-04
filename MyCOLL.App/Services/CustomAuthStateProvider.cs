using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Text.Json;
using MyCOLL.Shared.Interface;

namespace MyCOLL.App.Services;  

public class CustomAuthStateProvider : AuthenticationStateProvider, IAuthService
{
    private readonly HttpClient _httpClient;
    
    public CustomAuthStateProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Tenta ler o token guardado no telemóvel
        var token = await SecureStorage.GetAsync("authToken");

        var identity = new ClaimsIdentity();
        _httpClient.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                // Se existe, cria a identidade do user
                identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token);
            }
            catch
            {
                // Token inválido ou expirado
                SecureStorage.Remove("authToken");
            }
        }

        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);
    }

    // Método chamado quando o user faz Login
    public async Task Login(string token)
    {
        await SecureStorage.SetAsync("authToken", token);
        
        // Lógica de notificar o estado...
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }

    // Método chamado quando o user faz Logout
    public async Task Logout()
    {
        SecureStorage.Remove("authToken");
        
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        NotifyAuthenticationStateChanged(authState);
    }

    // Método auxiliar para ler o payload do JWT
    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}