using EcommerceGym.Api.Data;
using EcommerceGym.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGym.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogosController : ControllerBase
{
    private readonly AppDbContext _db;
    public CatalogosController(AppDbContext db) => _db = db;

    [HttpGet("disciplinas")]
    public async Task<ActionResult<List<OpcionDto>>> Disciplinas() =>
        await _db.Disciplinas.AsNoTracking().OrderBy(d => d.Nombre)
            .Select(d => new OpcionDto { Id = d.IdDisciplina, Nombre = d.Nombre }).ToListAsync();

    [HttpGet("grupos-musculares")]
    public async Task<ActionResult<List<OpcionDto>>> GruposMusculares() =>
        await _db.GruposMusculares.AsNoTracking().OrderBy(g => g.Nombre)
            .Select(g => new OpcionDto { Id = g.IdGrupoMuscular, Nombre = g.Nombre }).ToListAsync();
}