using System.ComponentModel.DataAnnotations;

namespace EcommerceGym.Api.Models
{
    public class Subcategoria
    {
        [Key]
        public int IdSubcategoria { get; set; }
        [MaxLength(100)] 
        public string Nombre { get; set; } = string.Empty;

        public int IdCategoria { get; set; }
        public Categoria Categoria { get; set; } = null!;
    }
}
