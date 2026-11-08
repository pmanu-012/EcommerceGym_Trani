using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [ForeignKey(nameof(IdUsuario))]
        public Usuario Usuario { get; set; } = null!;
    }
}
