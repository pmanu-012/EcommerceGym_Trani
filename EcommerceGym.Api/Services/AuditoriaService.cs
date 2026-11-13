using System.Text.Json;
using EcommerceGym.Api.Data;
using EcommerceGym.Api.Models;
using EcommerceGym.Shared.Enums;

namespace EcommerceGym.Api.Services;

//Agrega el registro al contexto, así se guarda junto con la operación: o se guardan las dos cosas o ninguna.
public class AuditoriaService
{
    private readonly AppDbContext _db;
    public AuditoriaService(AppDbContext db) => _db = db;

    public void Registrar(int idUsuario, AccionAuditoria accion, string entidad, int idRegistro,
                          object? anterior, object? nuevo)
    {
        _db.Auditorias.Add(new Auditoria
        {
            IdUsuario = idUsuario,
            Accion = accion,
            Entidad = entidad,
            IdRegistro = idRegistro,
            Fecha = DateTime.Now,
            ValorAnterior = anterior is null ? null : JsonSerializer.Serialize(anterior),
            ValorNuevo = nuevo is null ? null : JsonSerializer.Serialize(nuevo)
        });
    }
}