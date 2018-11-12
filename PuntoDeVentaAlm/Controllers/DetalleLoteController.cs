using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PuntoDeVentaAlm.Models;

namespace PuntoDeVentaAlm.Controllers
{
    [Authorize(Users = "eloyola@ejemplo.com")]
    public class DetalleLoteController : Controller
    {
        private ALMENDRITAEntities db = new ALMENDRITAEntities();

        // GET: DetalleLote
        public ActionResult Index()
        {
            var tBL_DETALLE_LOTE = db.TBL_DETALLE_LOTE.Include(t => t.TBL_LOTE_PRODUCCION);
            return View(tBL_DETALLE_LOTE.ToList());
        }

        public ActionResult IndexLote()
        {
            return RedirectToAction("Index", "Lote");
        }

        // GET: DetalleLote/Details/5
        public ActionResult Details(int? id2,string producto,string idLote)
        {
            if (id2 == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_DETALLE_LOTE tBL_DETALLE_LOTE = db.TBL_DETALLE_LOTE.Find(id2);
            if (tBL_DETALLE_LOTE == null)
            {
                return HttpNotFound();
            }

            ViewBag.Lote = idLote;
            ViewBag.Prodcuto = producto;
            return View(tBL_DETALLE_LOTE);
        }

        // GET: DetalleLote/Create
        public ActionResult Create(string id2, string idL)
        {
            if (id2 == null || idL == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TBL_LOTE_PRODUCCION tBL_LOTE_PRODUCCION = db.TBL_LOTE_PRODUCCION.Find(idL);
            if (tBL_LOTE_PRODUCCION == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdLote = idL;
            ViewBag.ID_LOTE = new SelectList(db.TBL_LOTE_PRODUCCION.Where(x => x.ID_LOTE.Equals(idL)), "ID_LOTE","ID_LOTE");
            ViewBag.PRODUCTONAME = db.TBL_PRODUCTOS.Where(x => x.ID_PRODUCTO.Equals(id2)).Select(x => x.DESCRIPCION).ToList()[0];
            return View();

        }



        // POST: DetalleLote/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CANTIDAD,ID_LOTE")] TBL_DETALLE_LOTE tBL_DETALLE_LOTE)
        {
            tBL_DETALLE_LOTE.VEDIDOS = 0;
            tBL_DETALLE_LOTE.FECHA_REGISTRO = DateTime.Now;
            tBL_DETALLE_LOTE.ID_USER_ASP = HttpContext.User.Identity.Name;
            tBL_DETALLE_LOTE.DISPONIBLE = true;

            if (ModelState.IsValid)
            {
                db.TBL_DETALLE_LOTE.Add(tBL_DETALLE_LOTE);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_LOTE = new SelectList(db.TBL_LOTE_PRODUCCION, "ID_LOTE", "ID_PRODUC", tBL_DETALLE_LOTE.ID_LOTE);
            return View(tBL_DETALLE_LOTE);
        }

        public bool Create2([Bind(Include = "CANTIDAD,ID_LOTE,ID_USER_ASP")] TBL_DETALLE_LOTE tBL_DETALLE_LOTE)
        {
            tBL_DETALLE_LOTE.VEDIDOS = 0;
            tBL_DETALLE_LOTE.FECHA_REGISTRO = DateTime.Now;
            tBL_DETALLE_LOTE.DISPONIBLE = true;

            if (ModelState.IsValid)
            {
                db.TBL_DETALLE_LOTE.Add(tBL_DETALLE_LOTE);
                db.SaveChanges();
                return true;
            }
            return false;
        }

        // GET: DetalleLote/Edit/5
        public ActionResult Edit(string id2, string idL)
        {
            try
            {
                if (idL == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                int idDet = db.TBL_DETALLE_LOTE.Where(x => x.ID_LOTE.Equals(idL)).Select(x => x.ID_DETALLE).ToList()[0];
                TBL_DETALLE_LOTE tBL_DETALLE_LOTE = db.TBL_DETALLE_LOTE.Find(idDet);
                if (tBL_DETALLE_LOTE == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Lote = idL;
                ViewBag.Producto = db.TBL_LOTE_PRODUCCION.Where(x => x.ID_LOTE.Equals(idL)).Select(x => x.DESCRIPCION).ToList()[0];
                ViewBag.ID_LOTE = new SelectList(db.TBL_LOTE_PRODUCCION, "ID_LOTE", "ID_PRODUC", tBL_DETALLE_LOTE.ID_LOTE);
                return View(tBL_DETALLE_LOTE);
            }
            catch(Exception) 
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        // POST: DetalleLote/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_DETALLE,CANTIDAD")] TBL_DETALLE_LOTE tBL_DETALLE_LOTE)
        {
            var original = db.TBL_DETALLE_LOTE.AsNoTracking().Where(x => x.ID_DETALLE == tBL_DETALLE_LOTE.ID_DETALLE).ToList();

            if(original.Count > 0)
            {
                tBL_DETALLE_LOTE.ID_LOTE = original[0].ID_LOTE;
                tBL_DETALLE_LOTE.FECHA_REGISTRO = DateTime.Now;
                tBL_DETALLE_LOTE.DISPONIBLE = false;
                tBL_DETALLE_LOTE.VEDIDOS = original[0].VEDIDOS;
                tBL_DETALLE_LOTE.ID_USER_ASP = HttpContext.User.Identity.Name;
                //tBL_DETALLE_LOTE.TBL_LOTE_PRODUCCION = original[0].TBL_LOTE_PRODUCCION;

                if (ModelState.IsValid)
                {
                    db.Entry(tBL_DETALLE_LOTE).State = EntityState.Modified;
                    //se general carga la entrada de mercancia en la tabla STOCK
                    var idProducto = (db.TBL_LOTE_PRODUCCION.AsNoTracking().Where(x => x.ID_LOTE == tBL_DETALLE_LOTE.ID_LOTE).Select(x => x.ID_PRODUC).ToList());
                    int idAlmacen = 2;
                    string produc = idProducto[0];
                    var originalStock = (db.TBL_STOCK.Where(x => x.ID_PRODUC.Equals(produc)).Where(x => x.ID_ALMAC == idAlmacen)).ToList();

                    if (originalStock.Count == 0)
                    {
                        TBL_STOCK nuevoStock = new TBL_STOCK() { ID_PRODUC = produc, ID_ALMAC = idAlmacen,STOCK = tBL_DETALLE_LOTE.CANTIDAD, PEDIDO=0,COMPROMETIDO=0,ULTIMA_FECHA_ACTUALIZACION=DateTime.Now};
                        db.TBL_STOCK.Add(nuevoStock);
                        db.SaveChanges();
                    }
                    else if (originalStock.Count == 1)
                    {
                        originalStock[0].STOCK += tBL_DETALLE_LOTE.CANTIDAD;
                        //db.Entry(originalStock[0]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        TempData["Mensaje"] = "<div class=\"alert alert-danger\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button><strong>No se pudo generar STOCK, favor de intentarlo de nuevo..</strong> </div>";
                        return RedirectToAction("Index", "Lote");
                    }

                    TempData["Mensaje"] = "<div class=\"alert alert-success\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button><strong>STOCK del LOTE generado exitosamente.</strong> </div>";
                    return RedirectToAction("Index","Lote");
                }
                TempData["Mensaje"] = "<div class=\"alert alert-danger\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button><strong>No se pudo generar STOCK, favor de intentarlo de nuevo.</strong> </div>";
                return RedirectToAction("Index", "Lote");
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        //Todo stock generado desde un lote, sera cargado al almacen "MOLIDOS ALMACEN" con un ID_ALMACEN=2
        //public bool generaStock(string ID_PRODUC, float STOCK, int ID_ALMAC)
        //{
        //    var original = db.TBL_STOCK.Where(x => x.ID_PRODUC.Equals(ID_PRODUC)).Where(x => x.ID_ALMAC == 2).ToList();

        //    if (original.Count == 0)
        //    {
        //        //db.TBL_STOCK.Add();
        //        db.SaveChanges();
        //        return true;
        //    }
        //    else if (original.Count == 1)
        //    {
        //        original[0].STOCK = original[0].STOCK += tBL_STOCK.STOCK;
        //        original[0].ULTIMA_FECHA_ACTUALIZACION = DateTime.Now;
        //        db.SaveChanges();
        //        return true;
        //    }
        //    return false;
        //}

        // GET: DetalleLote/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_DETALLE_LOTE tBL_DETALLE_LOTE = db.TBL_DETALLE_LOTE.Find(id);
            if (tBL_DETALLE_LOTE == null)
            {
                return HttpNotFound();
            }
            return View(tBL_DETALLE_LOTE);
        }

        // POST: DetalleLote/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TBL_DETALLE_LOTE tBL_DETALLE_LOTE = db.TBL_DETALLE_LOTE.Find(id);
            db.TBL_DETALLE_LOTE.Remove(tBL_DETALLE_LOTE);
            db.SaveChanges();
            return RedirectToAction("Index");
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
