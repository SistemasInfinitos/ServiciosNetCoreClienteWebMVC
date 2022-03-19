using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiciosNetCore.ModelsAPI.Pedidos
{
    public class DataTableResponsePedido
    {
        public int draw;
        public int recordsTotal;
        public int recordsFiltered;
        public List<EncabezadoPedidosModel> data;
    }
}
