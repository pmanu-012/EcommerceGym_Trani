using System.Security.Claims;
using EcommerceGym.Api.Data;
using EcommerceGym.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGym.Api.Controllers;

[ApiController]
[Route("api/mis-resenas")]
[Authorize(Roles = "Cliente")]
public class MisResenasController : ControllerBase
{
    private readonly AppDbContext _db;
    public MisResenasController(AppDbContext db) => _db = db;

    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<MiResenaDto>>> Get()
    {
        var deProductos = await _db.ResenasProducto
            .AsNoTracking()
            .Where(r => r.IdUsuario == UsuarioId)
            .Select(r => new MiResenaDto
            {
                Tipo = "Producto",
                IdResena = r.IdResena,
                IdProducto = r.IdProducto,
                Titulo = r.Producto.Nombre,
                Calificacion = r.Calificacion,
                Comentario = r.Comentario,
                Fecha = r.Fecha
            })
            .ToListAsync();

        var generales = await _db.ResenasPlataforma
            .AsNoTracking()
            .Where(r => r.IdUsuario == UsuarioId)
            .Select(r => new MiResenaDto
            {
                Tipo = "Plataforma",
                IdResena = r.IdResena,
                IdProducto = null,
                Titulo = "Reseña general",
                Calificacion = r.Calificacion,
                Comentario = r.Comentario,
                Fecha = r.Fecha
            })
            .ToListAsync();

        return deProductos.Concat(generales).OrderByDescending(r => r.Fecha).ToList();
    }
}