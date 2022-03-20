using System.ComponentModel.DataAnnotations;

namespace ServicioPersonas.ModelsAPI.Persona
{
    public class UsuariosModel
    {
        public int? id { get; set; }

        [Required]
        [StringLength(25, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [Display(Name = "Usuario")]
        public string nombreUsuario { get; set; }

        [Required]
        [StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string passwordHash { get; set; }
        public bool estado { get; set; }
        public string fechaCreacion { get; set; }
        public string fechaActualizacion { get; set; }
        public int? personaId { get; set; }
        public string persona { get; set; }
    }
}
