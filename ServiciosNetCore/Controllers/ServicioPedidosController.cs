using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServiciosNetCore.Configuration;
using ServiciosNetCore.ModelsAPI.Comun;
using ServiciosNetCore.ModelsAPI.DataTable;
using ServiciosNetCore.ModelsAPI.Pedidos;
using ServiciosNetCore.ModelsDB.Contexts;
using ServiciosNetCore.Repositorio.PedidosES;
using ServiciosNetCore.Repositorio.ProcuctosES;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServiciosNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicioPedidosController : ControllerBase
    {
        private readonly IPedidosESRepositorio _repositoryPedido;
        public ServicioPedidosController(IOptionsMonitor<JwtConfiguracion> optionsMonitor, Context context)
        {
            _repositoryPedido = new PedidosESRepositorio(optionsMonitor, context);
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> CrearPedido([FromBody] EncabezadoPedidosModel entidad)
        {
            ResponseApp data = new ResponseApp()
            {
                mensaje = "Ups!. Tu Solicitud No Pudo ser Procesada",
                ok = false
            };
            try
            {
                data.resul = await Task.Run(() => _repositoryPedido.CrearPedido(entidad));
                if (data.resul>0)
                {
                    data.mensaje = "Transaccion exitosa!";
                }
                return Ok(data);
            }
            catch (Exception x)
            {
                data.mensaje = "Ups!. Algo salio mal!. Error interno. " + x.HResult;
                return BadRequest(data);
            }
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> AgregarDetallePedido([FromBody] DetallePedidosModel entidad)
        {
            ResponseApp data = new ResponseApp()
            {
                mensaje = "Ups!. Tu Solicitud No Pudo ser Procesada",
                ok = false
            };
            try
            {
                data.ok = await Task.Run(() => _repositoryPedido.AgregarDetallePedido(entidad));
                if (data.ok)
                {
                    data.mensaje = "Transaccion exitosa!";
                }
                return Ok(data);
            }
            catch (Exception x)
            {
                data.mensaje = "Ups!. Algo salio mal!. Error interno. " + x.HResult;
                return BadRequest(data);
            }
        }

        [Route("[action]")]
        [HttpPut]
        public async Task<IActionResult> ActualizarPedidoEncabezado([FromBody] EncabezadoPedidosModel entidad)
        {
            ResponseApp data = new ResponseApp()
            {
                mensaje = "Ups!. Tu Solicitud No Pudo ser Procesada",
                ok = false
            };
            try
            {
                data.ok = await Task.Run(() => _repositoryPedido.ActualizarPedidoEncabezado(entidad));
                if (data.ok)
                {
                    data.mensaje = "Transaccion exitosa!";
                }
                return Ok(data);
            }
            catch (Exception x)
            {
                data.mensaje = "Ups!. Algo salio mal!. Error interno. " + x.HResult;
                return BadRequest(data);
            }
        }

        [Route("[action]")]
        [HttpDelete]
        public async Task<IActionResult> DeletePedido(int? id)
        {
            ResponseApp data = new ResponseApp()
            {
                mensaje = "Ups!. Tu Solicitud No Pudo ser Procesada",
                ok = false
            };

            if (id != null)
            {
                try
                {
                    data.ok = await Task.Run(() => _repositoryPedido.DeletePedido(id.Value));
                    if (data.ok)
                    {
                        data.mensaje = "Transaccion exitosa!";
                    }
                    return Ok(data);
                }
                catch (Exception x)
                {
                    data.mensaje = "Ups!. Algo salio mal!. Error interno. " + x.HResult;
                }
            }
            return BadRequest(data);
        }

        [Route("[action]")]
        [HttpDelete]
        public async Task<IActionResult> DeletePedidoDetalle(int? id)
        {
            ResponseApp data = new ResponseApp()
            {
                mensaje = "Ups!. Tu Solicitud No Pudo ser Procesada",
                ok = false
            };

            if (id != null)
            {
                try
                {
                    data.ok = await Task.Run(() => _repositoryPedido.DeletePedidoDetalle(id.Value));
                    if (data.ok)
                    {
                        data.mensaje = "Transaccion exitosa!";
                    }
                    return Ok(data);
                }
                catch (Exception x)
                {
                    data.mensaje = "Ups!. Algo salio mal!. Error interno. " + x.HResult;
                }
            }
            return BadRequest(data);
        }

        [Route("[action]", Name = "GetPedido")]
        [HttpGet]
        public async Task<IActionResult> GetPedido(string buscar, int? id)
        {
            bool ok = false;
            string mensaje = "Sin Datos";
            var pedido = await Task.Run(() => _repositoryPedido.GetPedido(buscar, id));
            if (pedido != null && pedido.id > 0)
            {
                mensaje = "ok";
                ok = true;
            }
            var options = new JsonSerializerOptions { IncludeFields = true };
            var json = JsonSerializer.Serialize(pedido, options);
            return Ok(json);
        }

        [Route("[action]", Name = "GetListDetallePedido")]
        [HttpGet]
        public async Task<IActionResult> GetListDetallePedido(int? id)
        {
            var pedido = await Task.Run(() => _repositoryPedido.GetListDetallePedido(id));
            var options = new JsonSerializerOptions { IncludeFields = true };
            var json = JsonSerializer.Serialize(pedido, options);
            return Ok(json);
        }

        [Route("[action]", Name = "GetPedidoDropList")]
        [HttpGet]
        public async Task<IActionResult> GetPedidoDropList(string buscar, int? id)
        {
            var pedido = await Task.Run(() => _repositoryPedido.GetPedidosDropList(buscar, id));
            return Ok(pedido);
        }

        /// <summary>
        /// Metodo post espesial para Datatable
        /// </summary>
        /// <param name="dtParms"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ListPedidos([FromBody] DataTableParameter datatParms)
        {
            DataTableResponsePedido res = await Task.Run(() => _repositoryPedido.GetPedidosDataTable(datatParms));
            res.draw = datatParms.draw;

            var options = new JsonSerializerOptions { IncludeFields = true };
            var json = JsonSerializer.Serialize(res, options);

            return Ok(json);
        }
    }
}
