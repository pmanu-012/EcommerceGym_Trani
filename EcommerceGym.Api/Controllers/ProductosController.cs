using EcommerceGym.Api.Data;
using EcommerceGym.Shared;
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

    [HttpGet]
    public async Task<ActionResult<List<ProductoDto>>> GetAll()
    {
        return await _db.Productos
            .Where(p => p.Activo)
            .OrderBy(p => p.Nombre)
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
}