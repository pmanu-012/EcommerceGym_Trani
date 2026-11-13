// PedidoAdminDto.cs
using EcommerceGym.Shared.Enums;
namespace EcommerceGym.Shared.DTOs;

public class PedidoAdminDto
{
    public int IdPedido { get; set; }
    public DateTime Fecha { get; set; }
    public EstadoPedido Estado { get; set; }
    public decimal Total { get; set; }
    public string Cliente { get; set; } = string.Empty;
}