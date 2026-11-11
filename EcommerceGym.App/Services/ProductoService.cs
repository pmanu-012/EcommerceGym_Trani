using System.Net.Http.Json;
using EcommerceGym.Shared.DTOs;

namespace EcommerceGym.App.Services;

public class ProductoService
{
    private readonly HttpClient _http;
    public ProductoService(HttpClient http) => _http = http;

    public async Task<List<ProductoDto>> GetProductosAsync(
        int? idCategoria = null, int? idSubcategoria = null, string? buscar = null,
        bool soloOfertas = false, string? orden = null)
    {
        var qs = new List<string>();
        if (idCategoria.HasValue) qs.Add($"idCategoria={idCategoria}");
        if (idSubcategoria.HasValue) qs.Add($"idSubcategoria={idSubcategoria}");
        if (!string.IsNullOrWhiteSpace(buscar)) qs.Add($"buscar={Uri.EscapeDataString(buscar)}");
        if (soloOfertas) qs.Add("soloOfertas=true");
        if (!string.IsNullOrWhiteSpace(orden)) qs.Add($"orden={orden}");

        var url = "api/productos" + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");
        return await _http.GetFromJsonAsync<List<ProductoDto>>(url) ?? new();
    }
}