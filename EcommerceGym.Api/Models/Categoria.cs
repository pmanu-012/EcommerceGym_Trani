using System.ComponentModel.DataAnnotations;

namespace EcommerceGym.Api.Models
{
    public class Categoria
    {
        [Key] 
        public int IdCategoria { get; set; }
        [MaxLength(100)] 
        public string Nombre { get; set; } = string.Empty;

        public List<Subcategoria> Subcategorias { get; set; } = new();
    }
}
