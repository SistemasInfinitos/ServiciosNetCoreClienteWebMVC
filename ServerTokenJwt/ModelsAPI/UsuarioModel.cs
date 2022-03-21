using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerTokenJwt.ModelsAPI
{
    public class UsuarioModel
    {
        public int? id { get; set; }
        public bool estado { get; set; }
        public string usuario { get; set; }
        public string pasword { get; set; }
        public string roles { get; set; }
        public string policy { get; set; }
        public virtual string fullName { get; set; }
    }
}
