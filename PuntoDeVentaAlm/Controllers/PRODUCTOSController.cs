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
    public class PRODUCTOSController : Controller
    {
        private ALMENDRITAEntities db = new ALMENDRITAEntities();

        // GET: PRODUCTOS
        public ActionResult Index()
        {
            return View(db.TBL_PRODUCTOS.Where(x => x.ESTATUS == true).ToList());
        }

        // GET: PRODUCTOS/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_PRODUCTOS tBL_PRODUCTOS = db.TBL_PRODUCTOS.Find(id);
            if (tBL_PRODUCTOS == null)
            {
                return HttpNotFound();
            }
            return View(tBL_PRODUCTOS);
        }

        // GET: PRODUCTOS/Create
        public ActionResult Create()
        {
            if (TempData["Mensaje"] != null)
                ViewBag.Mensaje = TempData["Mensaje"].ToString();
            return View();
        }

        // POST: PRODUCTOS/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_PRODUCTO,DESCRIPCION,PRECIO,IVA,IEPS,U_MEDIDA,CLAVE_SAT,ULTIM_PRECIO_COMPRA,PRODUCTO_PROPIO")] TBL_PRODUCTOS tBL_PRODUCTOS)
        {
            var producto = db.TBL_PRODUCTOS.Where(x => x.ID_PRODUCTO.Equals(tBL_PRODUCTOS.ID_PRODUCTO)).Count();

            if(producto > 0)
            {
                TempData["Mensaje"] = "<div class=\"alert alert-warning\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button><strong>El ID CONTPAQ "+tBL_PRODUCTOS.ID_PRODUCTO+" ya está registrado.</strong> </div>";
                return RedirectToAction("Create");
            }


            tBL_PRODUCTOS.ESTATUS = true;
            tBL_PRODUCTOS.FECHA_ALTA = DateTime.Now;

            if (tBL_PRODUCTOS.IVA==null)
                tBL_PRODUCTOS.IVA = 0;

            if (tBL_PRODUCTOS.IEPS == null)
                tBL_PRODUCTOS.IEPS = 0;
            
            if (tBL_PRODUCTOS.PRODUCTO_PROPIO == null)
                tBL_PRODUCTOS.PRODUCTO_PROPIO = false;

            tBL_PRODUCTOS.ID_USER_ASP = HttpContext.User.Identity.Name;

            try
            {
                if (ModelState.IsValid)
                {
                    db.TBL_PRODUCTOS.Add(tBL_PRODUCTOS);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                throw;
            }
            

            return View(tBL_PRODUCTOS);
        }

        // GET: PRODUCTOS/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_PRODUCTOS tBL_PRODUCTOS = db.TBL_PRODUCTOS.Find(id);
            if (tBL_PRODUCTOS == null)
            {
                return HttpNotFound();
            }

            if (TempData["Mensaje"] != null)
                ViewBag.Mensaje = TempData["Mensaje"].ToString();
            return View(tBL_PRODUCTOS);
        }

        // POST: PRODUCTOS/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_PRODUCTO,DESCRIPCION,PRECIO,IVA,IEPS,U_MEDIDA,CLAVE_SAT,FECHA_ALTA,ESTATUS,ULTIM_PRECIO_COMPRA")] TBL_PRODUCTOS tBL_PRODUCTOS)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tBL_PRODUCTOS).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tBL_PRODUCTOS);
        }

        // GET: PRODUCTOS/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_PRODUCTOS tBL_PRODUCTOS = db.TBL_PRODUCTOS.Find(id);
            if (tBL_PRODUCTOS == null)
            {
                return HttpNotFound();
            }
            return View(tBL_PRODUCTOS);
        }

        // POST: PRODUCTOS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            TBL_PRODUCTOS tBL_PRODUCTOS = db.TBL_PRODUCTOS.Find(id);
            db.TBL_PRODUCTOS.Remove(tBL_PRODUCTOS);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public PartialViewResult listaPresentaciones(string ID_PRODUCTO,string DESCRIPCION)
        {
            //ViewBag.ID_PRODUCTO = new SelectList(db.TBL_PRODUCTOS.Where(x => x.ID_PRODUCTO.Equals(ID_PRODUCTO)), "ID_PRODUCTO", "DESCRIPCION");
            ViewBag.Descripcion = DESCRIPCION;
            return PartialView("_Presentacion",db.TBL_PRESENTACIONES_PRODUCT.Where(x => x.ID_PRODUCTO.Equals(ID_PRODUCTO)).ToList());
        }

        [HttpPost]
        public PartialViewResult nuevaPresentacion(string ID_PRODUCTO)
        {
            ViewBag.ID_PRODUCTO = new SelectList(db.TBL_PRODUCTOS.Where(x => x.ID_PRODUCTO.Equals(ID_PRODUCTO)),"ID_PRODUCTO","DESCRIPCION");
            return PartialView("_nuevaPresentacion");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void registraPresentacion([Bind(Include = "ID_PRODUCTO,DESCRIPCION,UNIDADES")]TBL_PRESENTACIONES_PRODUCT tBL_PRESENTACION)
        {
            try
            {
                var val = db.TBL_PRESENTACIONES_PRODUCT.Where(x => x.DESCRIPCION.Equals(tBL_PRESENTACION.DESCRIPCION)).ToList();
                if(tBL_PRESENTACION.DESCRIPCION != null && tBL_PRESENTACION.UNIDADES != null && val.Count()==0)
                {
                    db.TBL_PRESENTACIONES_PRODUCT.Add(tBL_PRESENTACION);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
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
