using ServicioPersonas.ModelsAPI.Persona;
using System.Collections.Generic;

namespace ServicioPersonas.ModelsAPI.DataTable.Persona
{
    public partial struct DataTableResponsePersona
    {
        public int draw;
        public int recordsTotal;
        public int recordsFiltered;
        public List<PersonasModel> data;
    }
}
