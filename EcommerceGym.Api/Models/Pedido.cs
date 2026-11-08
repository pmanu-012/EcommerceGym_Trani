using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EcommerceGym.Shared.Enums;

namespace EcommerceGym.Api.Models
{
    public class Pedido
    {
        [Key] 
        public int IdPedido { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente;
        [Column(TypeName = "decimal(18,2)")] 
        public decimal Total { get; set; }
        [Column(TypeName = "decimal(18,2)")] 
        public decimal CostoEnvio { get; set; }
        [MaxLength(150)] 
        public string Calle { get; set; } = string.Empty;
        [MaxLength(20)] 
        public string Numero { get; set; } = string.Empty;
        [MaxLength(100)] 
        public string Ciudad { get; set; } = string.Empty;
        [MaxLength(100)] 
        public string Provincia { get; set; } = string.Empty;
        [MaxLength(20)] 
        public string CodigoPostal { get; set; } = string.Empty;

        public int IdUsuario { get; set; }
        [ForeignKey(nameof(IdUsuario))]
        public Usuario Usuario { get; set; } = null!;
        public List<DetallePedido> Detalles { get; set; } = new();
    }
}
