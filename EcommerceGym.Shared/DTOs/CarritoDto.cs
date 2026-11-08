namespace EcommerceGym.Shared.DTOs;

public class CarritoDto
{
    public List<CarritoItemDto> Items { get; set; } = new();
    public decimal Total => Items.Sum(i => i.Subtotal);
    public int CantidadTotal => Items.Sum(i => i.Cantidad);
}