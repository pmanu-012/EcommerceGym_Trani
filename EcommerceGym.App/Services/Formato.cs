using System.Globalization;

namespace EcommerceGym.App.Services;

public static class Formato
{
    private static readonly CultureInfo Es = new("es-AR");

    public static string Precio(decimal valor) => $"$ {valor.ToString("N0", Es)}";
}