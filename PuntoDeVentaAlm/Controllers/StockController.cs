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
    public class StockController : Controller
    {
        private ALMENDRITAEntities db = new ALMENDRITAEntities();

        // GET: Stock
        public ActionResult Index()
        {
            var tBL_STOCK = db.TBL_STOCK.Include(t => t.TBL_ALMACENES).Include(t => t.TBL_PRODUCTOS);
            return View(tBL_STOCK.ToList());
        }

        // GET: Stock/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_STOCK tBL_STOCK = db.TBL_STOCK.Find(id);
            if (tBL_STOCK == null)
            {
                return HttpNotFound();
            }
            return View(tBL_STOCK);
        }

        // GET: Stock/Create
        public ActionResult Create()
        {
            ViewBag.ID_ALMAC = new SelectList(db.TBL_ALMACENES.Where(x => x.ESTATUS == true), "ID_ALMACEN", "NOM_ALMACEN");
            //ViewBag.ID_PRODUC = new SelectList(db.TBL_PRODUCTOS, "ID_PRODUCTO", "DESCRIPCION");
            return View();
        }

        // POST: Stock/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_PRODUC,STOCK,ID_ALMAC")] TBL_STOCK tBL_STOCK)
        {
            List<string> id_product = db.TBL_PRODUCTOS.Where(x => x.DESCRIPCION.Equals(tBL_STOCK.ID_PRODUC)).Where(x => x.ESTATUS==true).Select(x => x.ID_PRODUCTO).ToList();

            if (id_product.Count == 1)
            {
                if (tBL_STOCK.STOCK == null)
                {
                    ViewBag.ID_ALMAC = new SelectList(db.TBL_ALMACENES, "ID_ALMACEN", "NOM_ALMACEN", tBL_STOCK.ID_ALMAC);
                    ViewBag.ID_PRODUC = new SelectList(db.TBL_PRODUCTOS, "ID_PRODUCTO", "DESCRIPCION", tBL_STOCK.ID_PRODUC);
                    ViewBag.Confirma = "<div class=\"alert alert-danger\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button><strong>El STOCK no fue ingresado.</strong> </div>";
                    return View(tBL_STOCK);
                }
                tBL_STOCK.ID_PRODUC = id_product.ToArray()[0];
                var valida = db.TBL_STOCK.AsNoTracking().Where(x => x.ID_PRODUC.Equals(tBL_STOCK.ID_PRODUC)).Where(x => x.ID_ALMAC.Equals(tBL_STOCK.ID_ALMAC)).ToList();

                if (valida.Count() == 0 && valida != null)
                {
                    tBL_STOCK.COMPROMETIDO = 0;
                    tBL_STOCK.PEDIDO = 0;
                    tBL_STOCK.ULTIMA_FECHA_ACTUALIZACION = DateTime.Now;
                    if (ModelState.IsValid)
                    {
                        db.TBL_STOCK.Add(tBL_STOCK);
                        db.SaveChanges();
                        ViewBag.ID_ALMAC = new SelectList(db.TBL_ALMACENES, "ID_ALMACEN", "NOM_ALMACEN", tBL_STOCK.ID_ALMAC);
                        ViewBag.ID_PRODUC = new SelectList(db.TBL_PRODUCTOS, "ID_PRODUCTO", "DESCRIPCION", tBL_STOCK.ID_PRODUC);
                        ViewBag.Confirma = "<div class=\"alert alert-success\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button><strong>Stock GENERADO exitosamente.</strong> </div>";
                        return View(tBL_STOCK);
                    }
                }
                else if (valida.Count() == 1)
                {
                    tBL_STOCK.COMPROMETIDO = 0;
                    tBL_STOCK.PEDIDO = 0;
                    tBL_STOCK.ID_ALMAC = (int)valida[0].ID_ALMAC;
                    tBL_STOCK.ID_PRODUC = valida[0].ID_PRODUC.ToString();
                    tBL_STOCK.ULTIMA_FECHA_ACTUALIZACION = DateTime.Now;
                    tBL_STOCK.STOCK = tBL_STOCK.STOCK + (int)valida[0].STOCK;
                    tBL_STOCK.ID_STOCK = (int)valida[0].ID_STOCK;

                    db.Entry(tBL_STOCK).State = EntityState.Modified;
                    db.SaveChanges();

                    ViewBag.ID_ALMAC = new SelectList(db.TBL_ALMACENES, "ID_ALMACEN", "NOM_ALMACEN", tBL_STOCK.ID_ALMAC);
                    ViewBag.ID_PRODUC = new SelectList(db.TBL_PRODUCTOS, "ID_PRODUCTO", "DESCRIPCION", tBL_STOCK.ID_PRODUC);
                    ViewBag.Confirma = "<div class=\"alert alert-success\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button><strong>Stock AGREGADO exitosamente.</strong> </div>";
                    return View(tBL_STOCK);

                }
                else
                {
                    ViewBag.ID_ALMAC = new SelectList(db.TBL_ALMACENES, "ID_ALMACEN", "NOM_ALMACEN", tBL_STOCK.ID_ALMAC);
                    ViewBag.ID_PRODUC = new SelectList(db.TBL_PRODUCTOS, "ID_PRODUCTO", "DESCRIPCION", tBL_STOCK.ID_PRODUC);
                    ViewBag.Confirma = "<div class=\"alert alert-danger\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button><strong>Error al procesar la solicitud.</strong> </div>";
                    return View(tBL_STOCK);
                }
            }
                ViewBag.ID_ALMAC = new SelectList(db.TBL_ALMACENES, "ID_ALMACEN", "NOM_ALMACEN", tBL_STOCK.ID_ALMAC);
                ViewBag.ID_PRODUC = new SelectList(db.TBL_PRODUCTOS, "ID_PRODUCTO", "DESCRIPCION", tBL_STOCK.ID_PRODUC);
                ViewBag.Confirma = "<div class=\"alert alert-danger\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button><strong>El producto no existe en la base de datos.</strong> </div>";
                return View(tBL_STOCK);

        }

        // GET: Stock/Salida
        public ActionResult BajaStock()
        {
            ViewBag.ID_ALMAC = new SelectList(db.TBL_ALMACENES, "ID_ALMACEN", "NOM_ALMACEN");
            ViewBag.ID_PRODUC = new SelectList(db.TBL_PRODUCTOS, "ID_PRODUCTO", "DESCRIPCION");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BajaStock([Bind(Include = "STOCK,ID_PRODUC,ID_ALMAC")] TBL_STOCK tBL_STOCK)
        {
            List<string> id_product = db.TBL_PRODUCTOS.Where(x => x.DESCRIPCION.Equals(tBL_STOCK.ID_PRODUC)).Where(x => x.ESTATUS == true).Select(x => x.ID_PRODUCTO).ToList();

            if (id_product.Count == 1)
            {
                if (tBL_STOCK.STOCK == null)
                {
                    ViewBag.ID_ALMAC = new SelectList(db.TBL_ALMACENES, "ID_ALMACEN", "NOM_ALMACEN", tBL_STOCK.ID_ALMAC);
                    ViewBag.ID_PRODUC = new SelectList(db.TBL_PRODUCTOS, "ID_PRODUCTO", "DESCRIPCION", tBL_STOCK.ID_PRODUC);
                    ViewBag.Confirma = "<div class=\"alert alert-danger\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button><strong>El STOCK no fue ingresado.</strong> </div>";
                    return View(tBL_STOCK);
                }

                tBL_STOCK.ID_PRODUC = id_product.ToArray()[0];
                var valida = db.TBL_STOCK.AsNoTracking().Where(x => x.ID_PRODUC.Equals(tBL_STOCK.ID_PRODUC)).Where(x => x.ID_ALMAC.Equals(tBL_STOCK.ID_ALMAC)).ToList();

                if (valida.Count() == 0)
                {
                    
                        ViewBag.ID_ALMAC = new SelectList(db.TBL_ALMACENES, "ID_ALMACEN", "NOM_ALMACEN", tBL_STOCK.ID_ALMAC);
                        ViewBag.ID_PRODUC = new SelectList(db.TBL_PRODUCTOS, "ID_PRODUCTO", "DESCRIPCION", tBL_STOCK.ID_PRODUC);
                        ViewBag.Confirma = "<div class=\"alert alert-warning\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button><strong>No existe STOCK en el almacen.</strong> </div>";
                        return View(tBL_STOCK);
                }
                else if (valida.Count() == 1)
                {
                    if ((int)valida[0].STOCK > tBL_STOCK.STOCK)
                    {
                        tBL_STOCK.COMPROMETIDO = 0;
                        tBL_STOCK.PEDIDO = 0;
                        tBL_STOCK.ID_ALMAC = (int)valida[0].ID_ALMAC;
                        tBL_STOCK.ID_PRODUC = valida[0].ID_PRODUC.ToString();
                        tBL_STOCK.ULTIMA_FECHA_ACTUALIZACION = DateTime.Now;
                        tBL_STOCK.STOCK = (int)valida[0].STOCK - tBL_STOCK.STOCK;
                        tBL_STOCK.ID_STOCK = (int)valida[0].ID_STOCK;

                        db.Entry(tBL_STOCK).State = EntityState.Modified;
                        db.SaveChanges();

                        ViewBag.ID_ALMAC = new SelectList(db.TBL_ALMACENES, "ID_ALMACEN", "NOM_ALMACEN", tBL_STOCK.ID_ALMAC);
                        ViewBag.ID_PRODUC = new SelectList(db.TBL_PRODUCTOS, "ID_PRODUCTO", "DESCRIPCION", tBL_STOCK.ID_PRODUC);
                        ViewBag.Confirma = "<div class=\"alert alert-success\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button><strong>Stock DESCONTADO exitosamente.</strong> </div>";
                        return View(tBL_STOCK);
                    }
                    else
                    {
                        ViewBag.ID_ALMAC = new SelectList(db.TBL_ALMACENES, "ID_ALMACEN", "NOM_ALMACEN", tBL_STOCK.ID_ALMAC);
                        ViewBag.ID_PRODUC = new SelectList(db.TBL_PRODUCTOS, "ID_PRODUCTO", "DESCRIPCION", tBL_STOCK.ID_PRODUC);
                        ViewBag.Confirma = "<div class=\"alert alert-warning\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button><strong>El STOCK no puede ser negativo.</strong> </div>";
                        return View(tBL_STOCK);
                    }

                }
                else
                {
                    ViewBag.ID_ALMAC = new SelectList(db.TBL_ALMACENES, "ID_ALMACEN", "NOM_ALMACEN", tBL_STOCK.ID_ALMAC);
                    ViewBag.ID_PRODUC = new SelectList(db.TBL_PRODUCTOS, "ID_PRODUCTO", "DESCRIPCION", tBL_STOCK.ID_PRODUC);
                    ViewBag.Confirma = "<div class=\"alert alert-danger\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button><strong>Error al procesar la solicitud.</strong> </div>";
                    return View(tBL_STOCK);
                }
            }
            ViewBag.ID_ALMAC = new SelectList(db.TBL_ALMACENES, "ID_ALMACEN", "NOM_ALMACEN", tBL_STOCK.ID_ALMAC);
            ViewBag.ID_PRODUC = new SelectList(db.TBL_PRODUCTOS, "ID_PRODUCTO", "DESCRIPCION", tBL_STOCK.ID_PRODUC);
            ViewBag.Confirma = "<div class=\"alert alert-danger\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button><strong>El producto no existe en la base de datos.</strong> </div>";
            return View(tBL_STOCK);

        }

        // GET: Stock/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_STOCK tBL_STOCK = db.TBL_STOCK.Find(id);
            if (tBL_STOCK == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_ALMAC = new SelectList(db.TBL_ALMACENES, "ID_ALMACEN", "NOM_ALMACEN", tBL_STOCK.ID_ALMAC);
            ViewBag.ID_PRODUC = new SelectList(db.TBL_PRODUCTOS, "ID_PRODUCTO", "DESCRIPCION", tBL_STOCK.ID_PRODUC);
            return View(tBL_STOCK);
        }

        // POST: Stock/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_STOCK,STOCK,PEDIDO,COMPROMETIDO,ID_PRODUCT,ID_ALMAC,ULTIMA_FECHA_ACTUALIZACION")] TBL_STOCK tBL_STOCK)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tBL_STOCK).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_ALMAC = new SelectList(db.TBL_ALMACENES, "ID_ALMACEN", "NOM_ALMACEN", tBL_STOCK.ID_ALMAC);
            ViewBag.ID_PRODUC = new SelectList(db.TBL_PRODUCTOS, "ID_PRODUCTO", "DESCRIPCION", tBL_STOCK.ID_PRODUC);
            return View(tBL_STOCK);
        }

        // GET: Stock/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_STOCK tBL_STOCK = db.TBL_STOCK.Find(id);
            if (tBL_STOCK == null)
            {
                return HttpNotFound();
            }
            return View(tBL_STOCK);
        }

        // POST: Stock/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TBL_STOCK tBL_STOCK = db.TBL_STOCK.Find(id);
            db.TBL_STOCK.Remove(tBL_STOCK);
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

        

        public JsonResult BuscarProducto(string term)
        {
            using (ALMENDRITAEntities db = new ALMENDRITAEntities())
            {
                var resultado = db.TBL_PRODUCTOS.Where(x => x.DESCRIPCION.Contains(term)).Where(x => x.ESTATUS == true).Select(x => x.DESCRIPCION).Take(10).ToList();
                /*var resultado = from producto in db.TBL_PRODUCTOS
                                where producto.DESCRIPCION.Contains(term)
                                select producto.DESCRIPCION;*/

                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult redirectInventario()
        {
            return RedirectToAction("index","Inventarios");
        }

    }
}
