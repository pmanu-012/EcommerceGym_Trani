using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceGym.Shared
{
        public enum EstadoPedido { Pendiente, Confirmado, Enviado, Entregado }

        public enum UsoProducto { Gimnasio, HomeGym, Mixto }

        public enum MaterialProducto { Acero, AceroInoxidable, Hierro, HierroFundido, Goma, Caucho, Aluminio, Algodon, Poliester, Cuero, Látex, Madera, Otro }

        public enum AccionAuditoria { Crear, Modificar, Eliminar }
}
