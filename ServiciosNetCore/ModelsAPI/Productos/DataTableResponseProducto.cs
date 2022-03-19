using System.Collections.Generic;

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
