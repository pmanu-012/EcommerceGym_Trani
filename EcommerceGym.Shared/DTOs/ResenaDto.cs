namespace EcommerceGym.Shared.DTOs;

public class ResenaDto
{
    public int IdResena { get; set; }
    public int IdUsuario { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public int Calificacion { get; set; }
    public string Comentario { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}