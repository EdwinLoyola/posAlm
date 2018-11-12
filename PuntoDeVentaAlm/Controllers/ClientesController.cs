using PuntoDeVentaAlm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PuntoDeVentaAlm.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult buscaCliente(string term)
        {
            try
            {
                using (ALMENDRITAEntities db = new ALMENDRITAEntities())
                {
                    var lClientes = db.TBL_CLIENTES.AsNoTracking().Where(x => x.RAZON_SOCIAL.Contains(term)).Select(x => new { label = x.RAZON_SOCIAL, value = new { id = x.ID_CLIENTE, nombre = x.RAZON_SOCIAL, rfc = x.RFC, lCredito = x.LIMITE_CREDITO, dCredito = x.DIAS_CREDITO } }).ToArray();
                    return Json(lClientes, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult buscaDireccion(int idCliente)
        {
            try
            {
                using (ALMENDRITAEntities db = new ALMENDRITAEntities())
                {
                    var direccion = db.TBL_DIRECCIONES.AsNoTracking().Where(x => x.ID_CLIENTE == idCliente).Select(x => new { idDir = x.ID_DIRECCION, calle = x.CALLE, num = x.NUMERO, col = x.COLONIA, municipio = x.MUNICIPIO, est = x.ESTADO, tel = x.TELEFONO, correo = x.CORREO  }).ToArray();
                    return Json(direccion, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}
