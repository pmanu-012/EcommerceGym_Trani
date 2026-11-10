using Microsoft.Maui.Storage;

namespace EcommerceGym.App.Services;

//Guarda la sesión en el almacenamiento seguro del dispositivo.
//En Windows el almacenamiento seguro a veces no funciona sin empaquetar la app,
//así que hay un respaldo en Preferences para que no te frene mientras desarrollás:
public class TokenStore
{
    private const string Clave = "sesion";

    public async Task GuardarAsync(string json)
    {
        try { await SecureStorage.Default.SetAsync(Clave, json); }
        catch { Preferences.Default.Set(Clave, json); }
    }

    public async Task<string?> LeerAsync()
    {
        try
        {
            var valor = await SecureStorage.Default.GetAsync(Clave);
            if (!string.IsNullOrEmpty(valor)) return valor;
        }
        catch { /* almacenamiento seguro no disponible en este entorno */ }

        var respaldo = Preferences.Default.Get(Clave, string.Empty);
        return string.IsNullOrEmpty(respaldo) ? null : respaldo;
    }

    public void Borrar()
    {
        try { SecureStorage.Default.Remove(Clave); } catch { }
        Preferences.Default.Remove(Clave);
    }
}