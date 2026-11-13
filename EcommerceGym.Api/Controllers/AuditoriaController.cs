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
public class AuditoriaController : ControllerBase
{
    private readonly AppDbContext _db;
    public AuditoriaController(AppDbContext db) => _db = db;

    // GET api/auditoria?entidad=Producto&accion=Modificar&limite=50
    [HttpGet]
    public async Task<ActionResult<List<AuditoriaDto>>> Listar(
        string? entidad, AccionAuditoria? accion, int limite = 50)
    {
        limite = Math.Clamp(limite, 1, 200);

        var query = _db.Auditorias.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(entidad))
            query = query.Where(a => a.Entidad == entidad);

        if (accion.HasValue)
            query = query.Where(a => a.Accion == accion.Value);

        return await query
            .OrderByDescending(a => a.Fecha)
            .ThenByDescending(a => a.IdAuditoria)
            .Take(limite)
            .Select(a => new AuditoriaDto
            {
                IdAuditoria = a.IdAuditoria,
                Fecha = a.Fecha,
                Accion = a.Accion,
                Entidad = a.Entidad,
                IdRegistro = a.IdRegistro,
                Usuario = a.Usuario.Nombre + " " + a.Usuario.Apellido,
                ValorAnterior = a.ValorAnterior,
                ValorNuevo = a.ValorNuevo
            })
            .ToListAsync();
    }
}