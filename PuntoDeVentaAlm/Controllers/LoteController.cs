using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PuntoDeVentaAlm.Models;
using PuntoDeVentaAlm.Utilerias;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.ReportSource;
using System.IO;

namespace PuntoDeVentaAlm.Controllers
{
    [Authorize(Users = "eloyola@ejemplo.com")]
    public class LoteController : Controller
    {
        private ALMENDRITAEntities db = new ALMENDRITAEntities();

        // GET: Lote
        public ActionResult Index()
        {
            if(TempData["Mensaje"] != null)
            ViewBag.Salida = TempData["Mensaje"].ToString();

            var tBL_LOTE_PRODUCCION = db.TBL_LOTE_PRODUCCION.Where(x => x.ABIERTO == true).Where(x => x.CANCELADO == false);
            return View(tBL_LOTE_PRODUCCION.ToList());
        }

        // GET: Lote/Details/5
        public ActionResult DetailsLote(string id,string product)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                product = db.TBL_PRODUCTOS.Where(y => y.ID_PRODUCTO.Equals(product)).Select(y => y.DESCRIPCION).ToList()[0];
                return RedirectToAction("Details", "DetalleLote", routeValues: new { id2 = db.TBL_DETALLE_LOTE.Where(x => x.ID_LOTE.Equals(id)).Select(x => x.ID_DETALLE).ToList()[0], producto = product,idLote=id });
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }


        // GET: Lote/Create
        public ActionResult Create()
        {
            ViewBag.DESCRIPCION = new SelectList(db.TBL_PRESENTACIONES_PRODUCT, "DESCRIPCION", "DESCRIPCION");
            return View();
        }

        public ActionResult RedireccionaCreate(string id, string idLot)
        {
            try
            {
                var validaDetalle = db.TBL_DETALLE_LOTE.Where(x => x.ID_LOTE.Equals(idLot)).Select(x => x.DISPONIBLE).ToList()[0];
                var cancelado = db.TBL_LOTE_PRODUCCION.Where(x => x.ID_LOTE.Equals(idLot)).Select(x => x.CANCELADO).ToList()[0];
                if (validaDetalle == true && cancelado==false)
                    return RedirectToAction("Edit", "DetalleLote", routeValues: new { id2 = id, idL = idLot });
                else
                {
                    TempData["Mensaje"] = "<div class=\"alert alert-warning\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button><strong>No se puede editar Stock. Consulte la sección DETALLES</strong> </div>";
                    return RedirectToAction("Index");
                }
                
            }
            catch(Exception)
            {
                return HttpNotFound();
            }
        }

        // POST: Lote/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DESCRIPCION,COSTO_PRODUCCION,FECHA_PRODUC")] TBL_LOTE_PRODUCCION tBL_LOTE_PRODUCCION)
        {
            //ID_LOTE,FECHA_REGISTRO,ABIERTO,ID_USU
            Utilerias.UtileriaComun aux = new Utilerias.UtileriaComun();
            tBL_LOTE_PRODUCCION.ID_LOTE = aux.GeneraIdProduccion();
            if (tBL_LOTE_PRODUCCION.ID_LOTE.Length == 8)
            {
                tBL_LOTE_PRODUCCION.FECHA_REGISTRO = DateTime.Now;
                tBL_LOTE_PRODUCCION.ABIERTO = true;
                tBL_LOTE_PRODUCCION.ID_USER_ASP = HttpContext.User.Identity.Name;
                tBL_LOTE_PRODUCCION.CANCELADO = false;
                var idProducto = db.TBL_PRESENTACIONES_PRODUCT.Where(x => x.DESCRIPCION.Equals(tBL_LOTE_PRODUCCION.DESCRIPCION)).Select(x => x.ID_PRODUCTO).ToList();
                tBL_LOTE_PRODUCCION.ID_PRODUC = idProducto[0];

                if (tBL_LOTE_PRODUCCION.COSTO_PRODUCCION == null)
                    tBL_LOTE_PRODUCCION.COSTO_PRODUCCION = 0;

                DetalleLoteController dlote = new DetalleLoteController();
                TBL_DETALLE_LOTE modelDetalle = new TBL_DETALLE_LOTE();
                modelDetalle.ID_LOTE = tBL_LOTE_PRODUCCION.ID_LOTE;
                modelDetalle.ID_USER_ASP = HttpContext.User.Identity.Name;
                modelDetalle.CANTIDAD = 0;
                modelDetalle.DISPONIBLE = true;

                if (ModelState.IsValid)
                {
                    db.TBL_LOTE_PRODUCCION.Add(tBL_LOTE_PRODUCCION);
                    db.SaveChanges();
                    if (dlote.Create2(modelDetalle))
                    {
                        ViewBag.Confirma = "<div class=\"alert alert-success\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button><strong>"+tBL_LOTE_PRODUCCION.ID_LOTE+" generado exitosamente.</strong> </div>";
                        return RedirectToAction("Index");
                    }
                    else
                        if(DeleteConfirmed2(tBL_LOTE_PRODUCCION.ID_LOTE))
                        {
                            ViewBag.Confirma = "<div class=\"alert alert-danger\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button><strong>Error al crear nuevo LOTE, favor de intentarlo de nuevo.</strong> </div>";
                            return RedirectToAction("Index");
                        }
                }
            }
            else
            {
                ViewBag.ID_PRODUC = new SelectList(db.TBL_PRODUCTOS, "ID_PRODUCTO", "DESCRIPCION", tBL_LOTE_PRODUCCION.ID_PRODUC);
                return View(tBL_LOTE_PRODUCCION);
            }

            ViewBag.ID_PRODUC = new SelectList(db.TBL_PRODUCTOS, "ID_PRODUCTO", "DESCRIPCION", tBL_LOTE_PRODUCCION.ID_PRODUC);
            return View(tBL_LOTE_PRODUCCION);
        }

