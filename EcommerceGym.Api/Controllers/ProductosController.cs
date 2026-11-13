using EcommerceGym.Api.Data;
using EcommerceGym.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using EcommerceGym.Api.Models;
using EcommerceGym.Api.Services;
using EcommerceGym.Shared.Enums;
using Microsoft.AspNetCore.Authorization;

namespace EcommerceGym.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly AuditoriaService _auditoria;
    public ProductosController(AppDbContext db, AuditoriaService auditoria)
    {
        _db = db;
        _auditoria = auditoria;
    }

    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

// GET api/productos?idCategoria=1&idSubcategoria=2&buscar=banco&soloOfertas=true&orden=precio_asc
[HttpGet]
    public async Task<ActionResult<List<ProductoDto>>> GetAll(
        int? idCategoria, int? idSubcategoria, string? buscar,
        bool soloOfertas = false, string? orden = null)
    {
        var query = _db.Productos.AsNoTracking().Where(p => p.Activo);

        if (idCategoria.HasValue)
            query = query.Where(p => p.Subcategoria.IdCategoria == idCategoria.Value);

        if (idSubcategoria.HasValue)
            query = query.Where(p => p.IdSubcategoria == idSubcategoria.Value);

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var texto = buscar.Trim();
            query = query.Where(p => p.Nombre.Contains(texto));
        }

        if (soloOfertas)
            query = query.Where(p => p.PrecioOferta != null);

        query = orden switch
        {
            "precio_asc" => query.OrderBy(p => p.PrecioOferta ?? p.Precio),
            "precio_desc" => query.OrderByDescending(p => p.PrecioOferta ?? p.Precio),
            "recientes" => query.OrderByDescending(p => p.IdProducto),
            _ => query.OrderBy(p => p.Nombre)
        };

        return await query
            .Select(p => new ProductoDto
            {
                IdProducto = p.IdProducto,
                Nombre = p.Nombre,
                Precio = p.Precio,
                PrecioOferta = p.PrecioOferta,
                Stock = p.Stock,
                ImagenUrl = p.ImagenUrl,
                Categoria = p.Subcategoria.Categoria.Nombre,
                Subcategoria = p.Subcategoria.Nombre
            })
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductoDetalleDto>> GetById(int id)
    {
        var p = await _db.Productos
            .AsNoTracking()
            .Include(x => x.Subcategoria).ThenInclude(s => s.Categoria)
            .Include(x => x.ProductoDisciplinas).ThenInclude(pd => pd.Disciplina)
            .Include(x => x.ProductoGruposMusculares).ThenInclude(pg => pg.GrupoMuscular)
            .FirstOrDefaultAsync(x => x.IdProducto == id && x.Activo);

        if (p is null)
            return NotFound(new { mensaje = "El producto no existe." });

        return new ProductoDetalleDto
        {
            IdProducto = p.IdProducto,
            Nombre = p.Nombre,
            Precio = p.Precio,
            PrecioOferta = p.PrecioOferta,
            Stock = p.Stock,
            ImagenUrl = p.ImagenUrl,
            Categoria = p.Subcategoria.Categoria.Nombre,
            Subcategoria = p.Subcategoria.Nombre,
            Uso = p.Uso?.ToString(),
            Material = p.Material?.ToString(),
            Disciplinas = p.ProductoDisciplinas.Select(d => d.Disciplina.Nombre).ToList(),
            GruposMusculares = p.ProductoGruposMusculares.Select(g => g.GrupoMuscular.Nombre).ToList()
        };
    }
    // Datos para precargar el formulario de edición (administrador)
    [HttpGet("{id:int}/edicion")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ProductoFormDto>> GetParaEdicion(int id)
    {
        var p = await _db.Productos
            .AsNoTracking()
            .Include(x => x.ProductoDisciplinas)
            .Include(x => x.ProductoGruposMusculares)
            .FirstOrDefaultAsync(x => x.IdProducto == id && x.Activo);

        if (p is null)
            return NotFound(new { mensaje = "El producto no existe." });

        return new ProductoFormDto
        {
            IdProducto = p.IdProducto,
            Nombre = p.Nombre,
            Precio = p.Precio,
            PrecioOferta = p.PrecioOferta,
            Stock = p.Stock,
            ImagenUrl = p.ImagenUrl,
            Uso = p.Uso,
            Material = p.Material,
            IdSubcategoria = p.IdSubcategoria,
            IdDisciplinas = p.ProductoDisciplinas.Select(d => d.IdDisciplina).ToList(),
            IdGruposMusculares = p.ProductoGruposMusculares.Select(g => g.IdGrupoMuscular).ToList()
        };
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ProductoFormDto>> Crear(ProductoFormDto dto)
    {
        var error = await ValidarAsync(dto);
        if (error is not null)
            return BadRequest(new { mensaje = error });

        var p = new Producto { Activo = true };
        Aplicar(p, dto);
        Sincronizar(p, dto);
        _db.Productos.Add(p);
        await _db.SaveChangesAsync();

        // El Id del producto recién se conoce después del primer guardado
        _auditoria.Registrar(UsuarioId, AccionAuditoria.Crear, "Producto", p.IdProducto, null, Snapshot(p));
        await _db.SaveChangesAsync();

        dto.IdProducto = p.IdProducto;
        return dto;
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> Actualizar(int id, ProductoFormDto dto)
    {
        var p = await _db.Productos
            .Include(x => x.ProductoDisciplinas)
            .Include(x => x.ProductoGruposMusculares)
            .FirstOrDefaultAsync(x => x.IdProducto == id && x.Activo);

        if (p is null)
            return NotFound(new { mensaje = "El producto no existe." });

        var error = await ValidarAsync(dto);
        if (error is not null)
            return BadRequest(new { mensaje = error });

        var anterior = Snapshot(p);
        Aplicar(p, dto);
        Sincronizar(p, dto);
        _auditoria.Registrar(UsuarioId, AccionAuditoria.Modificar, "Producto", p.IdProducto, anterior, Snapshot(p));

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // Baja lógica: el producto deja de mostrarse, pero se conserva el historial de pedidos
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> Eliminar(int id)
    {
        var p = await _db.Productos.FirstOrDefaultAsync(x => x.IdProducto == id && x.Activo);
        if (p is null)
            return NotFound(new { mensaje = "El producto no existe." });

        var anterior = Snapshot(p);
        p.Activo = false;

        // Se quita de los carritos de los clientes
        _db.CarritoItems.RemoveRange(await _db.CarritoItems.Where(c => c.IdProducto == id).ToListAsync());

        _auditoria.Registrar(UsuarioId, AccionAuditoria.Eliminar, "Producto", id, anterior, Snapshot(p));
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private async Task<string?> ValidarAsync(ProductoFormDto dto)
    {
        if (dto.PrecioOferta.HasValue && dto.PrecioOferta >= dto.Precio)
            return "El precio de oferta debe ser menor al precio normal.";

        if (!string.IsNullOrWhiteSpace(dto.ImagenUrl) &&
            !dto.ImagenUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !dto.ImagenUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return "La URL de la imagen debe comenzar con http:// o https://.";

        if (!await _db.Subcategorias.AnyAsync(s => s.IdSubcategoria == dto.IdSubcategoria))
            return "La subcategoría seleccionada no existe.";

        var disciplinas = dto.IdDisciplinas.Distinct().ToList();
        if (await _db.Disciplinas.CountAsync(d => disciplinas.Contains(d.IdDisciplina)) != disciplinas.Count)
            return "Alguna de las disciplinas seleccionadas no existe.";

        var grupos = dto.IdGruposMusculares.Distinct().ToList();
        if (await _db.GruposMusculares.CountAsync(g => grupos.Contains(g.IdGrupoMuscular)) != grupos.Count)
            return "Alguno de los grupos musculares seleccionados no existe.";

        return null;
    }

    private static void Aplicar(Producto p, ProductoFormDto dto)
    {
        p.Nombre = dto.Nombre.Trim();
        p.Precio = dto.Precio;
        p.PrecioOferta = dto.PrecioOferta;
        p.Stock = dto.Stock;
        p.ImagenUrl = dto.ImagenUrl?.Trim() ?? string.Empty;
        p.Uso = dto.Uso;
        p.Material = dto.Material;
        p.IdSubcategoria = dto.IdSubcategoria;
    }

    // Deja las relaciones N:M exactamente como las indica el formulario (solo agrega y quita las diferencias)
    private void Sincronizar(Producto p, ProductoFormDto dto)
    {
        var disciplinas = dto.IdDisciplinas.Distinct().ToHashSet();
        _db.ProductoDisciplinas.RemoveRange(p.ProductoDisciplinas.Where(x => !disciplinas.Contains(x.IdDisciplina)).ToList());
        var disciplinasActuales = p.ProductoDisciplinas.Select(x => x.IdDisciplina).ToHashSet();
        foreach (var idDisciplina in disciplinas.Where(i => !disciplinasActuales.Contains(i)))
            p.ProductoDisciplinas.Add(new ProductoDisciplina { IdDisciplina = idDisciplina });

        var grupos = dto.IdGruposMusculares.Distinct().ToHashSet();
        _db.ProductoGruposMusculares.RemoveRange(p.ProductoGruposMusculares.Where(x => !grupos.Contains(x.IdGrupoMuscular)).ToList());
        var gruposActuales = p.ProductoGruposMusculares.Select(x => x.IdGrupoMuscular).ToHashSet();
        foreach (var idGrupo in grupos.Where(i => !gruposActuales.Contains(i)))
            p.ProductoGruposMusculares.Add(new ProductoGrupoMuscular { IdGrupoMuscular = idGrupo });
    }

    private static object Snapshot(Producto p) =>
        new { p.Nombre, p.Precio, p.PrecioOferta, p.Stock, p.IdSubcategoria, p.Activo };
}