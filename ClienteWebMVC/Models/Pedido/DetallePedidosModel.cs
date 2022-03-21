namespace ClienteWebMVC.Models.Pedido
{
    public class DetallePedidosModel
    {
        public int? id { get; set; }
        public int? encabezadoPedidosId { get; set; }
        public int productoId { get; set; }
        public virtual string producto { get; set; }
        public string cantidad { get; set; }
        public string porcentajeIva { get; set; }
        public string valorUnitario { get; set; }
        public bool? estado { get; set; }
        public string fechaCreacion { get; set; }
        public string fechaActualizacion { get; set; }
    }
}
