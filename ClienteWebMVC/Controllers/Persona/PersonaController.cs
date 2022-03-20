using ClienteWebMVC.Configuration;
using ClienteWebMVC.Models.Persona;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public async Task<ActionResult> Gestion(string id)
        {
            // esta es una forma de trabajar, aumenta la seguridad pero tanbien el tiempo de desarrollo
            var httpClient = new HttpClient();
            List<JwtConfiguracionAPI>api = _jwtConfig.api; // esto garantiza la migracion a produccion ya que la url siempre cambia
            PersonasModel model = new PersonasModel();
            ViewBag.idString = "";

            if (!string.IsNullOrWhiteSpace(id))
            {
                ViewBag.idString = id;
                string endpoint = "api/Personas/GetPersona";
                string parmetro = id;
                string uri = api[0]+ "/" + endpoint + "?id=" + parmetro;

                //var data = new StringContent("objt_json", Encoding.UTF8, "application/json");
                //var response =await  httpClient.PostAsync(uri, data);
                try
                {
                    var response = await httpClient.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        model = JsonConvert.DeserializeObject<PersonasModel>(await response.Content.ReadAsStringAsync());
                        //var options = new JsonSerializerOptions { IncludeFields = true };
                        //model = JsonSerializer.DeserializeAsync<PersonasModel>(await response.Content.ReadAsStringAsync(), options);
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
            #endregion
            return View(model);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> GetPersonas()
        {
            return View();
        }
    }
}
