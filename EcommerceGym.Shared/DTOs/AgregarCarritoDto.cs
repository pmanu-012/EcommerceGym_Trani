using System.ComponentModel.DataAnnotations;
namespace EcommerceGym.Shared.DTOs;

public class AgregarCarritoDto
{
    public int IdProducto { get; set; }

    [Range(1, 999, ErrorMessage = "La cantidad debe ser al menos 1.")]
    public int Cantidad { get; set; } = 1;
}