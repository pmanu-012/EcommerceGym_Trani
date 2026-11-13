using System.ComponentModel.DataAnnotations;
namespace EcommerceGym.Shared.DTOs;

public class NuevoIngresoItemDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccioná un producto.")]
    public int IdProducto { get; set; }

    [Range(1, 100000, ErrorMessage = "La cantidad debe ser al menos 1.")]
    public int Cantidad { get; set; }
}