        // GET: Lote/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_LOTE_PRODUCCION tBL_LOTE_PRODUCCION = db.TBL_LOTE_PRODUCCION.Find(id);
            if (tBL_LOTE_PRODUCCION == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_PRODUC = new SelectList(db.TBL_PRODUCTOS, "ID_PRODUCTO", "DESCRIPCION", tBL_LOTE_PRODUCCION.ID_PRODUC);
            return View(tBL_LOTE_PRODUCCION);
        }

        // POST: Lote/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_LOTE,COSTO_PRODUCCION,FECHA_PRODUC,FECHA_REGISTRO,ID_PRODUC,ABIERTO,ID_USU")] TBL_LOTE_PRODUCCION tBL_LOTE_PRODUCCION)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tBL_LOTE_PRODUCCION).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_PRODUC = new SelectList(db.TBL_PRODUCTOS, "ID_PRODUCTO", "DESCRIPCION", tBL_LOTE_PRODUCCION.ID_PRODUC);
            return View(tBL_LOTE_PRODUCCION);
        }

        // GET: Lote/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_LOTE_PRODUCCION tBL_LOTE_PRODUCCION = db.TBL_LOTE_PRODUCCION.Find(id);
            if (tBL_LOTE_PRODUCCION == null)
            {
                return HttpNotFound();
            }
            return View(tBL_LOTE_PRODUCCION);
        }

        // POST: Lote/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            TBL_LOTE_PRODUCCION tBL_LOTE_PRODUCCION = db.TBL_LOTE_PRODUCCION.Find(id);
            db.TBL_LOTE_PRODUCCION.Remove(tBL_LOTE_PRODUCCION);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public bool DeleteConfirmed2(string id)
        {
            TBL_LOTE_PRODUCCION tBL_LOTE_PRODUCCION = db.TBL_LOTE_PRODUCCION.Find(id);
            db.TBL_LOTE_PRODUCCION.Remove(tBL_LOTE_PRODUCCION);
            db.SaveChanges();
            return true;
        }

        [HttpPost]
        public JsonResult cancelaLote (string idLot)
        {
            try
            {
                var lot = db.TBL_LOTE_PRODUCCION.Find(idLot);
                var detLot = db.TBL_DETALLE_LOTE.Where(x => x.ID_LOTE.Equals(idLot)).ToList()[0];
                UtileriaComun aux = new UtileriaComun();
                if (detLot.VEDIDOS == 0 && aux.bajaStock(lot.ID_PRODUC, detLot.CANTIDAD, 2) )
                {
                    lot.CANCELADO = true;
                    db.SaveChanges();
                    return Json(true);
                }
                else
                    return Json(false);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }

        public FileResult tarjetonLote(string id,string product)
        {
            //var ruta = Server.MapPath("");
            string ruta = @"C:\Users\Sistemas\Pictures\qrLotes\" + id + ".png";
            var l = db.TBL_LOTE_PRODUCCION.Where(x=> x.ID_LOTE.Equals(id)).ToList();
            string mensaje = String.Empty;

            if( l!= null)
                mensaje = id + "," + product+","+l[0].DESCRIPCION+","+l[0].FECHA_PRODUC;

            using(UtileriaComun util = new UtileriaComun())
            {
                util.qrLote(mensaje,id);
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/tarProduccion.rpt")));

            rd.SetParameterValue("Producto", l[0].DESCRIPCION);
            rd.SetParameterValue("LOTE",id);
            rd.SetParameterValue("ruta",ruta);
            rd.SetParameterValue("FECHA",l[0].FECHA_PRODUC);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0,SeekOrigin.Begin);

            return File(stream,"application/pdf","Tarjeton "+id+".pdf");
            //return File(ruta,"application/png",id+".png");
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
