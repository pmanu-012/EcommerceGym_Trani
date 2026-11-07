using System.ComponentModel.DataAnnotations;

namespace EcommerceGym.Api.Models
{
    public class DetalleIngreso
    {
        [Key]
        public int IdDetalleIngreso { get; set; }
        public int Cantidad { get; set; }

        public int IdIngreso { get; set; }
        public IngresoMercaderia Ingreso { get; set; } = null!;
        public int IdProducto { get; set; }
        public Producto Producto { get; set; } = null!;
    }
}
