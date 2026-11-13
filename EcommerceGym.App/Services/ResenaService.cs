using System.Net.Http.Json;
using EcommerceGym.Shared.DTOs;

namespace EcommerceGym.App.Services;

public class ResenaService
{
    private const string SinConexion = "No se pudo conectar con el servidor. Verificá tu conexión.";

    private readonly HttpClient _http;
    public ResenaService(HttpClient http) => _http = http;

    // idProducto con valor = reseñas de ese producto; null = reseñas generales de la plataforma
    public async Task<ResenasDto> ObtenerAsync(int? idProducto, int limite = 3)
    {
        var url = idProducto.HasValue
            ? $"api/resenas-producto/producto/{idProducto}"
            : $"api/resenas-plataforma?limite={limite}";

        return await _http.GetFromJsonAsync<ResenasDto>(url) ?? new();
    }

    public async Task<List<MiResenaDto>> MisResenasAsync() =>
        await _http.GetFromJsonAsync<List<MiResenaDto>>("api/mis-resenas") ?? new();

    public Task<string?> CrearAsync(int? idProducto, ResenaFormDto dto) =>
        EjecutarAsync(() => idProducto.HasValue
            ? _http.PostAsJsonAsync($"api/resenas-producto/producto/{idProducto}", dto)
            : _http.PostAsJsonAsync("api/resenas-plataforma", dto));

    // tipo: "Producto" o "Plataforma"
    public Task<string?> EditarAsync(string tipo, int idResena, ResenaFormDto dto) =>
        EjecutarAsync(() => _http.PutAsJsonAsync($"{Ruta(tipo)}/{idResena}", dto));

    public Task<string?> EliminarAsync(string tipo, int idResena) =>
        EjecutarAsync(() => _http.DeleteAsync($"{Ruta(tipo)}/{idResena}"));

    private static string Ruta(string tipo) =>
        tipo == "Producto" ? "api/resenas-producto" : "api/resenas-plataforma";

    // Devuelve null si salió bien, o el mensaje de error
    private static async Task<string?> EjecutarAsync(Func<Task<HttpResponseMessage>> peticion)
    {
        try
        {
            var resp = await peticion();
            return resp.IsSuccessStatusCode ? null : await ApiErrores.LeerAsync(resp);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return SinConexion;
        }
    }
}