using ClienteWebMVC.Configuration;
using ClienteWebMVC.Models.Persona;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClienteWebMVC.Controllers.Persona
{
    [Route("web/per")]
    public class PersonaController : Controller
    {
        private readonly JwtConfiguracion _jwtConfig;
        public PersonaController(IOptionsMonitor<JwtConfiguracion> optionsMonitor)
        {
            this._jwtConfig = optionsMonitor.CurrentValue;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> Gestion(string id,string accessToken)
        {
            // esta es una forma de trabajar, aumenta la seguridad pero tanbien el tiempo de desarrollo
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            List<JwtConfiguracionAPI> api = new List<JwtConfiguracionAPI>();
            api = _jwtConfig.api; // esto garantiza la migracion a produccion ya que la url siempre cambia
            PersonasModel model = new PersonasModel();
            ViewBag.idString = "";

            if (!string.IsNullOrWhiteSpace(id))
            {
                ViewBag.idString = id;
                string endpoint = "api/ServicioPersonas/GetPersona";
                string parmetro = id;
                var apis = api.Where(x => x.servicio == "ServicioPersonas").FirstOrDefault();
                string uri = apis.uri + "/" + endpoint + "?id=" + parmetro;

                try
                {
                    var response = await httpClient.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        model = JsonConvert.DeserializeObject<PersonasModel>(await response.Content.ReadAsStringAsync());
                    }
                }
                catch (Exception X)
                {
                    string mensaje = X.Message;
                    throw;
                }
            }
            return View(model);
        }

        [Route("[action]")]
        [HttpGet]
        public ActionResult GetPersonas()
        {
            return View();
        }
    }
}
