namespace EcommerceGym.Shared.DTOs;

public class ResenasDto
{
    public double Promedio { get; set; }
    public int Cantidad { get; set; }
    public List<ResenaDto> Items { get; set; } = new();
    public ResenaDto? MiResena { get; set; } // es la reseña del usuario que está consultando (si mandó su token),
    // para que la app sepa si mostrar “Dejar una reseña” o “Editar tu reseña”
}