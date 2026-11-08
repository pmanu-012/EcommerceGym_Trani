using EcommerceGym.Api.Data;
using EcommerceGym.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGym.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly AppDbContext _db;
    public ProductosController(AppDbContext db) => _db = db;

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
}