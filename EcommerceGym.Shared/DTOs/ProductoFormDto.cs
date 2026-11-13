using System.ComponentModel.DataAnnotations;
using EcommerceGym.Shared.Enums;
namespace EcommerceGym.Shared.DTOs;

public class ProductoFormDto
{
    public int IdProducto { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Range(0.01, 1000000000, ErrorMessage = "El precio debe ser mayor a 0.")]
    public decimal Precio { get; set; }

    [Range(0.01, 1000000000, ErrorMessage = "El precio de oferta debe ser mayor a 0.")]
    public decimal? PrecioOferta { get; set; }

    [Range(0, 1000000, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    [StringLength(500, ErrorMessage = "La URL no puede superar los 500 caracteres.")]
    public string? ImagenUrl { get; set; }

    public UsoProducto? Uso { get; set; }
    public MaterialProducto? Material { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccioná una subcategoría.")]
    public int IdSubcategoria { get; set; }

    public List<int> IdDisciplinas { get; set; } = new();
    public List<int> IdGruposMusculares { get; set; } = new();
}