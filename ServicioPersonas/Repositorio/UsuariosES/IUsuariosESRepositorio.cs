using ServicioPersonas.ModelsAPI.Comun;
using ServicioPersonas.ModelsAPI.DataTable;
using ServicioPersonas.ModelsAPI.Persona;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicioPersonas.Repositorio.UsuariosES
{
    public interface IUsuariosESRepositorio
    {
        /// <summary>
        /// Borra un usuario de la base de datos
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        Task<bool> DeleteUsuario(int id);

        /// <summary>
        /// Actualiza un usuario existente en la base de datos
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        Task<bool> ActualizarUsuario(UsuariosModel entidad);

        /// <summary>
        /// Crea una nuevo usuario en la base de datos
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        Task<bool> CrearUsuario(UsuariosModel entidad);

        /// <summary>
        /// trae un usuario segun si id de la base de datos
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<UsuariosModel> GetUsuario(string buscar, int? Id);

        /// <summary>
        /// obtiene una lista para un DropDownList  segun el filtro, maximo 10 item por coincidencia o los primero 10,  top 10
        /// </summary>
        /// <param name="buscar"></param>
        /// <returns></returns>
        Task<List<DropListModel>> GetUsuariosDropList(string buscar, int? id);

        /// <summary>
        /// Trae los usuarios de la base de datos,  segun el rango de paginado configurado
        /// </summary>
        /// <returns></returns>
        Task<DataTableResponseUsuario> GetUsuariosDataTable(DataTableParameter dtParameters);

        /// <summary>
        /// Encripta una cadena de texto (en una direecion ->)
        /// </summary>
        /// <param name="contrasena"></param>
        /// <returns></returns>
        Task<string> EncriptarContrasena(string contrasena);
    }
}
