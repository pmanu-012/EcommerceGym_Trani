using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace EcommerceGym.Shared.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MaterialProducto { Acero, AceroInoxidable, Hierro, HierroFundido, Goma, Caucho, Aluminio, Algodon, Poliester, Cuero, Látex, Madera, Otro }
}
