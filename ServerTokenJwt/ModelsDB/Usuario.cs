using System;
using System.Collections.Generic;

#nullable disable

namespace ServerTokenJwt.ModelsDB
{
    public partial class Usuario
    {
        public int id { get; set; }
        public string nombreUsuario { get; set; }
        public string passwordHash { get; set; }
        public bool? estado { get; set; }
        public DateTime fechaCreacion { get; set; }
        public DateTime? fechaActualizacion { get; set; }
        public int? personaId { get; set; }

        public virtual Persona persona { get; set; }
    }
}
