using System.ComponentModel.DataAnnotations;

namespace EcommerceGym.Api.Models
{
    public class Disciplina
    {
        [Key]
        public int IdDisciplina { get; set; }
        [MaxLength(100)] 
        public string Nombre { get; set; } = string.Empty;
    }
}
