using System.ComponentModel.DataAnnotations;

namespace EcommerceGym.Api.Models
{
    public class CarritoItem
    {
        [Key] 
        public int IdCarritoItem { get; set; }
        public int Cantidad { get; set; }

        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public int IdProducto { get; set; }
        public Producto Producto { get; set; } = null!;
    }
}
