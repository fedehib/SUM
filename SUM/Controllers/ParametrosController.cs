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
    public class ParametrosController : Controller
    {
        private Sum1Entities db = new Sum1Entities();
        private Usuario GetUsuario()
        {
            return ((Usuario)Session["Usuario"]);
        }

        // GET: Parametros
        public ActionResult Index()
        {
           
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");
            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");

            var usuario = GetUsuario();

            var parametros = db.Parametros.Include(p => p.Consorcio).Where(x => x.cd_consorcio == usuario.cd_consorcio); ;
            return View(parametros.ToList());
        }

        // GET: Parametros/Details/5
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
            Parametros parametros = db.Parametros.Find(id);
            if (parametros == null)
            {
                return HttpNotFound();
            }
            return View(parametros);
        }

        // GET: Parametros/Create
        public ActionResult Create()
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
            return View();
        }

        // POST: Parametros/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cd_consorcio,cd_dias_previos_cancelacion")] Parametros parametros)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
            var usuario = GetUsuario();

            parametros.cd_consorcio = usuario.cd_consorcio;
            if (ModelState.IsValid)
            {
                db.Parametros.Add(parametros);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            
            return View(parametros);
        }

        // GET: Parametros/Edit/5
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
            Parametros parametros = db.Parametros.Find(id);
            if (parametros == null)
            {
                return HttpNotFound();
            }
            
            return View(parametros);
        }

        // POST: Parametros/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cd_consorcio,cd_dias_previos_cancelacion")] Parametros parametros)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                db.Entry(parametros).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(parametros);
        }

        // GET: Parametros/Delete/5
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
            Parametros parametros = db.Parametros.Find(id);
            if (parametros == null)
            {
                return HttpNotFound();
            }
            return View(parametros);
        }

        // POST: Parametros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
            Parametros parametros = db.Parametros.Find(id);
            db.Parametros.Remove(parametros);
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
