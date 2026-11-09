using System.ComponentModel.DataAnnotations;
namespace EcommerceGym.Shared.DTOs;

public class CheckoutDto
{
    [Required(ErrorMessage = "La calle es obligatoria.")]
    [StringLength(150)]
    public string Calle { get; set; } = string.Empty;

    [Required(ErrorMessage = "El número es obligatorio.")]
    [StringLength(20)]
    public string Numero { get; set; } = string.Empty;

    [Required(ErrorMessage = "La ciudad es obligatoria.")]
    [StringLength(100)]
    public string Ciudad { get; set; } = string.Empty;

    [Required(ErrorMessage = "La provincia es obligatoria.")]
    [StringLength(100)]
    public string Provincia { get; set; } = string.Empty;

    [Required(ErrorMessage = "El código postal es obligatorio.")]
    [StringLength(20)]
    public string CodigoPostal { get; set; } = string.Empty;
}