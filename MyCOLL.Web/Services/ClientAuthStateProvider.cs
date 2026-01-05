using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage; // <--- O SEGREDO
using Microsoft.AspNetCore.Components.Authorization;
using MyCOLL.Shared.Interface;

namespace MyCOLL.Web.Services;

public class ClientAuthStateProvider : AuthenticationStateProvider, IAuthService
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _http;

    public ClientAuthStateProvider(ILocalStorageService localStorage, HttpClient http)
    {
        _localStorage = localStorage;
        _http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // 1. Tentar ler o token do navegador
        var token = await _localStorage.GetItemAsync<string>("authToken");

        // 2. Se não houver token, o utilizador é anónimo
        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        // 3. Se houver token, configurar o HttpClient para o usar sempre
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

        // 4. Ler as claims (dados) de dentro do token e avisar a App que estamos logados
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
    }
    
    // 2. Implemente o método Login
    public async Task Login(string token)
    {
        // Guardar no LocalStorage
        await _localStorage.SetItemAsync("authToken", token);
        
        // Configurar Header
        _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
        
        // Avisar a App que mudou o estado
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    // 3. Implemente o método Logout
    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        _http.DefaultRequestHeaders.Authorization = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    // Método auxiliar para ler o Token (igual ao que já deve ter no MAUI)
    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}