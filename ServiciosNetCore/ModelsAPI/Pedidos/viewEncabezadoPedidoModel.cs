using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiciosNetCore.ModelsAPI.Pedidos
{
    public class viewEncabezadoPedidoModel
    {
        public int id { get; set; }
        public int usuarioId { get; set; }
        public int clientePersonaId { get; set; }
        public string valorNeto { get; set; }
        public string valorIva { get; set; }
        public string valorTotal { get; set; }
        public bool estado { get; set; }
        public string fechaCreacion { get; set; }
        public string fechaActualizacion { get; set; }
        public string nombreUsuario { get; set; }
        public string cliente { get; set; }
    }
}
