using ServiciosNetCore.ModelsAPI.Comun;
using ServiciosNetCore.ModelsAPI.DataTable;
using ServiciosNetCore.ModelsAPI.Productos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiciosNetCore.Repositorio.ProductosES
{
    public interface IProductosESRepositorio
    {
        /// <summary>
        /// Borra un producto de la base de datos
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        Task<bool> DeleteProducto(int id);

        /// <summary>
        /// Actualiza un producto existente en la base de datos
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        Task<bool> ActualizarProducto(ProductosModel entidad);

        /// <summary>
        /// Crea una nuevo producto en la base de datos
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        Task<bool> CrearProducto(ProductosModel entidad);

        /// <summary>
        /// trae un producto segun su id de la base de datos
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<ProductosModel> GetProducto(string buscar, int? Id);

        /// <summary>
        /// obtiene una lista para un DropDownList  segun el filtro, maximo 10 item por coincidencia o los primero 10,  top 10
        /// </summary>
        /// <param name="buscar"></param>
        /// <returns></returns>
        Task<List<DropListModel>> GetProductosDropList(string buscar, int? id);

        /// <summary>
        /// Trae las productos de la base de datos,  segun el rango de paginado configurado
        /// </summary>
        /// <returns></returns>
        Task<DataTableResponseProducto> GetProductosDataTable(DataTableParameter dtParameters);
    }
}
