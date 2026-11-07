using System.ComponentModel.DataAnnotations;

namespace EcommerceGym.Api.Models
{
    public class GrupoMuscular
    {
        [Key]
        public int IdGrupoMuscular { get; set; }
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
    }
}
