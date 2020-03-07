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
    public class FeriadosController : Controller
    {
        private Sum1Entities db = new Sum1Entities();

        private Usuario GetUsuario()
        {
            return ((Usuario)Session["Usuario"]);
        }

        // GET: Feriados
        public ActionResult Index()
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
            var usuario = GetUsuario();
            var feriados = db.Feriados.Include(f => f.Consorcio).Where(x => x.cd_consorcio == usuario.cd_consorcio).OrderBy(x=>x.fc_fecha);
            return View(feriados.ToList());
        }

        // GET: Feriados/Details/5
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
            Feriados feriados = db.Feriados.Find(GetUsuario().cd_consorcio,id);
            if (feriados == null)
            {
                return HttpNotFound();
            }
            return View(feriados);
        }

        // GET: Feriados/Create
        public ActionResult Create()
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
            var usuario = GetUsuario();
            ViewBag.cd_consorcio = new SelectList(db.Consorcio.Where(x => x.cd_consorcio == usuario.cd_consorcio), "cd_consorcio", "tx_descripcion");
            return View();
        }

        // POST: Feriados/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cd_consorcio,cd_fecha,fc_fecha,tx_descripcion")] Feriados feriados)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
            var usuario = GetUsuario();
            feriados.cd_consorcio = usuario.cd_consorcio;
            if (ModelState.IsValid)
            {
                db.Feriados.Add(feriados);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.cd_consorcio = new SelectList(db.Consorcio.Where(x => x.cd_consorcio == usuario.cd_consorcio), "cd_consorcio", "tx_descripcion", feriados.cd_consorcio);
            return View(feriados);
        }

        // GET: Feriados/Edit/5
        public ActionResult Edit(int? id)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
            var usuario = GetUsuario();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feriados feriados = db.Feriados.Find(usuario.cd_consorcio,id);
            if (feriados == null)
            {
                return HttpNotFound();
            }
            ViewBag.cd_consorcio = new SelectList(db.Consorcio.Where(x => x.cd_consorcio == usuario.cd_consorcio), "cd_consorcio", "tx_descripcion", feriados.cd_consorcio);
            return View(feriados);
        }

        // POST: Feriados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cd_consorcio,cd_fecha,fc_fecha,tx_descripcion")] Feriados feriados)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
            var usuario = GetUsuario();
            if (ModelState.IsValid)
            {
                db.Entry(feriados).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.cd_consorcio = new SelectList(db.Consorcio.Where(x => x.cd_consorcio == usuario.cd_consorcio), "cd_consorcio", "tx_descripcion", feriados.cd_consorcio);
            return View(feriados);
        }

        // GET: Feriados/Delete/5
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
            var usuario = GetUsuario();
            Feriados feriados = db.Feriados.Find(usuario.cd_consorcio, id);
            if (feriados == null)
            {
                return HttpNotFound();
            }
            return View(feriados);
        }

        // POST: Feriados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");

            var usuario = GetUsuario();
            Feriados feriados = db.Feriados.Find(usuario.cd_consorcio, id);
            db.Feriados.Remove(feriados);
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
