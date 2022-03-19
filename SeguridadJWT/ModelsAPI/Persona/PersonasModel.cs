using System.ComponentModel.DataAnnotations;
namespace SeguridadJWT.ModelsAPI.Persona
{
    public class PersonasModel
    {
        public int? id { get; set; }
        [Required]
        public string nombres { get; set; }
        [Required]
        public string apellidos { get; set; }
        public string fechaCreacion { get; set; }
        public string fechaActualizacion { get; set; }
        public bool estado { get; set; }
        public virtual string cliente { get; set; }
    }
}
