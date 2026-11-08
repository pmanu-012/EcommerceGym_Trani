namespace EcommerceGym.Shared.DTOs;

public class CarritoItemDto
{
    public int IdCarritoItem { get; set; }
    public int IdProducto { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string ImagenUrl { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
    public int Stock { get; set; }
    public decimal Subtotal => PrecioUnitario * Cantidad;
}