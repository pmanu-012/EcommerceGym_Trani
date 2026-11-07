    using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceGym.Shared.DTOs
{
    public class ProductoDto
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public decimal? PrecioOferta { get; set; }
        public int Stock { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Subcategoria { get; set; } = string.Empty;
    }
}
