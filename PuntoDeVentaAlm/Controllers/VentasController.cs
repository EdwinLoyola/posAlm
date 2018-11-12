using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PuntoDeVentaAlm.Controllers
{
    public class VentasController : Controller
    {
        // GET: Ventas
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}