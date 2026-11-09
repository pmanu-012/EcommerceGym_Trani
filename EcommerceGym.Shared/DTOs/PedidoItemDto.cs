namespace EcommerceGym.Shared.DTOs;

public class PedidoItemDto
{
    public int IdProducto { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal => PrecioUnitario * Cantidad;
}