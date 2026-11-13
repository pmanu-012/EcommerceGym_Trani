namespace EcommerceGym.Shared.DTOs;

public class EstadisticasDto
{
    public decimal VentasHoy { get; set; }
    public decimal VentasMes { get; set; }
    public List<ProductoVendidoDto> MasVendidos { get; set; } = new();
    public CategoriaVentasDto? CategoriaLider { get; set; }
    public List<ProductoValoradoDto> MejorValorados { get; set; } = new();
    public List<ProductoStockDto> StockBajo { get; set; } = new();
}