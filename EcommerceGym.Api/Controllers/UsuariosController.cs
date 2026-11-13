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
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly AuditoriaService _auditoria;

    public UsuariosController(AppDbContext db, AuditoriaService auditoria)
    {
        _db = db;
        _auditoria = auditoria;
    }

    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<UsuarioDto>>> Listar(string? buscar)
    {
        var query = _db.Usuarios.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var t = buscar.Trim();
            query = query.Where(u => u.Nombre.Contains(t) || u.Apellido.Contains(t) || u.Email.Contains(t));
        }

        return await query
            .OrderBy(u => u.Apellido).ThenBy(u => u.Nombre)
            .Select(u => new UsuarioDto
            {
                IdUsuario = u.IdUsuario,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Email = u.Email,
                Telefono = u.Telefono,
                Rol = u.Rol.Nombre,
                Activo = u.Activo
            })
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioDto>> GetById(int id)
    {
        var u = await _db.Usuarios.AsNoTracking().Include(x => x.Rol)
            .FirstOrDefaultAsync(x => x.IdUsuario == id);

        if (u is null)
            return NotFound(new { mensaje = "El usuario no existe." });

        return new UsuarioDto
        {
            IdUsuario = u.IdUsuario,
            Nombre = u.Nombre,
            Apellido = u.Apellido,
            Email = u.Email,
            Telefono = u.Telefono,
            Rol = u.Rol.Nombre,
            Activo = u.Activo
        };
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Editar(int id, UsuarioEditDto dto)
    {
        var u = await _db.Usuarios.FirstOrDefaultAsync(x => x.IdUsuario == id);
        if (u is null)
            return NotFound(new { mensaje = "El usuario no existe." });

        var email = dto.Email.Trim().ToLower();
        if (await _db.Usuarios.AnyAsync(x => x.Email == email && x.IdUsuario != id))
            return Conflict(new { mensaje = "Ya existe otra cuenta con ese email." });

        if (!dto.Activo && id == UsuarioId)
            return BadRequest(new { mensaje = "No podés dar de baja tu propia cuenta." });

        var anterior = Snapshot(u);
        u.Nombre = dto.Nombre.Trim();
        u.Apellido = dto.Apellido.Trim();
        u.Telefono = dto.Telefono.Trim();
        u.Email = email;
        u.Activo = dto.Activo;

        _auditoria.Registrar(UsuarioId, AccionAuditoria.Modificar, "Usuario", id, anterior, Snapshot(u));
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // Baja lógica: la cuenta no puede volver a iniciar sesión, pero se conserva su historial
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DarDeBaja(int id)
    {
        var u = await _db.Usuarios.FirstOrDefaultAsync(x => x.IdUsuario == id && x.Activo);
        if (u is null)
            return NotFound(new { mensaje = "El usuario no existe o ya está dado de baja." });

        if (id == UsuarioId)
            return BadRequest(new { mensaje = "No podés dar de baja tu propia cuenta." });

        var anterior = Snapshot(u);
        u.Activo = false;

        _auditoria.Registrar(UsuarioId, AccionAuditoria.Eliminar, "Usuario", id, anterior, Snapshot(u));
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // Nunca se registra el hash de la contraseña
    private static object Snapshot(Usuario u) => new { u.Nombre, u.Apellido, u.Telefono, u.Email, u.Activo };
}