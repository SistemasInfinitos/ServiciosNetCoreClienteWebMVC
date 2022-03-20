using ClienteWebMVC.Configuration;
using ClienteWebMVC.Models.Pedido;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClienteWebMVC.Controllers.Pedido
{
    [Route("web/ped")]
    public class PedidosController : Controller
    {
        private readonly JwtConfiguracion _jwtConfig;
        public PedidosController(IOptionsMonitor<JwtConfiguracion> optionsMonitor)
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
            EncabezadoPedidosModel model = new EncabezadoPedidosModel();
            ViewBag.idString = "";

            if (!string.IsNullOrWhiteSpace(id))
            {
                ViewBag.idString = id;
                string endpoint = "api/ServicioPedidos/GetPedido";
                string parmetro = id;
                var apis = api.Where(x => x.servicio == "ServiciosNetCore").FirstOrDefault();
                string uri = apis.uri + "/" + endpoint + "?id=" + parmetro;

                try
                {
                    var response = await httpClient.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        model = JsonConvert.DeserializeObject<EncabezadoPedidosModel>(await response.Content.ReadAsStringAsync());
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
        public ActionResult GetPedidos()
        {
            return View();
        }
    }
}
