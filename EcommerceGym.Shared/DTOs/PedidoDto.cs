using EcommerceGym.Shared.Enums;
namespace EcommerceGym.Shared.DTOs;

public class PedidoDto
{
    public int IdPedido { get; set; }
    public DateTime Fecha { get; set; }
    public EstadoPedido Estado { get; set; }
    public decimal Total { get; set; }
    public decimal CostoEnvio { get; set; }
    public string Calle { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string Provincia { get; set; } = string.Empty;
    public string CodigoPostal { get; set; } = string.Empty;
    public List<PedidoItemDto> Items { get; set; } = new();
    public decimal Subtotal => Items.Sum(i => i.Subtotal);
}