namespace EcommerceGym.Shared.DTOs;

public class IngresoItemDto
{
    public int IdProducto { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}