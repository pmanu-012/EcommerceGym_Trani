using System.Net;
using System.Net.Http.Json;
using EcommerceGym.Shared.DTOs;

namespace EcommerceGym.App.Services;

public class PedidoService
{
    private readonly HttpClient _http;
    public PedidoService(HttpClient http) => _http = http;

    public async Task<(PedidoDto? Pedido, string? Error)> CrearAsync(CheckoutDto dto)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/pedidos", dto);
            if (!resp.IsSuccessStatusCode)
                return (null, await ApiErrores.LeerAsync(resp));

            return (await resp.Content.ReadFromJsonAsync<PedidoDto>(), null);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return (null, "No se pudo conectar con el servidor. Verificá tu conexión.");
        }
    }

    public async Task<List<PedidoResumenDto>> MisPedidosAsync() =>
        await _http.GetFromJsonAsync<List<PedidoResumenDto>>("api/pedidos/mis-pedidos") ?? new();

    public async Task<PedidoDto?> GetPedidoAsync(int id)
    {
        var resp = await _http.GetAsync($"api/pedidos/{id}");
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<PedidoDto>();
    }
}