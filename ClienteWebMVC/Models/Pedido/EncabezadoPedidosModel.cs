using System.Collections.Generic;

namespace ClienteWebMVC.Models.Pedido
{
    public class EncabezadoPedidosModel
    {
        public int? id { get; set; }
        public int? usuarioId { get; set; }
        public int clientePersonaId { get; set; }
        public string valorNeto { get; set; }
        public string valorIva { get; set; }
        public string valorTotal { get; set; }
        public bool estado { get; set; }
        public string fechaCreacion { get; set; }
        public string fechaActualizacion { get; set; }

        public List<DetallePedidosModel> detallePedidos { get; set; }
    }
}
