using System.Collections.Generic;

namespace ServiciosNetCore.ModelsAPI.Pedidos
{
    public class DataTableResponsePedido
    {
        public int draw;
        public int recordsTotal;
        public int recordsFiltered;
        public List<viewEncabezadoPedidoModel> data;
    }
}
