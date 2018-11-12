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
    public class SucursalesController : Controller
    {
        private ALMENDRITAEntities db = new ALMENDRITAEntities();

        // GET: Sucursales
        public ActionResult Index()
        {
            return View(db.TBL_SUCURSALES.ToList());
        }

        // GET: Sucursales/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_SUCURSALES tBL_SUCURSALES = db.TBL_SUCURSALES.Find(id);
            if (tBL_SUCURSALES == null)
            {
                return HttpNotFound();
            }
            return View(tBL_SUCURSALES);
        }

        // GET: Sucursales/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sucursales/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NOM_SUCURSAL,PAIS,ESTADO,CALLE,NUMERO,COLONIA,CP,TELEFONO")] TBL_SUCURSALES tBL_SUCURSALES)
        {
            tBL_SUCURSALES.ESTATUS = true;
            tBL_SUCURSALES.FECHA_REGISTRO_SUCURSAL = DateTime.Now;
            tBL_SUCURSALES.ID_USER_ASP = HttpContext.User.Identity.Name;
            if (ModelState.IsValid)
            {
                db.TBL_SUCURSALES.Add(tBL_SUCURSALES);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tBL_SUCURSALES);
        }

        // GET: Sucursales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_SUCURSALES tBL_SUCURSALES = db.TBL_SUCURSALES.Find(id);
            if (tBL_SUCURSALES == null)
            {
                return HttpNotFound();
            }
            return View(tBL_SUCURSALES);
        }

        // POST: Sucursales/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_SUCURSAL,FECHA_REGISTRO_SUCURSAL,ID_USER_ASP,NOM_SUCURSAL,PAIS,ESTADO,CALLE,NUMERO,COLONIA,CP,ESTATUS,TELEFONO")] TBL_SUCURSALES tBL_SUCURSALES)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tBL_SUCURSALES).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tBL_SUCURSALES);
        }

        // GET: Sucursales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_SUCURSALES tBL_SUCURSALES = db.TBL_SUCURSALES.Find(id);
            if (tBL_SUCURSALES == null)
            {
                return HttpNotFound();
            }
            return View(tBL_SUCURSALES);
        }

        // POST: Sucursales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TBL_SUCURSALES tBL_SUCURSALES = db.TBL_SUCURSALES.Find(id);
            db.TBL_SUCURSALES.Remove(tBL_SUCURSALES);
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
