using System.ComponentModel.DataAnnotations;

namespace ServiciosNetCore.ModelsAPI.Productos
{
    public class ProductosModel
    {
        public int? id { get; set; }

        [Required]
        public string descripcion { get; set; }

        [Required]
        public string valorUnitario { get; set; }
        public bool estado { get; set; }
        public string fechaCreacion { get; set; }
        public string fechaActualizacion { get; set; }

    }
}
