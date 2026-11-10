using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using EcommerceGym.Shared.DTOs;
using Microsoft.AspNetCore.Components.Authorization;

namespace EcommerceGym.App.Services;

// maneja la sesión y además le dice a Blazor quién es el usuario y qué rol tiene
public class AuthService : AuthenticationStateProvider
{
    private readonly HttpClient _http;
    private readonly TokenStore _store;
    private Task? _init;

    public AuthService(HttpClient http, TokenStore store)
    {
        _http = http;
        _store = store;
    }

    public AuthResponseDto? Sesion { get; private set; }
    public bool EstaAutenticado => Sesion is not null;
    public bool EsAdmin => Sesion?.Rol == "Administrador";

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await (_init ??= CargarSesionGuardadaAsync());
        return new AuthenticationState(CrearPrincipal());
    }

    public Task<string?> LoginAsync(LoginDto dto) => AutenticarAsync("api/auth/login", dto);

    public Task<string?> RegistroAsync(RegistroDto dto) => AutenticarAsync("api/auth/registro", dto);

    public Task LogoutAsync()
    {
        _store.Borrar();
        Sesion = null;
        _http.DefaultRequestHeaders.Authorization = null;
        Notificar();
        return Task.CompletedTask;
    }

    // Devuelve null si salió bien, o el mensaje de error
    private async Task<string?> AutenticarAsync<T>(string url, T dto)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync(url, dto);
            if (!resp.IsSuccessStatusCode)
                return await ApiErrores.LeerAsync(resp);

            var sesion = await resp.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (sesion is null) return "La respuesta del servidor no es válida.";

            await _store.GuardarAsync(JsonSerializer.Serialize(sesion));
            AplicarSesion(sesion);
            Notificar();
            return null;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return "No se pudo conectar con el servidor. Verificá que la API esté en ejecución.";
        }
    }

    private async Task CargarSesionGuardadaAsync()
    {
        var json = await _store.LeerAsync();
        if (string.IsNullOrEmpty(json)) return;

        try
        {
            var sesion = JsonSerializer.Deserialize<AuthResponseDto>(json);
            if (sesion is not null) AplicarSesion(sesion);
        }
        catch (JsonException)
        {
            _store.Borrar();
        }
    }

    private void AplicarSesion(AuthResponseDto sesion)
    {
        Sesion = sesion;
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sesion.Token);
    }

    private void Notificar() =>
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(CrearPrincipal())));

    private ClaimsPrincipal CrearPrincipal()
    {
        if (Sesion is null)
            return new ClaimsPrincipal(new ClaimsIdentity());

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Sesion.IdUsuario.ToString()),
            new Claim(ClaimTypes.Name, $"{Sesion.Nombre} {Sesion.Apellido}"),
            new Claim(ClaimTypes.Email, Sesion.Email),
            new Claim(ClaimTypes.Role, Sesion.Rol)
        }, "jwt");

        return new ClaimsPrincipal(identity);
    }
}