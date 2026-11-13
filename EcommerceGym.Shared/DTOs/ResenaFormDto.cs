using System.ComponentModel.DataAnnotations;
namespace EcommerceGym.Shared.DTOs;

public class ResenaFormDto
{
    [Range(1, 5, ErrorMessage = "Elegí una calificación de 1 a 5 estrellas.")]
    public int Calificacion { get; set; }

    [Required(ErrorMessage = "Escribí un comentario.")]
    [StringLength(1000, ErrorMessage = "El comentario no puede superar los 1000 caracteres.")]
    public string Comentario { get; set; } = string.Empty;
}