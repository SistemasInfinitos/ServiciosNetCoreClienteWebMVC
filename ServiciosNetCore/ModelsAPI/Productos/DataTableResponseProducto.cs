using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiciosNetCore.ModelsAPI.Productos
{
    public class DataTableResponseProducto
    {
        public int draw;
        public int recordsTotal;
        public int recordsFiltered;
        public List<ProductosModel> data;
    }
}
