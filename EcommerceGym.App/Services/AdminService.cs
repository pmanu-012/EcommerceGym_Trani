using EcommerceGym.Shared.DTOs;
using EcommerceGym.Shared.Enums;
using System.Net.Http.Json;

namespace EcommerceGym.App.Services;

public class AdminService
{
    private const string SinConexion = "No se pudo conectar con el servidor. Verificá tu conexión.";

    private readonly HttpClient _http;
    private List<OpcionDto>? _disciplinas;
    private List<OpcionDto>? _grupos;

    public AdminService(HttpClient http) => _http = http;

    // ---------- Pedidos ----------
    public async Task<List<PedidoAdminDto>> PedidosAsync(string? buscar = null)
    {
        var url = "api/pedidos" +
                  (string.IsNullOrWhiteSpace(buscar) ? "" : $"?buscar={Uri.EscapeDataString(buscar)}");
        return await _http.GetFromJsonAsync<List<PedidoAdminDto>>(url) ?? new();
    }

    public async Task<(PedidoDto? Pedido, string? Error)> CambiarEstadoAsync(int id, EstadoPedido estado)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync($"api/pedidos/{id}/estado", new CambiarEstadoDto { Estado = estado });
            if (!resp.IsSuccessStatusCode) return (null, await ApiErrores.LeerAsync(resp));
            return (await resp.Content.ReadFromJsonAsync<PedidoDto>(), null);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return (null, SinConexion);
        }
    }

    // ---------- Productos ----------
    public async Task<ProductoFormDto?> GetProductoFormAsync(int id)
    {
        var resp = await _http.GetAsync($"api/productos/{id}/edicion");
        if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<ProductoFormDto>();
    }

    public Task<string?> CrearProductoAsync(ProductoFormDto dto) =>
        EjecutarAsync(() => _http.PostAsJsonAsync("api/productos", dto));

    public Task<string?> ActualizarProductoAsync(int id, ProductoFormDto dto) =>
        EjecutarAsync(() => _http.PutAsJsonAsync($"api/productos/{id}", dto));

    public Task<string?> EliminarProductoAsync(int id) =>
        EjecutarAsync(() => _http.DeleteAsync($"api/productos/{id}"));

    public async Task<List<OpcionDto>> DisciplinasAsync() =>
        _disciplinas ??= await _http.GetFromJsonAsync<List<OpcionDto>>("api/catalogos/disciplinas") ?? new();

    public async Task<List<OpcionDto>> GruposMuscularesAsync() =>
        _grupos ??= await _http.GetFromJsonAsync<List<OpcionDto>>("api/catalogos/grupos-musculares") ?? new();

     // ---------- Estadísticas ----------
    public async Task<EstadisticasDto> EstadisticasAsync(int umbralStock) =>
        await _http.GetFromJsonAsync<EstadisticasDto>($"api/estadisticas?umbralStock={umbralStock}") ?? new();

     // ---------- Usuarios ----------
    public async Task<List<UsuarioDto>> UsuariosAsync(string? buscar = null)
    {
        var url = "api/usuarios" +
        (string.IsNullOrWhiteSpace(buscar) ? "" : $"?buscar={Uri.EscapeDataString(buscar)}");
        return await _http.GetFromJsonAsync<List<UsuarioDto>>(url) ?? new ();
    }

    public async Task<UsuarioDto?> GetUsuarioAsync(int id)
    {
        var resp = await _http.GetAsync($"api/usuarios/{id}");
        if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<UsuarioDto>();
    }

    public Task<string?> EditarUsuarioAsync(int id, UsuarioEditDto dto) =>
        EjecutarAsync(() => _http.PutAsJsonAsync($"api/usuarios/{id}", dto));

    public Task<string?> DarDeBajaAsync(int id) =>
        EjecutarAsync(() => _http.DeleteAsync($"api/usuarios/{id}"));

    // ---------- Ingresos de mercadería ----------
    public async Task<List<IngresoResumenDto>> IngresosAsync() =>
        await _http.GetFromJsonAsync<List<IngresoResumenDto>>("api/ingresos") ?? new();

    public async Task<IngresoDto?> GetIngresoAsync(int id)
    {
        var resp = await _http.GetAsync($"api/ingresos/{id}");
        if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<IngresoDto>();
    }

    public Task<string?> CrearIngresoAsync(NuevoIngresoDto dto) =>
        EjecutarAsync(() => _http.PostAsJsonAsync("api/ingresos", dto));

    // ---------- Auditoría ----------
    public async Task<List<AuditoriaDto>> AuditoriaAsync(string? entidad = null, AccionAuditoria? accion = null, int limite = 50)
    {
        var qs = new List<string> { $"limite={limite}" };
        if (!string.IsNullOrWhiteSpace(entidad)) qs.Add($"entidad={Uri.EscapeDataString(entidad)}");
        if (accion.HasValue) qs.Add($"accion={accion}");
        return await _http.GetFromJsonAsync<List<AuditoriaDto>>("api/auditoria?" + string.Join("&", qs)) ?? new();
    }

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