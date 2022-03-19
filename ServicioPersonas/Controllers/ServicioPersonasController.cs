using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServicioPersonas.Configuration;
using ServicioPersonas.ModelsAPI.Comun;
using ServicioPersonas.ModelsAPI.DataTable;
using ServicioPersonas.ModelsAPI.DataTable.Persona;
using ServicioPersonas.ModelsAPI.Persona;
using ServicioPersonas.ModelsDB.Contexts;
using ServicioPersonas.Repositorio.PersonasES;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServicioPersonas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicioPersonasController : ControllerBase
    {
        private readonly IPersonasESRepositorio _repositoryPersonas;
        public ServicioPersonasController(IOptionsMonitor<JwtConfiguracion> optionsMonitor, Context context)
        {
            _repositoryPersonas = new PersonasESRepositorio(optionsMonitor, context);
        }
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> CreatePersona([FromBody] PersonasModel entidad)
        {
            ResponseApp data = new ResponseApp()
            {
                mensaje = "Ups!. Tu Solicitud No Pudo ser Procesada",
                ok = false
            };
            try
            {
                data.ok = await Task.Run(() => _repositoryPersonas.CrearPersona(entidad));
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
        public async Task<IActionResult> ActualizarPersona([FromBody] PersonasModel entidad)
        {
            ResponseApp data = new ResponseApp()
            {
                mensaje = "Ups!. Tu Solicitud No Pudo ser Procesada",
                ok = false
            };
            try
            {
                data.ok = await Task.Run(() => _repositoryPersonas.ActualizarPersona(entidad));
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
        public async Task<IActionResult> BorrarPersona(int? id)
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
                    data.ok = await Task.Run(() => _repositoryPersonas.DeletePersona(id.Value));
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

        [Route("[action]", Name = "GetPersona")]
        [HttpGet]
        public async Task<IActionResult> GetPersona(string buscar, int? id)
        {
            bool ok = false;
            string mensaje = "Sin Datos";
            var persona = await Task.Run(() => _repositoryPersonas.GetPersona(buscar, id));
            if (persona != null && persona.id > 0)
            {
                mensaje = "ok";
                ok = true;
            }
            var options = new JsonSerializerOptions { IncludeFields = true };
            var json = JsonSerializer.Serialize(persona, options);
            return Ok(json);
        }

        [Route("[action]", Name = "GetPersonasDropList")]
        [HttpGet]
        public async Task<IActionResult> GetPersonasDropList(string buscar, int? id)
        {
            var persona = await Task.Run(() => _repositoryPersonas.GetPersonasDropList(buscar, id));
            return Ok(persona);
        }

        /// <summary>
        /// Metodo post espesial para Datatable
        /// </summary>
        /// <param name="dtParms"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ListPersonas([FromBody] DataTableParameter datatParms)
        {
            DataTableResponsePersona res = await Task.Run(() => _repositoryPersonas.GetPersonasDataTable(datatParms));
            res.draw = datatParms.draw;

            var options = new JsonSerializerOptions { IncludeFields = true };
            var json = JsonSerializer.Serialize(res, options);

            return Ok(json);
        }
    }
}
