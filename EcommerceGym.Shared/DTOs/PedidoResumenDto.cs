using EcommerceGym.Shared.Enums;
namespace EcommerceGym.Shared.DTOs;

public class PedidoResumenDto
{
    public int IdPedido { get; set; }
    public DateTime Fecha { get; set; }
    public EstadoPedido Estado { get; set; }
    public decimal Total { get; set; }
}