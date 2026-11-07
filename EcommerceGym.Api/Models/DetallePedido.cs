using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceGym.Api.Models
{
    public class DetallePedido
    {
        [Key] 
        public int IdDetalle { get; set; }
        public int Cantidad { get; set; }
        [Column(TypeName = "decimal(18,2)")] 
        public decimal PrecioUnitario { get; set; }

        public int IdPedido { get; set; }
        public Pedido Pedido { get; set; } = null!;
        public int IdProducto { get; set; }
        public Producto Producto { get; set; } = null!;
    }
}
