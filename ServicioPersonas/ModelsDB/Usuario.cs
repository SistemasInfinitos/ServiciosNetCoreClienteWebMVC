using System;

#nullable disable

namespace ServicioPersonas.ModelsDB
{
    public partial class Usuario
    {
        public int id { get; set; }
        public string nombreUsuario { get; set; }
        public string passwordHash { get; set; }
        public bool estado { get; set; }
        public DateTime fechaCreacion { get; set; }
        public DateTime? fechaActualizacion { get; set; }
        public int personaId { get; set; }
    }
}
