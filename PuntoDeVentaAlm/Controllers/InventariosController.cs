using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PuntoDeVentaAlm.Models;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.ReportSource;
using System.IO;

namespace PuntoDeVentaAlm.Controllers
{
    [Authorize]
    public class InventariosController : Controller
    {
        private ALMENDRITAEntities db = new ALMENDRITAEntities();

        // GET: Inventarios
        public ActionResult Index()
        {
            return View(db.TBL_INVENTARIOS.ToList());
        }

        // GET: Inventarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_INVENTARIOS tBL_INVENTARIOS = db.TBL_INVENTARIOS.Find(id);
            if (tBL_INVENTARIOS == null)
            {
                return HttpNotFound();
            }
            return View(tBL_INVENTARIOS);
        }

        // GET: Inventarios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Inventarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_INVENTARIO,FECHA,OBSERVACIONES,ESTATUS,ID_ALMACEN,ID_USER_ASP")] TBL_INVENTARIOS tBL_INVENTARIOS)
        {
            if (ModelState.IsValid)
            {
                db.TBL_INVENTARIOS.Add(tBL_INVENTARIOS);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tBL_INVENTARIOS);
        }

        // GET: Inventarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_INVENTARIOS tBL_INVENTARIOS = db.TBL_INVENTARIOS.Find(id);
            if (tBL_INVENTARIOS == null)
            {
                return HttpNotFound();
            }
            return View(tBL_INVENTARIOS);
        }

        // POST: Inventarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_INVENTARIO,OBSERVACIONES")] TBL_INVENTARIOS tBL_INVENTARIOS)
        {
            var original = db.TBL_INVENTARIOS.Find(tBL_INVENTARIOS.ID_INVENTARIO);
            original.OBSERVACIONES = tBL_INVENTARIOS.OBSERVACIONES;
            if (ModelState.IsValid)
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tBL_INVENTARIOS);
        }

        // GET: Inventarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_INVENTARIOS tBL_INVENTARIOS = db.TBL_INVENTARIOS.Find(id);
            if (tBL_INVENTARIOS == null)
            {
                return HttpNotFound();
            }
            return View(tBL_INVENTARIOS);
        }

        // POST: Inventarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TBL_INVENTARIOS tBL_INVENTARIOS = db.TBL_INVENTARIOS.Find(id);
            db.TBL_INVENTARIOS.Remove(tBL_INVENTARIOS);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public PartialViewResult nuevoInventario()
        {
            ViewBag.ID_ALMACEN = new SelectList(db.TBL_ALMACENES.Where(x => x.ESTATUS == true),"ID_ALMACEN","NOM_ALMACEN");
            ViewBag.Mensaje = "";
            return PartialView("_nuevoInvent");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult crearInvent([Bind (Include="ID_ALMACEN")]TBL_INVENTARIOS tBL_INVENTARIOS)
        {
            try
            {
                tBL_INVENTARIOS.FECHA = DateTime.Now;
                tBL_INVENTARIOS.ESTATUS = true;
                tBL_INVENTARIOS.ID_USER_ASP = HttpContext.User.Identity.Name;
                var valida = db.TBL_INVENTARIOS.Where(x => x.ID_ALMACEN == tBL_INVENTARIOS.ID_ALMACEN && x.ESTATUS == true).ToList();

                if (ModelState.IsValid && valida.Count()==0)
                {
                    db.TBL_INVENTARIOS.Add(tBL_INVENTARIOS);
                    db.SaveChanges();
                    ViewBag.ID_ALMACEN = new SelectList(db.TBL_ALMACENES.Where(x => x.ESTATUS == true), "ID_ALMACEN", "NOM_ALMACEN");
                    ViewBag.Mensaje = "<div class=\"alert alert-success\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button><strong>Nuevo inventario generado exitosamente.</strong> </div>";
                    return PartialView("_nuevoInvent");
                }
                else
                {
                    ViewBag.ID_ALMACEN = new SelectList(db.TBL_ALMACENES.Where(x => x.ESTATUS == true), "ID_ALMACEN", "NOM_ALMACEN");
                    ViewBag.Mensaje = "<div class=\"alert alert-warning\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button><strong>No se ha cerrado otro inventario sobre este almacenn</strong> </div>";
                    return PartialView("_nuevoInvent");
                }
            }
            catch(Exception)
            {
                ViewBag.ID_ALMACEN = new SelectList(db.TBL_ALMACENES.Where(x => x.ESTATUS == true), "ID_ALMACEN", "NOM_ALMACEN");
                ViewBag.Mensaje = "<div class=\"alert alert-danger\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button><strong>Error al generar Nuevo Inventario, favor de intentar de nuevo.</strong> </div>";
                return PartialView("_nuevoInvent");
            }
        }

        public ActionResult CargaInventario()
        {
            //Creamos variable alm para almacenar el id_almacen que se le asigno al usuario. usamos USING para recuperar dicho almacen del usuario
            int idAlm;
            using (ApplicationDbContext user = new ApplicationDbContext())
            {
                idAlm = user.Users.AsNoTracking().Where( x=> x.UserName.Equals(HttpContext.User.Identity.Name)).Select(x => x.ID_ALMACEN).ToArray()[0];
            }

            var inventActivo = db.TBL_INVENTARIOS.AsNoTracking().Where(x => x.ID_ALMACEN == idAlm && x.ESTATUS == true).ToList();

            if (inventActivo.Count == 1)
            {
                TBL_INVENTARIOS tBL_INVENTARIOS = db.TBL_INVENTARIOS.Find(inventActivo[0].ID_INVENTARIO);
                return View("CargaInventario",tBL_INVENTARIOS);
                //return View("CargaInventario",db.TBL_DETALLES_INVENT.Where( x => x.ID_INVENTARIO == inventActivo[0].ID_INVENTARIO).ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public PartialViewResult _detalleInventario(int idInvent)
        {
            int idAlm = db.TBL_INVENTARIOS.AsNoTracking().Where(x => x.ID_INVENTARIO == idInvent).Select(x => x.ID_ALMACEN).ToList()[0];
            ViewBag.nomAlm = db.TBL_ALMACENES.Where(x => x.ID_ALMACEN == idAlm).Select(x => x.NOM_ALMACEN).ToList()[0];
            return PartialView("_trabajoInventario",db.TBL_DETALLES_INVENT.Where(x => x.ID_INVENTARIO == idInvent).ToList());
        }

        public JsonResult BuscarProducto(string term)
        {
            using (ALMENDRITAEntities db = new ALMENDRITAEntities())
            {
                var resultado = db.TBL_PRODUCTOS.Where(x => x.DESCRIPCION.Contains(term)).Where(x => x.ESTATUS == true).Select(x => new { label = x.DESCRIPCION, value = x.DESCRIPCION+"|"+x.ID_PRODUCTO }).ToArray();
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult _registraProducInvent(int idInvent)
        {
            IEnumerable<int> inv = new List<int> { idInvent };
            ViewBag.ID_INVENTARIO = new SelectList(inv);
            return PartialView("_registraInventario"); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult registraProducInvent([Bind(Include = "ID_INVENTARIO,ID_PRODUC,CANTIDAD")] TBL_DETALLES_INVENT tBL_DETALLE_INVENT)
        {
            try
            {
                //VERIFICAR QUE EL PROCESO DE INVENTAREADO NO HA SIDO CERRADO
                var inventAbierto = db.TBL_INVENTARIOS.AsNoTracking().Where(x => x.ID_INVENTARIO == tBL_DETALLE_INVENT.ID_INVENTARIO && x.ESTATUS == true).Select(x => x.ID_ALMACEN).ToArray();
                if(inventAbierto.Count() == 0)
                    return Json(new { result = 4, message = "El proceso de invetario ha sido cerrado por el administrador." }, JsonRequestBehavior.AllowGet);

                //VALIDACION CAMPOS VACIOS
                if (string.IsNullOrEmpty(tBL_DETALLE_INVENT.ID_PRODUC) || tBL_DETALLE_INVENT.CANTIDAD == 0)
                    return Json(new { result = 6, message = "No se ingreso PRODUCTO o CANTIDAD." }, JsonRequestBehavior.AllowGet);

                //OBTENER EL ID DEL PRODUCTO Y VERIFICAR QUE EXISTA EN LA BASE DE DATOS
                using(PuntoDeVentaAlm.Utilerias.UtileriaComun aux = new PuntoDeVentaAlm.Utilerias.UtileriaComun())
                {
                    tBL_DETALLE_INVENT.ID_PRODUC = aux.obtieneIdProduct(tBL_DETALLE_INVENT.ID_PRODUC);
                    if(!aux.existeProducto(tBL_DETALLE_INVENT.ID_PRODUC))
                        return Json(new { result = 3, message = "Producto no existente en la base de datos." }, JsonRequestBehavior.AllowGet);
                }

                //VALIDA QUE EL PRODUCTO EXISTA EN EL STOCK DEL ALMACEN
                int almacen = inventAbierto[0];
                var producAlmac = db.TBL_STOCK.AsNoTracking().Where(x => x.ID_ALMAC == almacen && x.ID_PRODUC.Equals(tBL_DETALLE_INVENT.ID_PRODUC)).Select(x => x.ID_STOCK).ToArray();
                if(producAlmac.Count() == 0)
                    return Json(new { result = 7, message = "El producto no ha sido registrado para este almacen." }, JsonRequestBehavior.AllowGet);

                //VALIDAR QUE EL PRODUCTO NO ESTÉ INVENTAREADO
                if (inventareado(tBL_DETALLE_INVENT.ID_PRODUC, tBL_DETALLE_INVENT.ID_INVENTARIO))
                    return Json(new { result = 5, message = "Producto inventareado, consultar seccion DETALLE INVENTARIO" }, JsonRequestBehavior.AllowGet);

                tBL_DETALLE_INVENT.ID_USER_ASP = HttpContext.User.Identity.Name;
                tBL_DETALLE_INVENT.FECHA_REGISTRO_INV = DateTime.Now;

                //SI EL MODELO ES VALIDO, PROCEDE A HACER EL REGISTRO DEL PRODUCTO A INVENTAREAR
                if (ModelState.IsValid)
                {
                    db.TBL_DETALLES_INVENT.Add(tBL_DETALLE_INVENT);
                    db.SaveChanges();
                    return Json(new { result = 0, message = "Producto inventariado EXITOSAMENTE." },JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { result = 1, message = "Producto no inventareado, favor de intentar de nuevo." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { result = 2, message = "Error al procesaro solicitud, contactar aladministrador del sistema." }, JsonRequestBehavior.AllowGet);
            }
        }

        private bool inventareado(string idProducInv, int idInvent)
        {
            var productoInventareado = db.TBL_DETALLES_INVENT.AsNoTracking().Where(x => x.ID_INVENTARIO == idInvent && x.ID_PRODUC == idProducInv).Select(x => x.ID_DET_INV).ToArray();
            if (productoInventareado.Count() == 1)
                return true;
            return false;
        }

        [HttpPost]
        public JsonResult eliminaRegistro(int idDetInv)
        {
            try
            {
                TBL_DETALLES_INVENT tBL_DETALLE_INVENT = db.TBL_DETALLES_INVENT.Find(idDetInv);
                
                //Valida que exista el registro en la base de datos
                if(tBL_DETALLE_INVENT == null)
                    return Json(new { result = 2, message = "No se encontreo registro." }, JsonRequestBehavior.AllowGet);

                db.TBL_DETALLES_INVENT.Remove(tBL_DETALLE_INVENT);
                db.SaveChanges();
                return Json(new { result = 0, message = "Registro eliminado EXITOSAMENTE." },JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { result = 1, message = "Registro eliminado EXITOSAMENTE." }, JsonRequestBehavior.AllowGet);
            }
        }

        public FileResult reporteInvent (int id)
        {
            try
            {
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/repInventario.rpt")));

                rd.SetParameterValue("idInvent", id);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);

                return File(stream, "application/pdf", "Inventario " + id + ".pdf");
            }
            catch { throw; }
        }

        public JsonResult ajustaInventario(int idInv)
        {
            try
            {
                var detalleInventario = db.TBL_DETALLES_INVENT.AsNoTracking().Where(x => x.ID_INVENTARIO == idInv).ToList();
                var cabeceraInv = db.TBL_INVENTARIOS.Where(x => x.ID_INVENTARIO == idInv).ToArray();
                int almacen = cabeceraInv[0].ID_ALMACEN;
                var stock = db.TBL_STOCK.Where(x => x.ID_ALMAC == almacen).ToList();
                string idProducto = string.Empty;
                float reemplazo=0;

                stock.ForEach(x => { 
                    idProducto = x.ID_PRODUC;
                    var aux = db.TBL_DETALLES_INVENT.AsNoTracking().Where( i => i.ID_INVENTARIO == idInv && i.ID_PRODUC.Equals(idProducto)).ToList();
                    if (aux.Count == 1)
                    {
                        reemplazo = (float)aux[0].CANTIDAD;
                        x.STOCK = reemplazo;
                    }
                });

                cabeceraInv[0].ESTATUS = false;
                db.SaveChanges();


                return Json( new { result = 0, message = "Ajunte de Inventario exitoso." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { result = 2, message = "Ajunte de Inventario fallido, contactar al administrador." }, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
