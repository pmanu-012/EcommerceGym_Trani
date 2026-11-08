using System.ComponentModel.DataAnnotations;
namespace EcommerceGym.Shared.DTOs;

public class ActualizarCarritoDto
{
    [Range(1, 999, ErrorMessage = "La cantidad debe ser al menos 1.")]
    public int Cantidad { get; set; }
}