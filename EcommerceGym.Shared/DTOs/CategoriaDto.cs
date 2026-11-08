namespace EcommerceGym.Shared.DTOs;

public class CategoriaDto
{
    public int IdCategoria { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public List<SubcategoriaDto> Subcategorias { get; set; } = new();
}