namespace EcommerceGym.Shared.DTOs;

public class MiResenaDto
{
    public string Tipo { get; set; } = string.Empty;      // "Producto" o "Plataforma"
    public int IdResena { get; set; }
    public int? IdProducto { get; set; }
    public string Titulo { get; set; } = string.Empty;    // nombre del producto o "Reseña general"
    public int Calificacion { get; set; }
    public string Comentario { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}