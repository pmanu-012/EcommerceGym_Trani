using System.ComponentModel.DataAnnotations;

namespace EcommerceGym.Api.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        [MaxLength(100)] 
        public string Nombre { get; set; } = string.Empty;
        [MaxLength(100)] 
        public string Apellido { get; set; } = string.Empty;
        [MaxLength(30)] 
        public string Telefono { get; set; } = string.Empty;
        [MaxLength(256)] 
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;

        public int IdRol { get; set; }
        public Rol Rol { get; set; } = null!;
    }
}
