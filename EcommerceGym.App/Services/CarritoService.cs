using System.Net.Http.Json;
using EcommerceGym.Shared.DTOs;

namespace EcommerceGym.App.Services;

public class CarritoService
{
    private readonly HttpClient _http;
    public CarritoService(HttpClient http) => _http = http;

    public CarritoDto Actual { get; private set; } = new();
    public int CantidadTotal => Actual.CantidadTotal;
    public event Action? Cambio;

    public async Task<CarritoDto> ObtenerAsync()
    {
        var carrito = await _http.GetFromJsonAsync<CarritoDto>("api/carrito") ?? new();
        Establecer(carrito);
        return carrito;
    }

    // Lo usa la barra inferior: sin sesión de cliente, el contador queda en 0
    public async Task RefrescarContadorAsync()
    {
        try { await ObtenerAsync(); }
        catch (Exception) { Establecer(new()); }
    }

    public Task<string?> AgregarAsync(int idProducto, int cantidad) =>
        EjecutarAsync(() => _http.PostAsJsonAsync("api/carrito",
            new AgregarCarritoDto { IdProducto = idProducto, Cantidad = cantidad }));

    public Task<string?> ActualizarAsync(int idItem, int cantidad) =>
        EjecutarAsync(() => _http.PutAsJsonAsync($"api/carrito/{idItem}",
            new ActualizarCarritoDto { Cantidad = cantidad }));

    public Task<string?> EliminarAsync(int idItem) =>
        EjecutarAsync(() => _http.DeleteAsync($"api/carrito/{idItem}"));

    // Devuelve null si salió bien, o el mensaje de error
    private async Task<string?> EjecutarAsync(Func<Task<HttpResponseMessage>> peticion)
    {
        try
        {
            var resp = await peticion();
            if (!resp.IsSuccessStatusCode) return await ApiErrores.LeerAsync(resp);
            await ObtenerAsync();
            return null;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return "No se pudo conectar con el servidor. Verificá tu conexión.";
        }
    }

    private void Establecer(CarritoDto carrito)
    {
        Actual = carrito;
        Cambio?.Invoke();
    }
}