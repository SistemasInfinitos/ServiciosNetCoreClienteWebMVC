using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClienteWebMVC.Controllers.Pedido
{
    public class PedidosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
