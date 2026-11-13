using System.Text.Json;

namespace EcommerceGym.App.Services;

public record CambioAuditoria(string Campo, string? Anterior, string? Nuevo);

//Convierte los valores anterior y nuevo (guardados como JSON) en una lista legible de cambios,
//para mostrar “Estado: Pendiente → Confirmado” en vez de texto crudo
public static class AuditoriaFormato
{
    public static List<CambioAuditoria> Cambios(string? anterior, string? nuevo)
    {
        var a = Leer(anterior);
        var n = Leer(nuevo);
        var lista = new List<CambioAuditoria>();

        foreach (var campo in a.Keys.Concat(n.Keys).Distinct())
        {
            a.TryGetValue(campo, out var valorAnterior);
            n.TryGetValue(campo, out var valorNuevo);

            // En una modificación solo interesa lo que realmente cambió
            if (anterior is not null && nuevo is not null && valorAnterior == valorNuevo) continue;

            lista.Add(new CambioAuditoria(campo, valorAnterior, valorNuevo));
        }

        return lista;
    }

    private static Dictionary<string, string> Leer(string? json)
    {
        var datos = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(json)) return datos;

        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Object) return datos;

            foreach (var p in doc.RootElement.EnumerateObject())
            {
                datos[p.Name] = p.Value.ValueKind switch
                {
                    JsonValueKind.String => p.Value.GetString() ?? "",
                    JsonValueKind.True => "Sí",
                    JsonValueKind.False => "No",
                    JsonValueKind.Null => "—",
                    JsonValueKind.Array => $"{p.Value.GetArrayLength()} ítems",
                    _ => p.Value.GetRawText()
                };
            }
        }
        catch (JsonException) { /* valor que no es JSON: se ignora */ }

        return datos;
    }
}