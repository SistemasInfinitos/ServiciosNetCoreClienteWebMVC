using ClienteWebMVC.Configuration;
using ClienteWebMVC.Models.Comun;
using ClienteWebMVC.Models.Persona;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClienteWebMVC.Controllers.Persona
{
    [Route("web/us")]
    public class UsuariosController : Controller
    {
        private readonly JwtConfiguracion _jwtConfig;
        public UsuariosController(IOptionsMonitor<JwtConfiguracion> optionsMonitor)
        {
            this._jwtConfig = optionsMonitor.CurrentValue;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> Gestion(string id)
        {
            // esta es una forma de trabajar, aumenta la seguridad pero tanbien el tiempo de desarrollo
            var httpClient = new HttpClient();
            List<JwtConfiguracionAPI> api = new List<JwtConfiguracionAPI>();
            api = _jwtConfig.api; // esto garantiza la migracion a produccion ya que la url siempre cambia
            UsuariosModel model = new UsuariosModel();
            List<DropListModel> modelPerona = new List<DropListModel>();

            ViewBag.idString = "";
            var apis = api.Where(x => x.servicio == "ServicioPersonas").FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(id))
            {
                ViewBag.idString = id;
                string endpoint = "api/ServicioUsuario/GetUsuario";
                string parmetro = id;
                string uri = apis.uri + "/" + endpoint + "?id=" + parmetro;

                try
                {
                    var response = await httpClient.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        model = JsonConvert.DeserializeObject<UsuariosModel>(await response.Content.ReadAsStringAsync());
                    }
                }
                catch (Exception X)
                {
                    string mensaje = X.Message;
                    throw;
                }
            }
            //si entra en el anterio bloque SelectList seleccionara el actual
            #region DropDownList
            #region Personas
            ViewBag.personaId = new SelectList(modelPerona, "id", "rangoEdades");
            if (model.personaId>0)
            {
                int param = model != null && model.personaId != null ? model.personaId.Value : 0;
                //se establece la parsona para que no traiga mas de uno ya que hay un buscar ajax dinamico
                string uriCliente = apis.uri + "/api/ServicioPersonas/GetPersonasDropList" + "?id=" + param;
                var cliente = await httpClient.GetAsync(uriCliente);
                if (cliente.IsSuccessStatusCode)
                {
                    modelPerona = JsonConvert.DeserializeObject<List<DropListModel>>(await cliente.Content.ReadAsStringAsync());
                    ViewBag.personaId = new SelectList(modelPerona, "id", "text", model.personaId);
                }
            }
            #endregion
            #endregion
            return View(model);
        }

        [Route("[action]")]
        [HttpGet]
        public ActionResult GetUsuarios()
        {
            return View();
        }
    }
}
