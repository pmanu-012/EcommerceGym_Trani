using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceGym.Api.Models
{
    public class ProductoDisciplina
    {
        [Key]
        public int IdProductoDisciplina { get; set; }

        public int IdProducto { get; set; }
        [ForeignKey(nameof(IdProducto))]
        public Producto Producto { get; set; } = null!;
        public int IdDisciplina { get; set; }
        [ForeignKey(nameof(IdDisciplina))]
        public Disciplina Disciplina { get; set; } = null!;
    }
}
