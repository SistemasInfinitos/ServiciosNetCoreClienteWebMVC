using System.Collections.Generic;

namespace ServicioPersonas.ModelsAPI.Persona
{
    public class DataTableResponseUsuario
    {
        public int draw;
        public int recordsTotal;
        public int recordsFiltered;
        public List<UsuariosModel> data;
    }
}
