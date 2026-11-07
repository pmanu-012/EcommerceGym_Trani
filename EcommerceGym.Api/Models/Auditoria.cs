using System.ComponentModel.DataAnnotations;
using EcommerceGym.Shared.Enums;

namespace EcommerceGym.Api.Models
{
    public class Auditoria
    {
        [Key]
        public int IdAuditoria { get; set; }
        public AccionAuditoria Accion { get; set; }
        [MaxLength(100)]
        public string Entidad { get; set; } = string.Empty;
        public int IdRegistro { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }

        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}
