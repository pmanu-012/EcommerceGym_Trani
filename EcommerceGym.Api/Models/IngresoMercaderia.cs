using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceGym.Api.Models
{
    public class IngresoMercaderia
    {
        [Key] 
        public int IdIngreso { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        [MaxLength(500)] 
        public string? Observacion { get; set; }

        public int IdUsuario { get; set; }
        [ForeignKey(nameof(IdUsuario))]
        public Usuario Usuario { get; set; } = null!;
        public List<DetalleIngreso> Detalles { get; set; } = new();
    }
}
