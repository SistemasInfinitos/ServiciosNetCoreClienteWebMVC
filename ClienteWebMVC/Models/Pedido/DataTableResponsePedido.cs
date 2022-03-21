using System.Collections.Generic;

namespace ClienteWebMVC.Models.Pedido
{
    public class DataTableResponsePedido
    {
        public int draw;
        public int recordsTotal;
        public int recordsFiltered;
        public List<viewEncabezadoPedidoModel> data;
    }
}
