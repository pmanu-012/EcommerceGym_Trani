using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace EcommerceGym.Shared.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UsoProducto { Gimnasio, HomeGym, Mixto }
}
