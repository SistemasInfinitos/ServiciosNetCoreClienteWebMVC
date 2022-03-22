using System.ComponentModel.DataAnnotations;

namespace ServerTokenJwt.ModelsAPI
{
    public class UsuarioModel
    {
        public int? id { get; set; }
        public bool? estado { get; set; }

        [Required]
        public string usuario { get; set; }

        [Required]
        public string pasword { get; set; }
        public string roles { get; set; }
        public string policy { get; set; }
        public virtual string fullName { get; set; }
    }
}
