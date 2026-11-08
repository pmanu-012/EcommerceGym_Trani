using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EcommerceGym.Shared.Enums;

namespace EcommerceGym.Api.Models
{
    public class Producto
    {
        [Key] 
        public int IdProducto { get; set; }
        [MaxLength(150)] 
        public string Nombre { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")] 
        public decimal Precio { get; set; }
        [Column(TypeName = "decimal(18,2)")] 
        public decimal? PrecioOferta { get; set; }
        public int Stock { get; set; }
        [MaxLength(500)] 
        public string ImagenUrl { get; set; } = string.Empty;
        public UsoProducto? Uso { get; set; }
        public MaterialProducto? Material { get; set; }
        public bool Activo { get; set; } = true;

        public int IdSubcategoria { get; set; }
        [ForeignKey(nameof(IdSubcategoria))]
        public Subcategoria Subcategoria { get; set; } = null!;
        public List<ProductoDisciplina> ProductoDisciplinas { get; set; } = new();
        public List<ProductoGrupoMuscular> ProductoGruposMusculares { get; set; } = new();
    }
}
