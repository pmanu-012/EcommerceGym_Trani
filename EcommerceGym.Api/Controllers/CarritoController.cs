using System.Security.Claims;
using EcommerceGym.Api.Data;
using EcommerceGym.Api.Models;
using EcommerceGym.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGym.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Cliente")]
public class CarritoController : ControllerBase
{
    private readonly AppDbContext _db;
    public CarritoController(AppDbContext db) => _db = db;

    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<CarritoDto>> Get()
    {
        var items = await _db.CarritoItems
            .AsNoTracking()
            .Where(c => c.IdUsuario == UsuarioId)
            .OrderBy(c => c.IdCarritoItem)
            .Select(c => new CarritoItemDto
            {
                IdCarritoItem = c.IdCarritoItem,
                IdProducto = c.IdProducto,
                Nombre = c.Producto.Nombre,
                ImagenUrl = c.Producto.ImagenUrl,
                PrecioUnitario = c.Producto.PrecioOferta ?? c.Producto.Precio,
                Cantidad = c.Cantidad,
                Stock = c.Producto.Stock
            })
            .ToListAsync();

        return new CarritoDto { Items = items };
    }

    [HttpPost]
    public async Task<ActionResult> Agregar(AgregarCarritoDto dto)
    {
        var producto = await _db.Productos
            .FirstOrDefaultAsync(p => p.IdProducto == dto.IdProducto && p.Activo);

        if (producto is null)
            return NotFound(new { mensaje = "El producto no existe." });

        var item = await _db.CarritoItems
            .FirstOrDefaultAsync(c => c.IdUsuario == UsuarioId && c.IdProducto == dto.IdProducto);

        var cantidadFinal = (item?.Cantidad ?? 0) + dto.Cantidad;

        if (cantidadFinal > producto.Stock)
            return BadRequest(new { mensaje = $"Stock insuficiente. Disponible: {producto.Stock}." });

        if (item is null)
            _db.CarritoItems.Add(new CarritoItem
            {
                IdUsuario = UsuarioId,
                IdProducto = dto.IdProducto,
                Cantidad = dto.Cantidad
            });
        else
            item.Cantidad = cantidadFinal;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Actualizar(int id, ActualizarCarritoDto dto)
    {
        var item = await _db.CarritoItems
            .Include(c => c.Producto)
            .FirstOrDefaultAsync(c => c.IdCarritoItem == id && c.IdUsuario == UsuarioId);

        if (item is null)
            return NotFound(new { mensaje = "El ítem no existe en tu carrito." });

        if (dto.Cantidad > item.Producto.Stock)
            return BadRequest(new { mensaje = $"Stock insuficiente. Disponible: {item.Producto.Stock}." });

        item.Cantidad = dto.Cantidad;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Eliminar(int id)
    {
        var item = await _db.CarritoItems
            .FirstOrDefaultAsync(c => c.IdCarritoItem == id && c.IdUsuario == UsuarioId);

        if (item is null)
            return NotFound(new { mensaje = "El ítem no existe en tu carrito." });

        _db.CarritoItems.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}