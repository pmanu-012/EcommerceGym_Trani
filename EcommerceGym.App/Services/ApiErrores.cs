using System.Text.Json;

namespace EcommerceGym.App.Services;

//Convierte las respuestas de error de la API en un mensaje legible.
//Se reutiliza en todos los servicios que hacen llamadas HTTP a la API.
public static class ApiErrores
{
    public static async Task<string> LeerAsync(HttpResponseMessage resp)
    {
        try
        {
            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Errores propios: { "mensaje": "..." }
            if (root.TryGetProperty("mensaje", out var mensaje))
                return mensaje.GetString() ?? "Ocurrió un error.";

            // Errores de validación automática: { "errors": { "Campo": ["..."] } }
            if (root.TryGetProperty("errors", out var errors))
                foreach (var campo in errors.EnumerateObject())
                    foreach (var msg in campo.Value.EnumerateArray())
                        return msg.GetString() ?? "Los datos ingresados no son válidos.";
        }
        catch (JsonException) { /* respuesta sin cuerpo JSON */ }

        return resp.StatusCode == System.Net.HttpStatusCode.Unauthorized
            ? "Tu sesión no es válida o venció. Iniciá sesión de nuevo."
            : $"Ocurrió un error inesperado ({(int)resp.StatusCode}).";
    }
}