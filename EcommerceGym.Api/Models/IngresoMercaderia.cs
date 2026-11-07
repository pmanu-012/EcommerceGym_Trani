using System.ComponentModel.DataAnnotations;

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
        public Usuario Usuario { get; set; } = null!;
        public List<DetalleIngreso> Detalles { get; set; } = new();
    }
}
