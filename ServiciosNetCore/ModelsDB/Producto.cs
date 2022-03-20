using System;
using System.Collections.Generic;

#nullable disable

namespace ServiciosNetCore.ModelsDB
{
    public partial class Producto
    {
        public Producto()
        {
            DetallePedidoes = new HashSet<DetallePedido>();
        }

        public int id { get; set; }
        public string descripcion { get; set; }
        public decimal valorUnitario { get; set; }
        public bool? estado { get; set; }
        public DateTime fechaCreacion { get; set; }
        public DateTime? fechaActualizacion { get; set; }
        public decimal iva { get; set; }

        public virtual ICollection<DetallePedido> DetallePedidoes { get; set; }
    }
}
