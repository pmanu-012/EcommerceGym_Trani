using System.Net.Http.Json;
using EcommerceGym.Shared.DTOs;

namespace EcommerceGym.App.Services;

public class CategoriaService
{
    private readonly HttpClient _http;
    private List<CategoriaDto>? _cache;

    public CategoriaService(HttpClient http) => _http = http;

    public async Task<List<CategoriaDto>> GetCategoriasAsync() =>
        _cache ??= await _http.GetFromJsonAsync<List<CategoriaDto>>("api/categorias") ?? new();
}