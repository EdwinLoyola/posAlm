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
    public class AlmacenesController : Controller
    {
        private ALMENDRITAEntities db = new ALMENDRITAEntities();

        // GET: Almacenes
        public ActionResult Index()
        {
            return View(db.TBL_ALMACENES.ToList());
        }

        // GET: Almacenes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_ALMACENES tBL_ALMACENES = db.TBL_ALMACENES.Find(id);
            if (tBL_ALMACENES == null)
            {
                return HttpNotFound();
            }
            return View(tBL_ALMACENES);
        }

        // GET: Almacenes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Almacenes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NOM_ALMACEN,ESTADO,CALLE,NUMERO,COLONIA,CP")] TBL_ALMACENES tBL_ALMACENES)
        {

            tBL_ALMACENES.ESTATUS = true;
            tBL_ALMACENES.FECHA_REGISTRO_ALMACEN = DateTime.Now;
            tBL_ALMACENES.ID_USER_ASP = HttpContext.User.Identity.Name;
            if (ModelState.IsValid)
            {
                db.TBL_ALMACENES.Add(tBL_ALMACENES);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tBL_ALMACENES);
        }

        // GET: Almacenes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_ALMACENES tBL_ALMACENES = db.TBL_ALMACENES.Find(id);
            if (tBL_ALMACENES == null)
            {
                return HttpNotFound();
            }
            return View(tBL_ALMACENES);
        }

        // POST: Almacenes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_ALMACEN,FECHA_REGISTRO_ALMACEN,ID_USER_ASP,NOM_ALMACEN,ESTADO,CALLE,NUMERO,COLONIA,CP,ESTATUS")] TBL_ALMACENES tBL_ALMACENES)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tBL_ALMACENES).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tBL_ALMACENES);
        }

        // GET: Almacenes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_ALMACENES tBL_ALMACENES = db.TBL_ALMACENES.Find(id);
            if (tBL_ALMACENES == null)
            {
                return HttpNotFound();
            }
            return View(tBL_ALMACENES);
        }

        // POST: Almacenes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TBL_ALMACENES tBL_ALMACENES = db.TBL_ALMACENES.Find(id);
            db.TBL_ALMACENES.Remove(tBL_ALMACENES);
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
