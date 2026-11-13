using System.Linq.Expressions;
using System.Security.Claims;
using EcommerceGym.Api.Data;
using EcommerceGym.Api.Models;
using EcommerceGym.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGym.Api.Controllers;

[ApiController]
[Route("api/resenas-plataforma")]
// Es casi igual a la de productos, sin producto. El listado devuelve las más recientes, para los testimonios del Home.
public class ResenasPlataformaController : ControllerBase
{
    private readonly AppDbContext _db;
    public ResenasPlataformaController(AppDbContext db) => _db = db;

    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private int? UsuarioOpcional =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    private static readonly Expression<Func<ResenaPlataforma, ResenaDto>> ADto = r => new ResenaDto
    {
        IdResena = r.IdResena,
        IdUsuario = r.IdUsuario,
        Usuario = r.Usuario.Nombre + " " + r.Usuario.Apellido.Substring(0, 1) + ".",
        Calificacion = r.Calificacion,
        Comentario = r.Comentario,
        Fecha = r.Fecha
    };

    // GET api/resenas-plataforma?limite=5
    [HttpGet]
    public async Task<ActionResult<ResenasDto>> Listar(int limite = 5)
    {
        limite = Math.Clamp(limite, 1, 50);

        var consulta = _db.ResenasPlataforma.AsNoTracking();

        var cantidad = await consulta.CountAsync();
        var promedio = cantidad == 0 ? 0 : await consulta.AverageAsync(r => (double)r.Calificacion);

        var items = await consulta
            .OrderByDescending(r => r.Fecha)
            .Take(limite)
            .Select(ADto)
            .ToListAsync();

        ResenaDto? mia = null;
        var yo = UsuarioOpcional;
        if (yo.HasValue)
            mia = await consulta.Where(r => r.IdUsuario == yo.Value).Select(ADto).FirstOrDefaultAsync();

        return new ResenasDto { Promedio = promedio, Cantidad = cantidad, Items = items, MiResena = mia };
    }

    [HttpPost]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<ResenaDto>> Crear(ResenaFormDto dto)
    {
        if (await _db.ResenasPlataforma.AnyAsync(r => r.IdUsuario == UsuarioId))
            return Conflict(new { mensaje = "Ya dejaste tu reseña general. Podés editarla." });

        var resena = new ResenaPlataforma
        {
            IdUsuario = UsuarioId,
            Calificacion = dto.Calificacion,
            Comentario = dto.Comentario.Trim(),
            Fecha = DateTime.Now
        };

        _db.ResenasPlataforma.Add(resena);
        await _db.SaveChangesAsync();

        return await _db.ResenasPlataforma.AsNoTracking()
            .Where(r => r.IdResena == resena.IdResena).Select(ADto).FirstAsync();
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<ResenaDto>> Editar(int id, ResenaFormDto dto)
    {
        var resena = await _db.ResenasPlataforma
            .FirstOrDefaultAsync(r => r.IdResena == id && r.IdUsuario == UsuarioId);

        if (resena is null)
            return NotFound(new { mensaje = "La reseña no existe." });

        resena.Calificacion = dto.Calificacion;
        resena.Comentario = dto.Comentario.Trim();
        await _db.SaveChangesAsync();

        return await _db.ResenasPlataforma.AsNoTracking()
            .Where(r => r.IdResena == id).Select(ADto).FirstAsync();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult> Eliminar(int id)
    {
        var resena = await _db.ResenasPlataforma
            .FirstOrDefaultAsync(r => r.IdResena == id && r.IdUsuario == UsuarioId);

        if (resena is null)
            return NotFound(new { mensaje = "La reseña no existe." });

        _db.ResenasPlataforma.Remove(resena);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}