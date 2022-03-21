namespace ServicioPersonas.Configuration
{
    public class JwtConfiguracion
    {
        public string service { get; set; }
        public string secret { get; set; }
        public string key { get; set; }
        public string iv { get; set; }
        public string connectionString { get; set; }
        public string audience { get; set; }
    }
}
