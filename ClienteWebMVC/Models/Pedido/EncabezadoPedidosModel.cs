using System.Collections.Generic;

namespace ClienteWebMVC.Models.Pedido
{
    public class EncabezadoPedidosModel
    {
        public int? id { get; set; }
        public int? usuarioId { get; set; }
        public int clientePersonaId { get; set; }
        public decimal valorNeto { get; set; }
        public decimal valorIva { get; set; }
        public decimal valorTotal { get; set; }
        public bool? estado { get; set; }
        public string fechaCreacion { get; set; }
        public string fechaActualizacion { get; set; }

        public List<DetallePedidosModel> detallePedidos { get; set; }
    }
}
