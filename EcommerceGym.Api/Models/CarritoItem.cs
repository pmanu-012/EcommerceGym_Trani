using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceGym.Api.Models
{
    public class CarritoItem
    {
        [Key] 
        public int IdCarritoItem { get; set; }
        public int Cantidad { get; set; }

        public int IdUsuario { get; set; }
        [ForeignKey(nameof(IdUsuario))]
        public Usuario Usuario { get; set; } = null!;
        public int IdProducto { get; set; }
        [ForeignKey(nameof(IdProducto))]
        public Producto Producto { get; set; } = null!;
    }
}
