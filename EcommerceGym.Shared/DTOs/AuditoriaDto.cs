using EcommerceGym.Shared.Enums;
namespace EcommerceGym.Shared.DTOs;

public class AuditoriaDto
{
    public int IdAuditoria { get; set; }
    public DateTime Fecha { get; set; }
    public AccionAuditoria Accion { get; set; }
    public string Entidad { get; set; } = string.Empty;
    public int IdRegistro { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string? ValorAnterior { get; set; }
    public string? ValorNuevo { get; set; }
}