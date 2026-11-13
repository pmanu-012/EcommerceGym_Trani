using System.Security.Claims;
using EcommerceGym.Api.Data;
using EcommerceGym.Api.Models;
using EcommerceGym.Api.Services;
using EcommerceGym.Shared.DTOs;
using EcommerceGym.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGym.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class IngresosController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly AuditoriaService _auditoria;

    public IngresosController(AppDbContext db, AuditoriaService auditoria)
    {
        _db = db;
        _auditoria = auditoria;
    }

    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<IngresoResumenDto>>> Listar()
    {
        return await _db.IngresosMercaderia
            .AsNoTracking()
            .OrderByDescending(i => i.Fecha)
            .Take(100)
            .Select(i => new IngresoResumenDto
            {
                IdIngreso = i.IdIngreso,
                Fecha = i.Fecha,
                Usuario = i.Usuario.Nombre + " " + i.Usuario.Apellido,
                Productos = i.Detalles.Count,
                Unidades = i.Detalles.Sum(d => d.Cantidad)
            })
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<IngresoDto>> GetById(int id)
    {
        var ingreso = await CargarDtoAsync(id);
        if (ingreso is null)
            return NotFound(new { mensaje = "El ingreso no existe." });

        return ingreso;
    }

    [HttpPost]
    public async Task<ActionResult<IngresoDto>> Crear(NuevoIngresoDto dto)
    {
        if (dto.Items.Count == 0)
            return BadRequest(new { mensaje = "El ingreso debe incluir al menos un producto." });

        // Si un producto se repite, se suman las cantidades
        var lineas = dto.Items
            .GroupBy(i => i.IdProducto)
            .Select(g => new { IdProducto = g.Key, Cantidad = g.Sum(i => i.Cantidad) })
            .ToList();

        var ids = lineas.Select(l => l.IdProducto).ToList();
        var productos = await _db.Productos
            .Where(p => ids.Contains(p.IdProducto) && p.Activo)
            .ToListAsync();

        if (productos.Count != ids.Count)
            return BadRequest(new { mensaje = "Alguno de los productos no existe o está dado de baja." });

        await using var transaccion = await _db.Database.BeginTransactionAsync();

        var ingreso = new IngresoMercaderia
        {
            Fecha = DateTime.Now,
            Observacion = string.IsNullOrWhiteSpace(dto.Observacion) ? null : dto.Observacion.Trim(),
            IdUsuario = UsuarioId,
            Detalles = lineas.Select(l => new DetalleIngreso
            {
                IdProducto = l.IdProducto,
                Cantidad = l.Cantidad
            }).ToList()
        };

        foreach (var l in lineas)
            productos.First(p => p.IdProducto == l.IdProducto).Stock += l.Cantidad;

        _db.IngresosMercaderia.Add(ingreso);
        await _db.SaveChangesAsync();

        // El Id del ingreso recién se conoce después del primer guardado
        _auditoria.Registrar(UsuarioId, AccionAuditoria.Crear, "Ingreso", ingreso.IdIngreso, null,
            new { ingreso.Observacion, Items = lineas });
        await _db.SaveChangesAsync();

        await transaccion.CommitAsync();

        return (await CargarDtoAsync(ingreso.IdIngreso))!;
    }

    private Task<IngresoDto?> CargarDtoAsync(int id) =>
        _db.IngresosMercaderia
            .AsNoTracking()
            .Where(i => i.IdIngreso == id)
            .Select(i => new IngresoDto
            {
                IdIngreso = i.IdIngreso,
                Fecha = i.Fecha,
                Observacion = i.Observacion,
                Usuario = i.Usuario.Nombre + " " + i.Usuario.Apellido,
                Items = i.Detalles.Select(d => new IngresoItemDto
                {
                    IdProducto = d.IdProducto,
                    Nombre = d.Producto.Nombre,
                    Cantidad = d.Cantidad
                }).ToList()
            })
            .FirstOrDefaultAsync();
}