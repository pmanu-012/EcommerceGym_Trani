using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceGym.Api.Models
{
    public class DetalleIngreso
    {
        [Key]
        public int IdDetalleIngreso { get; set; }
        public int Cantidad { get; set; }

        public int IdIngreso { get; set; }
        [ForeignKey(nameof(IdIngreso))]
        public IngresoMercaderia Ingreso { get; set; } = null!;
        public int IdProducto { get; set; }
        [ForeignKey(nameof(IdProducto))]
        public Producto Producto { get; set; } = null!;
    }
}
