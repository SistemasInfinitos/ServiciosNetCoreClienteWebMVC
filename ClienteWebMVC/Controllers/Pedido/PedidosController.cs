using ClienteWebMVC.Configuration;
using ClienteWebMVC.Models.Comun;
using ClienteWebMVC.Models.Pedido;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Identity.Web;

namespace ClienteWebMVC.Controllers.Pedido
{
    [Route("web/ped")]
    public class PedidosController : Controller
    {
        private readonly JwtConfiguracion _jwtConfig;
        readonly ITokenAcquisition tokenAcquisition;

        public PedidosController(IOptionsMonitor<JwtConfiguracion> optionsMonitor/*, ITokenAcquisition tokenAcquisition*/)
        {
            this._jwtConfig = optionsMonitor.CurrentValue;
            //this.tokenAcquisition = tokenAcquisition;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> Gestion(string id)
        {

            //var tok = Request.Querystring("id");
            //var tok2 = Request.ServerVariables("id");


            // esta es una forma de trabajar, aumenta la seguridad pero tanbien el tiempo de desarrollo
            var httpClient = new HttpClient();
            string accessToken = "";
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            List<JwtConfiguracionAPI> api = new List<JwtConfiguracionAPI>();
            api = _jwtConfig.api; // esto garantiza la migracion a produccion ya que la url siempre cambia
            EncabezadoPedidosModel model = new EncabezadoPedidosModel();
            List<DropListModel> modelPerona = new List<DropListModel>();

            ViewBag.idString = "";
            var apis = api.Where(x => x.servicio == "ServiciosNetCore").FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(id))
            {
                ViewBag.idString = id;
                string endpoint = "api/ServicioPedidos/GetPedido";
                string parmetro = id;
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
            //si entra en el anterio bloque SelectList seleccionara el actual
            #region DropDownList
            #region Personas
            ViewBag.clientePersonaId = new SelectList(modelPerona, "id", "text");
            if (model.clientePersonaId > 0)
            {
                var apis2 = api.Where(x => x.servicio == "ServicioPersonas").FirstOrDefault();
                int param = model.clientePersonaId;
                //se establece la parsona para que no traiga mas de uno ya que hay un buscar ajax dinamico
                string uriCliente = apis2.uri + "/api/ServicioPersonas/GetPersonasDropList" + "?id=" + param;
                var cliente = await httpClient.GetAsync(uriCliente);
                if (cliente.IsSuccessStatusCode)
                {
                    modelPerona = JsonConvert.DeserializeObject<List<DropListModel>>(await cliente.Content.ReadAsStringAsync());
                    ViewBag.clientePersonaId = new SelectList(modelPerona, "id", "text", model.clientePersonaId);
                }
            }
            #endregion
            #endregion
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
