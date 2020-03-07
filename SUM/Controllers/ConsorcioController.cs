using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUM.Models;

namespace SUM.Controllers
{
    public class ConsorcioController : Controller
    {
        private Sum1Entities db = new Sum1Entities();

        private Usuario GetUsuario()
        {
            return ((Usuario)Session["Usuario"]);
        }
        // GET: Consorcio
        public ActionResult Index()
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");
            if (GetUsuario().cd_usuario != "ROOT")
                return RedirectToAction("Index", "Home");
            var consorcio = db.Consorcio.Include(c => c.Parametros);
            return View(consorcio.ToList());
        }

        // GET: Consorcio/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consorcio consorcio = db.Consorcio.Find(id);
            if (consorcio == null)
            {
                return HttpNotFound();
            }
            if (GetUsuario().cd_usuario != "ROOT")
                return RedirectToAction("Index", "Home");
            return View(consorcio);
        }

        // GET: Consorcio/Create
        public ActionResult Create()
        {
            if (GetUsuario().cd_usuario != "ROOT")
                return RedirectToAction("Index", "Home");
            ViewBag.cd_consorcio = new SelectList(db.Consorcio, "cd_consorcio", "cd_consorcio");
            return View();
        }

        // POST: Consorcio/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cd_consorcio,tx_descripcion")] Consorcio consorcio)
        {
            if (GetUsuario().cd_usuario != "ROOT")
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                db.Consorcio.Add(consorcio);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.cd_consorcio = new SelectList(db.Consorcio, "cd_consorcio", "cd_consorcio", consorcio.cd_consorcio);
            return View(consorcio);
        }

        // GET: Consorcio/Edit/5
        public ActionResult Edit(int? id)
        {
            if (GetUsuario().cd_usuario != "ROOT")
                return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consorcio consorcio = db.Consorcio.Find(id);
            if (consorcio == null)
            {
                return HttpNotFound();
            }
            ViewBag.cd_consorcio = new SelectList(db.Consorcio, "cd_consorcio", "cd_consorcio", consorcio.cd_consorcio);
            return View(consorcio);
        }

        // POST: Consorcio/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cd_consorcio,tx_descripcion")] Consorcio consorcio)
        {
            if (GetUsuario().cd_usuario != "ROOT")
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                db.Entry(consorcio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.cd_consorcio = new SelectList(db.Consorcio, "cd_consorcio", "cd_consorcio", consorcio.cd_consorcio);
            return View(consorcio);
        }

        // GET: Consorcio/Delete/5
        public ActionResult Delete(int? id)
        {
            if (GetUsuario().cd_usuario != "ROOT")
                return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consorcio consorcio = db.Consorcio.Find(id);
            if (consorcio == null)
            {
                return HttpNotFound();
            }
            return View(consorcio);
        }

        // POST: Consorcio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (GetUsuario().cd_usuario != "ROOT")
                return RedirectToAction("Index", "Home");
            Consorcio consorcio = db.Consorcio.Find(id);
            db.Consorcio.Remove(consorcio);
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
