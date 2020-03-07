using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SUM.Models;

namespace SUM.Controllers
{
    
    public class AccountController : Controller
    {
        private Sum1Entities db = new Sum1Entities();

        public AccountController()
        {

        }

        public ActionResult Login()
        {
            List<SelectListItem> dept = new List<SelectListItem>();
            var query = from u in db.Consorcio select u;
            if (query.Count() > 0)
            {
                foreach (var v in query)
                {
                    dept.Add(new SelectListItem { Text = v.tx_descripcion, Value = v.cd_consorcio.ToString() });
                }
            }
            ViewBag.Cantidad = db.Consorcio.Count();
            ViewBag.Consorcio = dept;


            ViewBag.Resultado = "";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Contacto/Delete/5
        public ActionResult Login(string cd_usuario, string tx_contrasena, int? cd_consorcio)
        {
            if (db.Consorcio.Count() == 1)
                cd_consorcio = 1;
            cd_usuario = cd_usuario.ToUpper();
            Usuario usr = db.Usuario.Find(cd_consorcio, cd_usuario);

            if (usr != null)
            {
                if (usr.tx_contrasena == tx_contrasena)
                {
                    if (!usr.fl_inhabilitado)
                    {
                        Session["Usuario"] = usr;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Resultado = "Su usuario se encuentra inhabilitado para operar. Contactese con el administrador del corsorcio";
                        
                    }
                }
                else
                {
                    ViewBag.Resultado = "La contraseña ingresada es incorrecta";
                    
                }
            }
            else {
                ViewBag.Resultado = "Usuario inexistente";

            }

            List<SelectListItem> dept = new List<SelectListItem>();
            var query = from u in db.Consorcio select u;
            if (query.Count() > 0)
            {
                foreach (var v in query)
                {
                    dept.Add(new SelectListItem { Text = v.tx_descripcion, Value = v.cd_consorcio.ToString() });
                }
            }
            ViewBag.Consorcio = dept;
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Contacto/Delete/5
        public ActionResult LogOff()
        {
            Session["Usuario"] = null;
            ViewBag.Resultado = "";
            return RedirectToAction("Login", "Account");
        }

        

    }
}