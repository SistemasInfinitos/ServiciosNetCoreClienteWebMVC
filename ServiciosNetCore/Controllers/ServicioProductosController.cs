using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServiciosNetCore.Configuration;
using ServiciosNetCore.ModelsAPI.Comun;
using ServiciosNetCore.ModelsAPI.DataTable;
using ServiciosNetCore.ModelsAPI.Productos;
using ServiciosNetCore.ModelsDB.Contexts;
using ServiciosNetCore.Repositorio.ProcuctosES;
using ServiciosNetCore.Repositorio.ProductosES;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServiciosNetCore.Controllers
{
    //[Authorize(Roles = "Administrador")]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ServicioProductosController : ControllerBase
    {
        private readonly IProductosESRepositorio _repositoryProductos;
        public ServicioProductosController(IOptionsMonitor<JwtConfiguracion> optionsMonitor, Context context)
        {
            _repositoryProductos = new ProductosESRepositorio(optionsMonitor, context);
        }
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> CrearProducto([FromBody] ProductosModel entidad)
        {
            ResponseApp data = new ResponseApp()
            {
                mensaje = "Ups!. Tu Solicitud No Pudo ser Procesada",
                ok = false
            };
            try
            {
                data.ok = await Task.Run(() => _repositoryProductos.CrearProducto(entidad));
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
        public async Task<IActionResult> ActualizarProducto([FromBody] ProductosModel entidad)
        {
            ResponseApp data = new ResponseApp()
            {
                mensaje = "Ups!. Tu Solicitud No Pudo ser Procesada",
                ok = false
            };
            try
            {
                data.ok = await Task.Run(() => _repositoryProductos.ActualizarProducto(entidad));
                data.mensaje = "Transaccion exitosa!";
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
        public async Task<IActionResult> DeleteProducto(int? id)
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
                    data.ok = await Task.Run(() => _repositoryProductos.DeleteProducto(id.Value));
                    data.mensaje = "Transaccion exitosa!";
                    return Ok(data);
                }
                catch (Exception x)
                {
                    data.mensaje = "Ups!. Algo salio mal!. Error interno. " + x.HResult;
                }
            }
            return BadRequest(data);
        }

        [Route("[action]", Name = "GetProducto")]
        [HttpGet]
        public async Task<IActionResult> GetProducto(string buscar, int? id)
        {
            bool ok = false;
            string mensaje = "Sin Datos";
            var producto = await Task.Run(() => _repositoryProductos.GetProducto(buscar, id));
            if (producto != null && producto.id > 0)
            {
                mensaje = "ok";
                ok = true;
            }
            var options = new JsonSerializerOptions { IncludeFields = true };
            var json = JsonSerializer.Serialize(producto, options);
            return Ok(json);
        }

        [Route("[action]", Name = "GetProductoDropList")]
        [HttpGet]
        public async Task<IActionResult> GetProductoDropList(string buscar, int? id)
        {
            var producto = await Task.Run(() => _repositoryProductos.GetProductosDropList(buscar, id));
            return Ok(producto);
        }

        /// <summary>
        /// Metodo post espesial para Datatable
        /// </summary>
        /// <param name="dtParms"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ListProductos([FromBody] DataTableParameter datatParms)
        {
            DataTableResponseProducto res = await Task.Run(() => _repositoryProductos.GetProductosDataTable(datatParms));
            res.draw = datatParms.draw;

            var options = new JsonSerializerOptions { IncludeFields = true };
            var json = JsonSerializer.Serialize(res, options);

            return Ok(json);
        }
    }
}
