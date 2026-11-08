namespace EcommerceGym.Shared.DTOs;

public class ProductoDetalleDto : ProductoDto
{
    public string? Uso { get; set; }
    public string? Material { get; set; }
    public List<string> Disciplinas { get; set; } = new();
    public List<string> GruposMusculares { get; set; } = new();
}