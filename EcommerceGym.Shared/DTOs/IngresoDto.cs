namespace EcommerceGym.Shared.DTOs;

public class IngresoDto
{
    public int IdIngreso { get; set; }
    public DateTime Fecha { get; set; }
    public string? Observacion { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public List<IngresoItemDto> Items { get; set; } = new();
    public int TotalUnidades => Items.Sum(i => i.Cantidad);
}
