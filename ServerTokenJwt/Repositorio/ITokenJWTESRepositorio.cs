using ServerTokenJwt.ModelsAPI;
using System.Threading.Tasks;

namespace ServerTokenJwt.Repositorio
{
    public interface ITokenJWTESRepositorio
    {
        /// <summary>
        /// Encripta una cadena de texto (en una direecion ->)
        /// </summary>
        /// <param name="contrasena"></param>
        /// <returns></returns>
        Task<string> EncriptarContrasena(string contrasena);

        /// <summary>
        /// Verifia las credenciales de Usurio
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        Task<UsuarioModel> Login(UsuarioModel User);
    }
}
