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
    public class SociedadesController : Controller
    {
        private ALMENDRITAEntities db = new ALMENDRITAEntities();

        // GET: Sociedades
        public ActionResult Index()
        {
            return View(db.TBL_SOCIEDADES.ToList());
        }

        // GET: Sociedades/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_SOCIEDADES tBL_SOCIEDADES = db.TBL_SOCIEDADES.Find(id);
            if (tBL_SOCIEDADES == null)
            {
                return HttpNotFound();
            }
            return View(tBL_SOCIEDADES);
        }

        // GET: Sociedades/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sociedades/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NOM_SOCIEDAD,RFC,PAIS,ESTADO,CALLE,NUMERO,COLONIA,CP")] TBL_SOCIEDADES tBL_SOCIEDADES)
        {
            tBL_SOCIEDADES.ESTATUS = true;
            tBL_SOCIEDADES.FECHA_REGISTRO_SOCIEDAD = DateTime.Now;
            tBL_SOCIEDADES.ID_USER_ASP = HttpContext.User.Identity.Name;
            if (ModelState.IsValid)
            {
                db.TBL_SOCIEDADES.Add(tBL_SOCIEDADES);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tBL_SOCIEDADES);
        }

        // GET: Sociedades/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_SOCIEDADES tBL_SOCIEDADES = db.TBL_SOCIEDADES.Find(id);
            if (tBL_SOCIEDADES == null)
            {
                return HttpNotFound();
            }
            return View(tBL_SOCIEDADES);
        }

        // POST: Sociedades/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_SOCIEDAD,FECHA_REGISTRO_SOCIEDAD,ID_USER_ASP,NOM_SOCIEDAD,RFC,PAIS,ESTADO,CALLE,NUMERO,COLONIA,CP,ESTATUS")] TBL_SOCIEDADES tBL_SOCIEDADES)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tBL_SOCIEDADES).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tBL_SOCIEDADES);
        }

        // GET: Sociedades/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_SOCIEDADES tBL_SOCIEDADES = db.TBL_SOCIEDADES.Find(id);
            if (tBL_SOCIEDADES == null)
            {
                return HttpNotFound();
            }
            return View(tBL_SOCIEDADES);
        }

        // POST: Sociedades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TBL_SOCIEDADES tBL_SOCIEDADES = db.TBL_SOCIEDADES.Find(id);
            db.TBL_SOCIEDADES.Remove(tBL_SOCIEDADES);
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
