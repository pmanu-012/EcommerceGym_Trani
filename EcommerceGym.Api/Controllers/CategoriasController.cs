using EcommerceGym.Api.Data;
using EcommerceGym.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGym.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _db;
    public CategoriasController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<CategoriaDto>>> GetAll()
    {
        return await _db.Categorias
            .AsNoTracking()
            .OrderBy(c => c.IdCategoria)
            .Select(c => new CategoriaDto
            {
                IdCategoria = c.IdCategoria,
                Nombre = c.Nombre,
                Subcategorias = c.Subcategorias
                    .OrderBy(s => s.Nombre)
                    .Select(s => new SubcategoriaDto
                    {
                        IdSubcategoria = s.IdSubcategoria,
                        Nombre = s.Nombre
                    }).ToList()
            })
            .ToListAsync();
    }
}