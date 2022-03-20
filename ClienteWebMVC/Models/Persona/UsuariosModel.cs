﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClienteWebMVC.Models.Persona
{
    public class UsuariosModel
    {
        public int? id { get; set; }
        public string nombreUsuario { get; set; }
        public string passwordHash { get; set; }
        public bool estado { get; set; }
        public string fechaCreacion { get; set; }
        public string fechaActualizacion { get; set; }
        public int? personaId { get; set; }
    }
}