using System.ComponentModel.DataAnnotations;

namespace ClienteWebMVC.Models.Producto
{
    public class ProductosModel
    {
        public int? id { get; set; }

        [Required]
        public string descripcion { get; set; }

        [Required]
        [StringLength(24, ErrorMessage = "El {0} debe tener al menos {1} caracteres.", MinimumLength = 1)]
        [Display(Name = "Valor Unitario")]
        public string valorUnitario { get; set; }
        public bool estado { get; set; }
        public string fechaCreacion { get; set; }
        public string fechaActualizacion { get; set; }
        public string iva { get; set; }
    }
}
