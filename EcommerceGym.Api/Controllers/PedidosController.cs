using System.Security.Claims;
using EcommerceGym.Api.Data;
using EcommerceGym.Api.Models;
using EcommerceGym.Shared;
using EcommerceGym.Shared.DTOs;
using EcommerceGym.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGym.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PedidosController : ControllerBase
{
    private readonly AppDbContext _db;
    public PedidosController(AppDbContext db) => _db = db;

    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Checkout: convierte el carrito del cliente en un pedido
    [HttpPost]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<PedidoDto>> Crear(CheckoutDto dto)
    {
        var items = await _db.CarritoItems
            .Include(c => c.Producto)
            .Where(c => c.IdUsuario == UsuarioId)
            .ToListAsync();

        if (items.Count == 0)
            return BadRequest(new { mensaje = "Tu carrito está vacío." });

        foreach (var item in items)
        {
            if (!item.Producto.Activo)
                return BadRequest(new { mensaje = $"\"{item.Producto.Nombre}\" ya no está disponible." });

            if (item.Cantidad > item.Producto.Stock)
                return BadRequest(new
                {
                    mensaje = $"Stock insuficiente para \"{item.Producto.Nombre}\". Disponible: {item.Producto.Stock}."
                });
        }

        var pedido = new Pedido
        {
            IdUsuario = UsuarioId,
            Fecha = DateTime.Now,
            Estado = EstadoPedido.Pendiente,
            Calle = dto.Calle.Trim(),
            Numero = dto.Numero.Trim(),
            Ciudad = dto.Ciudad.Trim(),
            Provincia = dto.Provincia.Trim(),
            CodigoPostal = dto.CodigoPostal.Trim(),
            CostoEnvio = Tienda.CostoEnvio,
            Detalles = items.Select(i => new DetallePedido
            {
                Producto = i.Producto,
                Cantidad = i.Cantidad,
                // Se guarda el precio vigente al momento de la compra
                PrecioUnitario = i.Producto.PrecioOferta ?? i.Producto.Precio
            }).ToList()
        };

        pedido.Total = pedido.Detalles.Sum(d => d.PrecioUnitario * d.Cantidad) + pedido.CostoEnvio;

        _db.Pedidos.Add(pedido);
        _db.CarritoItems.RemoveRange(items);

        // Un solo SaveChanges: EF lo ejecuta en una transacción, o se guarda todo o nada
        await _db.SaveChangesAsync();

        return ToDto(pedido);
    }

    [HttpGet("mis-pedidos")]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<List<PedidoResumenDto>>> MisPedidos()
    {
        return await _db.Pedidos
            .AsNoTracking()
            .Where(p => p.IdUsuario == UsuarioId)
            .OrderByDescending(p => p.Fecha)
            .Select(p => new PedidoResumenDto
            {
                IdPedido = p.IdPedido,
                Fecha = p.Fecha,
                Estado = p.Estado,
                Total = p.Total
            })
            .ToListAsync();
    }

    // El cliente ve solo sus pedidos; el administrador puede ver cualquiera
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PedidoDto>> GetById(int id)
    {
        var esAdmin = User.IsInRole("Administrador");

        var pedido = await _db.Pedidos
            .AsNoTracking()
            .Include(p => p.Detalles).ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(p => p.IdPedido == id && (esAdmin || p.IdUsuario == UsuarioId));

        if (pedido is null)
            return NotFound(new { mensaje = "El pedido no existe." });

        return ToDto(pedido);
    }

    private static PedidoDto ToDto(Pedido p) => new()
    {
        IdPedido = p.IdPedido,
        Fecha = p.Fecha,
        Estado = p.Estado,
        Total = p.Total,
        CostoEnvio = p.CostoEnvio,
        Calle = p.Calle,
        Numero = p.Numero,
        Ciudad = p.Ciudad,
        Provincia = p.Provincia,
        CodigoPostal = p.CodigoPostal,
        Items = p.Detalles.Select(d => new PedidoItemDto
        {
            IdProducto = d.IdProducto,
            Nombre = d.Producto.Nombre,
            Cantidad = d.Cantidad,
            PrecioUnitario = d.PrecioUnitario
        }).ToList()
    };
}