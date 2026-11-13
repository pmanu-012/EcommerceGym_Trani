using EcommerceGym.Api.Data;
using EcommerceGym.Shared.DTOs;
using EcommerceGym.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGym.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]

// Se consideran ventas los pedidos que ya no están Pendientes
// (confirmados, enviados o entregados), y el importe es el total del pedido
public class EstadisticasController : ControllerBase
{
    private readonly AppDbContext _db;
    public EstadisticasController(AppDbContext db) => _db = db;

    // GET api/estadisticas?umbralStock=10
    [HttpGet]
    public async Task<ActionResult<EstadisticasDto>> Get(int umbralStock = 10)
    {
        umbralStock = Math.Clamp(umbralStock, 0, 1000);

        var hoy = DateTime.Now.Date;
        var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

        // Ventas = pedidos ya confirmados (confirmado, enviado o entregado)
        var ventas = _db.Pedidos.AsNoTracking().Where(p => p.Estado != EstadoPedido.Pendiente);
        var detalles = _db.DetallesPedido.AsNoTracking().Where(d => d.Pedido.Estado != EstadoPedido.Pendiente);

        var resultado = new EstadisticasDto
        {
            VentasHoy = await ventas.Where(p => p.Fecha >= hoy).SumAsync(p => (decimal?)p.Total) ?? 0m,
            VentasMes = await ventas.Where(p => p.Fecha >= inicioMes).SumAsync(p => (decimal?)p.Total) ?? 0m,

            MasVendidos = await detalles
                .GroupBy(d => new { d.IdProducto, d.Producto.Nombre })
                .OrderByDescending(g => g.Sum(d => d.Cantidad))
                .Take(5)
                .Select(g => new ProductoVendidoDto
                {
                    IdProducto = g.Key.IdProducto,
                    Nombre = g.Key.Nombre,
                    Unidades = g.Sum(d => d.Cantidad)
                })
                .ToListAsync(),

            CategoriaLider = await detalles
                .GroupBy(d => d.Producto.Subcategoria.Categoria.Nombre)
                .OrderByDescending(g => g.Sum(d => d.Cantidad))
                .Select(g => new CategoriaVentasDto { Nombre = g.Key, Unidades = g.Sum(d => d.Cantidad) })
                .FirstOrDefaultAsync(),

            MejorValorados = await _db.ResenasProducto.AsNoTracking()
                .Where(r => r.Producto.Activo)
                .GroupBy(r => new { r.IdProducto, r.Producto.Nombre })
                .OrderByDescending(g => g.Average(r => r.Calificacion))
                .ThenByDescending(g => g.Count())
                .Take(5)
                .Select(g => new ProductoValoradoDto
                {
                    IdProducto = g.Key.IdProducto,
                    Nombre = g.Key.Nombre,
                    Promedio = g.Average(r => r.Calificacion),
                    Cantidad = g.Count()
                })
                .ToListAsync(),

            StockBajo = await _db.Productos.AsNoTracking()
                .Where(p => p.Activo && p.Stock <= umbralStock)
                .OrderBy(p => p.Stock).ThenBy(p => p.Nombre)
                .Take(20)
                .Select(p => new ProductoStockDto { IdProducto = p.IdProducto, Nombre = p.Nombre, Stock = p.Stock })
                .ToListAsync()
        };

        return resultado;
    }
}