﻿using System;
using System.Collections.Generic;

#nullable disable

namespace SeguridadJWT.ModelsDB
{
    public partial class Persona
    {
        public int id { get; set; }
        public string nombres { get; set; }
        public string apellidos { get; set; }
        public bool estado { get; set; }
        public DateTime fechaCreacion { get; set; }
        public DateTime? fechaActualizacion { get; set; }
    }
}