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
        public decimal valorNeto { get; set; }
        public decimal valorIva { get; set; }
        public decimal valorTotal { get; set; }
        public bool estado { get; set; }
        public string fechaCreacion { get; set; }
        public string fechaActualizacion { get; set; }
        public string nombreUsuario { get; set; }
        public string cliente { get; set; }
    }
}
