using System;
using System.Collections.Generic;

#nullable disable

namespace ServiciosNetCore.ModelsDB
{
    public partial class EncabezadoPedido
    {
        public EncabezadoPedido()
        {
            DetallePedidoes = new HashSet<DetallePedido>();
        }

        public int id { get; set; }
        public int usuarioId { get; set; }
        public int clientePersonaId { get; set; }
        public decimal valorNeto { get; set; }
        public decimal valorIva { get; set; }
        public decimal valorTotal { get; set; }
        public bool estado { get; set; }
        public DateTime fechaCreacion { get; set; }
        public DateTime? fechaActualizacion { get; set; }

        public virtual ICollection<DetallePedido> DetallePedidoes { get; set; }
    }
}
