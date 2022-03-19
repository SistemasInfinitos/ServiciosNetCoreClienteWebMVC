using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiciosNetCore.ModelsAPI.Pedidos
{
    public class DetallePedidosModel
    {
        public int? id { get; set; }
        public int? encabezadoPedidosId { get; set; }
        public int productoId { get; set; }
        public decimal cantidad { get; set; }
        public decimal porcentajeIva { get; set; }
        public decimal valorUnitario { get; set; }
        public bool? estado { get; set; }
        public string fechaCreacion { get; set; }
        public string fechaActualizacion { get; set; }
    }
}
