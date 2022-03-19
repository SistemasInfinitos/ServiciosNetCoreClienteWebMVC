namespace ServiciosNetCore.ModelsAPI.Productos
{
    public class ProductosModel
    {
        public int id { get; set; }
        public string descripcion { get; set; }
        public decimal valorUnitario { get; set; }
        public bool estado { get; set; }
        public string fechaCreacion { get; set; }
        public string fechaActualizacion { get; set; }

    }
}
