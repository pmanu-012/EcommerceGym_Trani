using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceGym.Api.Models
{
    public class ProductoGrupoMuscular
    {
        [Key] 
        public int IdProductoGrupoMuscular { get; set; }

        public int IdProducto { get; set; }
        [ForeignKey(nameof(IdProducto))]
        public Producto Producto { get; set; } = null!;
        public int IdGrupoMuscular { get; set; }
        [ForeignKey(nameof(IdGrupoMuscular))]
        public GrupoMuscular GrupoMuscular { get; set; } = null!;
    }
}
