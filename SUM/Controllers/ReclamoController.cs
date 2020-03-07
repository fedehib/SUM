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
    public class ReclamoController : Controller
    {
        private Sum1Entities db = new Sum1Entities();

        private Usuario GetUsuario()
        {
            return ((Usuario)Session["Usuario"]);
        }

        // GET: Reclamo
        public ActionResult Index()
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            var usuario = GetUsuario();

            if (usuario.fl_administrador)
            {
                var reclamo = db.Reclamo.Include(r => r.Usuario).Where(x => x.cd_consorcio == usuario.cd_consorcio);
                return View(reclamo.ToList());
            }
            else {
                var reclamo = db.Reclamo.Include(r => r.Usuario).Where(x => x.cd_consorcio == usuario.cd_consorcio && (x.fl_publico==true || (x.cd_usuario== usuario.cd_usuario)));
                return View(reclamo.ToList());
            }
            
        }

        // GET: Reclamo/Details/5
        public ActionResult Details(int? id)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reclamo reclamo = db.Reclamo.Find(GetUsuario().cd_consorcio, id);
            if (reclamo == null)
            {
                return HttpNotFound();
            }
            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador && ((SUM.Models.Usuario)Session["Usuario"]).cd_usuario != reclamo.cd_usuario)
                return RedirectToAction("Index", "Home");
            return View(reclamo);
        }

        // GET: Reclamo/Create
        public ActionResult Create()
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            var usuario = GetUsuario();
            ViewBag.cd_usuario = new SelectList(db.Usuario.Where(x => x.cd_consorcio == usuario.cd_consorcio), "cd_usuario", "cd_usuario");
            return View();
        }

        // POST: Reclamo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cd_reclamo,cd_consorcio,cd_usuario,tx_reclamo,fl_publico")] Reclamo reclamo)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            var usuario = GetUsuario();
            reclamo.cd_consorcio = usuario.cd_consorcio;
            //el administrador podra ver el combo y realizar reservas a nombre de otros usuarios, pero el propio usuario se carga a si mismo y no se ve x pantalla eso.
            if (string.IsNullOrEmpty(reclamo.cd_usuario))
                reclamo.cd_usuario = usuario.cd_usuario;
            if (ModelState.IsValid)
            {
                db.Reclamo.Add(reclamo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.cd_usuario = new SelectList(db.Usuario.Where(x => x.cd_consorcio == usuario.cd_consorcio), "cd_usuario", "cd_usuario", reclamo.cd_usuario);
            return View(reclamo);
        }

        // GET: Reclamo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            var usuario = GetUsuario();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reclamo reclamo = db.Reclamo.Find(usuario.cd_consorcio,id);
            if (reclamo == null)
            {
                return HttpNotFound();
            }
            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador && ((SUM.Models.Usuario)Session["Usuario"]).cd_usuario != reclamo.cd_usuario)
                return RedirectToAction("Index", "Home");


            ViewBag.cd_usuario = new SelectList(db.Usuario.Where(x => x.cd_consorcio == usuario.cd_consorcio), "cd_usuario", "cd_usuario", reclamo.cd_usuario);
            return View(reclamo);
        }

        // POST: Reclamo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cd_reclamo,cd_consorcio,cd_usuario,tx_reclamo,fl_publico")] Reclamo reclamo)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");
            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador && ((SUM.Models.Usuario)Session["Usuario"]).cd_usuario != reclamo.cd_usuario)
                return RedirectToAction("Index", "Home");


            if (ModelState.IsValid)
            {
                db.Entry(reclamo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var usuario = GetUsuario();
            ViewBag.cd_usuario = new SelectList(db.Usuario.Where(x => x.cd_consorcio == usuario.cd_consorcio), "cd_usuario", "cd_usuario", reclamo.cd_usuario);
            return View(reclamo);
        }

        // GET: Reclamo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            var usuario = GetUsuario();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reclamo reclamo = db.Reclamo.Find(usuario.cd_consorcio, id);
            if (reclamo == null)
            {
                return HttpNotFound();
            }
            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador && ((SUM.Models.Usuario)Session["Usuario"]).cd_usuario != reclamo.cd_usuario)
                return RedirectToAction("Index", "Home");

            return View(reclamo);
        }

        // POST: Reclamo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");
           

            var usuario = GetUsuario();
            Reclamo reclamo = db.Reclamo.Find(usuario.cd_consorcio, id);
            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador && ((SUM.Models.Usuario)Session["Usuario"]).cd_usuario != reclamo.cd_usuario)
                return RedirectToAction("Index", "Home");

            db.Reclamo.Remove(reclamo);
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
