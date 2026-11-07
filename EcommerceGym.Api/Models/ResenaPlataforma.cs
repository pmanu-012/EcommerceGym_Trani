using System.ComponentModel.DataAnnotations;

namespace EcommerceGym.Api.Models
{
    public class ResenaPlataforma
    {
        [Key] 
        public int IdResena { get; set; }
        [Range(1, 5)] 
        public int Calificacion { get; set; }
        [MaxLength(1000)] 
        public string Comentario { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;

        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}
