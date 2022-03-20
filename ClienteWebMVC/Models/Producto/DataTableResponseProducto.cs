using System.Collections.Generic;

namespace ClienteWebMVC.Models.Producto
{
    public class DataTableResponseProducto
    {
        public int draw;
        public int recordsTotal;
        public int recordsFiltered;
        public List<ProductosModel> data;
    }
}
