using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Devices;

namespace EcommerceGym.App.Services;

public static class ApiConfig
{
    // true = API publicada en Somee; false = API local de desarrollo
    private const bool UsarProduccion = true;
    private const string UrlProduccion = "http://EcommerceGymMovil.somee.com";
    // En el emulador Android, 10.0.2.2 apunta a la PC donde corre la API
    public static string BaseUrl => UsarProduccion
        ? UrlProduccion
        : DeviceInfo.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:5196/"
            : "http://localhost:5196/";
}