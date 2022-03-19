using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServicioPersonas.Configuration;
using ServicioPersonas.ModelsAPI.Comun;
using ServicioPersonas.ModelsAPI.DataTable;
using ServicioPersonas.ModelsAPI.Persona;
using ServicioPersonas.ModelsDB.Contexts;
using ServicioPersonas.Repositorio.UsuariosES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServicioPersonas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicioUsuarioController : ControllerBase
    {
        private readonly IUsuariosESRepositorio _repositoryUsuario;
        public ServicioUsuarioController(IOptionsMonitor<JwtConfiguracion> optionsMonitor, Context context)
        {
            _repositoryUsuario = new UsuariosESRepositorio(optionsMonitor, context);
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> CreateUsuario([FromBody] UsuariosModel entidad)
        {
            ResponseApp data = new ResponseApp()
            {
                mensaje = "Ups!. Tu Solicitud No Pudo ser Procesada",
                ok = false
            };
            try
            {
                data.ok = await Task.Run(() => _repositoryUsuario.CrearUsuario(entidad));
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
        [HttpPut]
        public async Task<IActionResult> ActualizarUsuario([FromBody] UsuariosModel entidad)
        {
            ResponseApp data = new ResponseApp()
            {
                mensaje = "Ups!. Tu Solicitud No Pudo ser Procesada",
                ok = false
            };
            try
            {
                data.ok = await Task.Run(() => _repositoryUsuario.ActualizarUsuario(entidad));
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
        public async Task<IActionResult> BorrarUsuario(int? id)
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
                    data.ok = await Task.Run(() => _repositoryUsuario.DeleteUsuario(id.Value));
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

        [Route("[action]", Name = "GetUsuario")]
        [HttpGet]
        public async Task<IActionResult> GetUsuario(string buscar, int? id)
        {
            bool ok = false;
            string mensaje = "Sin Datos";
            var usuario = await Task.Run(() => _repositoryUsuario.GetUsuario(buscar, id));
            if (usuario != null && usuario.id > 0)
            {
                mensaje = "ok";
                ok = true;
            }
            var options = new JsonSerializerOptions { IncludeFields = true };
            var json = JsonSerializer.Serialize(usuario, options);
            return Ok(json);
        }

        [Route("[action]", Name = "GetUsuariosDropList")]
        [HttpGet]
        public async Task<IActionResult> GetUsuariosDropList(string buscar, int? id)
        {
            var usuario = await Task.Run(() => _repositoryUsuario.GetUsuariosDropList(buscar, id));
            return Ok(usuario);
        }

        /// <summary>
        /// Metodo post espesial para Datatable
        /// </summary>
        /// <param name="dtParms"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ListUsuarios([FromBody] DataTableParameter datatParms)
        {
            DataTableResponseUsuario res = await Task.Run(() => _repositoryUsuario.GetUsuariosDataTable(datatParms));
            res.draw = datatParms.draw;

            var options = new JsonSerializerOptions { IncludeFields = true };
            var json = JsonSerializer.Serialize(res, options);

            return Ok(json);
        }
    }
}
