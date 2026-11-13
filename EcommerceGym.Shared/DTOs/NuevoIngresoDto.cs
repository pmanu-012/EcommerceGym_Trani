using System.ComponentModel.DataAnnotations;
namespace EcommerceGym.Shared.DTOs;

public class NuevoIngresoDto
{
    [StringLength(500, ErrorMessage = "La observación no puede superar los 500 caracteres.")]
    public string? Observacion { get; set; }

    public List<NuevoIngresoItemDto> Items { get; set; } = new();
}