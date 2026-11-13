namespace EcommerceGym.Shared.DTOs;

public class ProductoStockDto
{
    public int IdProducto { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Stock { get; set; }
}