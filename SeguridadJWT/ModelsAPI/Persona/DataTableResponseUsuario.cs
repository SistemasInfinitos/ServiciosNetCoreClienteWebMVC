using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguridadJWT.ModelsAPI.Persona
{
    public class DataTableResponseUsuario
    {
        public int draw;
        public int recordsTotal;
        public int recordsFiltered;
        public List<UsuariosModel> data;
    }
}
