using System.Linq.Expressions;
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
[Route("api/resenas-producto")]
public class ResenasProductoController : ControllerBase
{
    private readonly AppDbContext _db;
    public ResenasProductoController(AppDbContext db) => _db = db;

    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Los endpoints públicos reconocen al usuario si envió su token
    private int? UsuarioOpcional =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    // Se muestra "Nombre A." para no exponer el apellido completo
    private static readonly Expression<Func<ResenaProducto, ResenaDto>> ADto = r => new ResenaDto
    {
        IdResena = r.IdResena,
        IdUsuario = r.IdUsuario,
        Usuario = r.Usuario.Nombre + " " + r.Usuario.Apellido.Substring(0, 1) + ".",
        Calificacion = r.Calificacion,
        Comentario = r.Comentario,
        Fecha = r.Fecha
    };

    [HttpGet("producto/{idProducto:int}")]
    public async Task<ActionResult<ResenasDto>> Listar(int idProducto)
    {
        if (!await _db.Productos.AnyAsync(p => p.IdProducto == idProducto && p.Activo))
            return NotFound(new { mensaje = "El producto no existe." });

        var consulta = _db.ResenasProducto.AsNoTracking().Where(r => r.IdProducto == idProducto);

        var cantidad = await consulta.CountAsync();
        var promedio = cantidad == 0 ? 0 : await consulta.AverageAsync(r => (double)r.Calificacion);

        var items = await consulta
            .OrderByDescending(r => r.Fecha)
            .Take(50)
            .Select(ADto)
            .ToListAsync();

        ResenaDto? mia = null;
        var yo = UsuarioOpcional;
        if (yo.HasValue)
            mia = await consulta.Where(r => r.IdUsuario == yo.Value).Select(ADto).FirstOrDefaultAsync();

        return new ResenasDto { Promedio = promedio, Cantidad = cantidad, Items = items, MiResena = mia };
    }

    [HttpPost("producto/{idProducto:int}")]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<ResenaDto>> Crear(int idProducto, ResenaFormDto dto)
    {
        if (!await _db.Productos.AnyAsync(p => p.IdProducto == idProducto && p.Activo))
            return NotFound(new { mensaje = "El producto no existe." });

        if (await _db.ResenasProducto.AnyAsync(r => r.IdProducto == idProducto && r.IdUsuario == UsuarioId))
            return Conflict(new { mensaje = "Ya reseñaste este producto. Podés editar tu reseña." });

        if (Tienda.ResenasSoloCompradores &&
            !await _db.DetallesPedido.AnyAsync(d => d.IdProducto == idProducto &&
                d.Pedido.IdUsuario == UsuarioId && d.Pedido.Estado != EstadoPedido.Pendiente))
            return BadRequest(new { mensaje = "Solo podés reseñar los productos que compraste." });

        var resena = new ResenaProducto
        {
            IdProducto = idProducto,
            IdUsuario = UsuarioId,
            Calificacion = dto.Calificacion,
            Comentario = dto.Comentario.Trim(),
            Fecha = DateTime.Now
        };

        _db.ResenasProducto.Add(resena);
        await _db.SaveChangesAsync();

        return await _db.ResenasProducto.AsNoTracking()
            .Where(r => r.IdResena == resena.IdResena).Select(ADto).FirstAsync();
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<ResenaDto>> Editar(int id, ResenaFormDto dto)
    {
        var resena = await _db.ResenasProducto
            .FirstOrDefaultAsync(r => r.IdResena == id && r.IdUsuario == UsuarioId);

        if (resena is null)
            return NotFound(new { mensaje = "La reseña no existe." });

        resena.Calificacion = dto.Calificacion;
        resena.Comentario = dto.Comentario.Trim();
        await _db.SaveChangesAsync();

        return await _db.ResenasProducto.AsNoTracking()
            .Where(r => r.IdResena == id).Select(ADto).FirstAsync();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult> Eliminar(int id)
    {
        var resena = await _db.ResenasProducto
            .FirstOrDefaultAsync(r => r.IdResena == id && r.IdUsuario == UsuarioId);

        if (resena is null)
            return NotFound(new { mensaje = "La reseña no existe." });

        _db.ResenasProducto.Remove(resena);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}