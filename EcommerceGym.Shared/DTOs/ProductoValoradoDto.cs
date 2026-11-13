namespace EcommerceGym.Shared.DTOs;

public class ProductoValoradoDto
{
    public int IdProducto { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public double Promedio { get; set; }
    public int Cantidad { get; set; }
}