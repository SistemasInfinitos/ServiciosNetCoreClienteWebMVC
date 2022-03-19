using SeguridadJWT.ModelsAPI.Persona;
using System.Collections.Generic;

namespace SeguridadJWT.ModelsAPI.DataTable.Persona
{
    public partial struct DataTableResponsePersona
    {
        public int draw;
        public int recordsTotal;
        public int recordsFiltered;
        public List<PersonasModel> data;
    }
}
