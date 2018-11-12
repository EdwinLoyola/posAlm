using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PuntoDeVentaAlm.Controllers
{
    [Authorize(Users = "eloyola@ejemplo.com")]
    public class AdministrarController : Controller
    {
        // GET: Administrar
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}