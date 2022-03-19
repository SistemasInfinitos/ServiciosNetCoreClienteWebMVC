namespace ServicioPersonas.ModelsAPI.Persona
{
    public class UsuariosModel
    {
        public int? id { get; set; }
        public string nombreUsuario { get; set; }
        public string passwordHash { get; set; }
        public bool estado { get; set; }
        public string fechaCreacion { get; set; }
        public string fechaActualizacion { get; set; }
        public string personaId { get; set; }
    }
}
