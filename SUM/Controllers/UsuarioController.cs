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
    public class UsuarioController : Controller
    {
        private Sum1Entities db = new Sum1Entities();


        // GET: Usuario
        public ActionResult Index()
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            var usuario1 = GetUsuario();
            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
            {
                var usuario = db.Usuario.Include(u => u.Consorcio).Where(x => x.cd_consorcio == usuario1.cd_consorcio && x.cd_usuario == usuario1.cd_usuario);
                ViewBag.Administrador = false;
                return View(usuario.ToList());
            }
            else {
                var usuario = db.Usuario.Include(u => u.Consorcio).Where(x => x.cd_consorcio == usuario1.cd_consorcio);
                ViewBag.Administrador = true;
                return View(usuario.ToList());
            }
        }

        // GET: Usuario/Details/5
        public ActionResult Details(string id)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrEmpty(id))
            {
                id = ((SUM.Models.Usuario)Session["Usuario"]).cd_usuario;
            }
            Usuario usuario = db.Usuario.Find(GetUsuario().cd_consorcio, id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador && ((SUM.Models.Usuario)Session["Usuario"]).cd_usuario !=usuario.cd_usuario)
                return RedirectToAction("Index", "Home");
            return View(usuario);
        }

        // GET: Usuario/Create
        public ActionResult Create()
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
            return View();
        }

        // POST: Usuario/Create
        // Para protegerse de ataques de publicación excesiva, habsilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cd_consorcio,cd_usuario,tx_contrasena,tx_mail,tx_telefono,fl_administrador,fl_inhabilitado,fl_inhabilita_reserva")] Usuario usuario)
        {
            usuario.cd_usuario = usuario.cd_usuario.ToUpper();
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
            var usuario1 = GetUsuario();
            usuario.cd_consorcio = usuario1.cd_consorcio;
            if (ModelState.IsValid)
            {
                db.Usuario.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            
            return View(usuario);
        }

        // GET: Usuario/Edit/5
        public ActionResult Edit(string id)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrEmpty(id))
            {
                id = ((SUM.Models.Usuario)Session["Usuario"]).cd_usuario;
            }
            Usuario usuario = db.Usuario.Find(GetUsuario().cd_consorcio, id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador && ((SUM.Models.Usuario)Session["Usuario"]).cd_usuario != usuario.cd_usuario)
                return RedirectToAction("Index", "Home");


            return View(usuario);
        }

        // POST: Usuario/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cd_consorcio,cd_usuario,tx_contrasena,tx_mail,tx_telefono,fl_administrador,fl_inhabilitado,fl_inhabilita_reserva")] Usuario usuario)
        {

            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");
            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador && ((SUM.Models.Usuario)Session["Usuario"]).cd_usuario != usuario.cd_usuario)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(usuario);
        }

        // GET: Usuario/Delete/5
        public ActionResult Delete(string id)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrEmpty(id))
            {
                id = ((SUM.Models.Usuario)Session["Usuario"]).cd_usuario;
            }
            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
           
            Usuario usuario = db.Usuario.Find(GetUsuario().cd_consorcio, id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador && ((SUM.Models.Usuario)Session["Usuario"]).cd_usuario != usuario.cd_usuario)
                return RedirectToAction("Index", "Home");

            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
            Usuario usuario = db.Usuario.Find(GetUsuario().cd_consorcio, id);
            db.Usuario.Remove(usuario);
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

        private Usuario GetUsuario() {
            return ((Usuario)Session["Usuario"]);
        }
    }
}
