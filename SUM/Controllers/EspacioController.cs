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
    public class EspacioController : Controller
    {
        private Sum1Entities db = new Sum1Entities();

        private Usuario GetUsuario()
        {
            return ((Usuario)Session["Usuario"]);
        }

        // GET: Espacio
        public ActionResult Index()
        {
            
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");
            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");

            var usuario = GetUsuario();

            var espacio = db.Espacio.Include(e => e.Consorcio).Where(x=>x.cd_consorcio == usuario.cd_consorcio);
            return View(espacio.ToList());
        }

        // GET: Espacio/Details/5
        public ActionResult Details(int? id)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Espacio espacio = db.Espacio.Find(id);
            if (espacio == null)
            {
                return HttpNotFound();
            }
            return View(espacio);
        }

        // GET: Espacio/Create
        public ActionResult Create()
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: Espacio/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cd_consorcio,cd_espacio,tx_descripcion,fl_costo_semana,fl_costo_fin_de_semana,fl_limpieza,fl_multa")] Espacio espacio)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
            var usuario = GetUsuario();

            espacio.cd_consorcio = usuario.cd_consorcio;
            if (ModelState.IsValid)
            {
                db.Espacio.Add(espacio);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            
            return View(espacio);
        }

        // GET: Espacio/Edit/5
        public ActionResult Edit(int? id)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Espacio espacio = db.Espacio.Find(id);
            if (espacio == null)
            {
                return HttpNotFound();
            }
            
            return View(espacio);
        }

        // POST: Espacio/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cd_consorcio,cd_espacio,tx_descripcion,fl_costo_semana,fl_costo_fin_de_semana,fl_limpieza,fl_multa")] Espacio espacio)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                db.Entry(espacio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(espacio);
        }

        // GET: Espacio/Delete/5
        public ActionResult Delete(int? id)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Espacio espacio = db.Espacio.Find(id);
            if (espacio == null)
            {
                return HttpNotFound();
            }
            return View(espacio);
        }

        // POST: Espacio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
            Espacio espacio = db.Espacio.Find(id);
            db.Espacio.Remove(espacio);
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
