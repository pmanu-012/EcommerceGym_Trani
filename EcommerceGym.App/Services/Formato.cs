using System.Globalization;

namespace EcommerceGym.App.Services;

public static class Formato
{
    private static readonly CultureInfo Es = new("es-AR");

    public static string Precio(decimal valor) => $"$ {valor.ToString("N0", Es)}";

    public static string Legible(string? valor) => valor switch
    {
         "HomeGym" => "Home gym",
         "HierroFundido" => "Hierro fundido",
         "Algodon" => "Algodón",
         "Poliester" => "Poliéster",
         null => string.Empty,
         _ => valor
    };
    public static string Fecha(DateTime fecha) => fecha.ToString("dd/MM/yyyy", Es);

    public static string NumeroPedido(int id) => "#" + id.ToString("D5");

    public static string Promedio(double valor) => valor.ToString("0.0", Es);
    public static string FechaHora(DateTime fecha) => fecha.ToString("dd/MM/yyyy HH:mm", Es);
}