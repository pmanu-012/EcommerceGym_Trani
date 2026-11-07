using System.ComponentModel.DataAnnotations;

namespace EcommerceGym.Api.Models
{
    public class Rol
    {
        [Key]
        public int IdRol { get; set; }
        [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;
    }
}
