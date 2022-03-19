using ServiciosNetCore.ModelsAPI.Comun;
using ServiciosNetCore.ModelsAPI.DataTable;
using ServiciosNetCore.ModelsAPI.Pedidos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiciosNetCore.Repositorio.PedidosES
{
    public interface IPedidosESRepositorio
    {
        /// <summary>
        /// Borra un pedido de la base de datos y su detalle
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        Task<bool> DeletePedido(int id);

        /// <summary>
        /// Borra una linea del detalle del pedido y recalcula el encabezado
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeletePedidoDetalle(int id);

        /// <summary>
        /// agrega un producto al pedido, recalcula el encabezado y lo actualiza
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        Task<bool> AgregarDetallePedido(DetallePedidosModel entidad);

        /// <summary>
        /// Actualiza el encabezado de un pedido existente en la base de datos
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        Task<bool> ActualizarPedidoEncabezado(EncabezadoPedidosModel entidad);

        /// <summary>
        /// Crea una nuevo pedido en la base de datos con su detalle
        /// </summary>
        /// <param name="entidad"></param>
        /// <returns></returns>
        Task<bool> CrearPedido(EncabezadoPedidosModel entidad);

        /// <summary>
        /// trae un pedido segun su id de la base de datos
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<EncabezadoPedidosModel> GetPedido(string buscar, int? id);

        /// <summary>
        /// obtiene la lista de productos de un pedido segun su id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<DetallePedidosModel>> GetListDetallePedido(int? id);

        /// <summary>
        /// obtiene una lista para un DropDownList  segun el filtro, maximo 10 item por coincidencia o los primero 10,  top 10
        /// </summary>
        /// <param name="buscar"></param>
        /// <returns></returns>
        Task<List<DropListModel>> GetPedidosDropList(string buscar, int? id);

        /// <summary>
        /// Trae las pedidos de la base de datos,  segun el rango de paginado configurado
        /// </summary>
        /// <returns></returns>
        Task<DataTableResponsePedido> GetPedidosDataTable(DataTableParameter dtParameters);
    }
}
