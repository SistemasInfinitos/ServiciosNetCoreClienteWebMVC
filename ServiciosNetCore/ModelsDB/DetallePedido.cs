using System;

#nullable disable

namespace ServiciosNetCore.ModelsDB
{
    public partial class DetallePedido
    {
        public int id { get; set; }
        public int encabezadoPedidosId { get; set; }
        public int productoId { get; set; }
        public decimal cantidad { get; set; }
        public decimal porcentajeIva { get; set; }
        public decimal valorUnitario { get; set; }
        public bool? estado { get; set; }
        public DateTime fechaCreacion { get; set; }
        public DateTime? fechaActualizacion { get; set; }

        public virtual EncabezadoPedido encabezadoPedidos { get; set; }
        public virtual Producto producto { get; set; }
    }
}
