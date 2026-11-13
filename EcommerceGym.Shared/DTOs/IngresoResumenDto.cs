namespace EcommerceGym.Shared.DTOs;

public class IngresoResumenDto
{
    public int IdIngreso { get; set; }
    public DateTime Fecha { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public int Productos { get; set; }
    public int Unidades { get; set; }
}