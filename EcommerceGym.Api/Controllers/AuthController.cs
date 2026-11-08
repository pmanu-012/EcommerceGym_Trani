using System.Security.Claims;
using EcommerceGym.Api.Data;
using EcommerceGym.Api.Models;
using EcommerceGym.Api.Services;
using EcommerceGym.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGym.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwt;

    public AuthController(AppDbContext db, JwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    [HttpPost("registro")]
    public async Task<ActionResult<AuthResponseDto>> Registro(RegistroDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        if (await _db.Usuarios.AnyAsync(u => u.Email == email))
            return Conflict(new { mensaje = "Ya existe una cuenta con ese email." });

        var rolCliente = await _db.Roles.FirstAsync(r => r.Nombre == "Cliente");

        var usuario = new Usuario
        {
            Nombre = dto.Nombre.Trim(),
            Apellido = dto.Apellido.Trim(),
            Telefono = dto.Telefono.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            IdRol = rolCliente.IdRol
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        return Ok(CrearRespuesta(usuario, rolCliente.Nombre));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        var usuario = await _db.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Email == email && u.Activo);

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            return Unauthorized(new { mensaje = "Email o contraseña incorrectos." });

        return Ok(CrearRespuesta(usuario, usuario.Rol.Nombre));
    }

    // Endpoint de prueba para verificar el token y el rol (se puede borrar después)
    [Authorize]
    [HttpGet("me")]
    public ActionResult Me() => Ok(new
    {
        IdUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier),
        Email = User.FindFirstValue(ClaimTypes.Email),
        Rol = User.FindFirstValue(ClaimTypes.Role)
    });

    private AuthResponseDto CrearRespuesta(Usuario u, string rol) => new()
    {
        Token = _jwt.GenerarToken(u, rol),
        IdUsuario = u.IdUsuario,
        Nombre = u.Nombre,
        Apellido = u.Apellido,
        Email = u.Email,
        Rol = rol
    };
}