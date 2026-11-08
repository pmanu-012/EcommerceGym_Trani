using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http.Json;
using EcommerceGym.Shared.DTOs;

namespace EcommerceGym.App.Services;

public class ProductoService
{
    private readonly HttpClient _http;
    public ProductoService(HttpClient http) => _http = http;

    public async Task<List<ProductoDto>> GetProductosAsync()
        => await _http.GetFromJsonAsync<List<ProductoDto>>("api/productos") ?? new();
}