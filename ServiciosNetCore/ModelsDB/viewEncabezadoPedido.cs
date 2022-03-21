using System;
using System.Collections.Generic;

#nullable disable

namespace ServiciosNetCore.ModelsDB
{
    public partial class viewEncabezadoPedido
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
