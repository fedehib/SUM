using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUM.Models;
using System.Net.Mail;


namespace SUM.Controllers
{
    public class ReservaController : Controller
    {
        public class _calendario
        {
            public string id { get; set; }
            public int resourceId { get; set; }
            public DateTime start { get; set; }
            public DateTime end { get; set; }
            public string title { get; set; }


        }
        private Sum1Entities db = new Sum1Entities();
        public JsonResult ListaReservas()
        {

            List<_calendario> lista = new List<_calendario>();
            var usuario = GetUsuario();
            DateTime fecha = DateTime.Now.Date.AddDays(int.Parse(ConfigurationManager.AppSettings["DiasPrevios"].ToString()));
            var reserva = db.Reserva.Include(r => r.Espacio).Include(r => r.Usuario).Where(x => x.cd_consorcio == usuario.cd_consorcio && x.fc_fecha >= fecha);

            foreach (var item in reserva)
            {
                string limpieza = (item.fl_contrata_limpieza ? "Con Limpieza" : "Sin Limpieza");
                _calendario dato = new _calendario();
                dato.start = item.fc_fecha;
                dato.end = item.fc_fecha;
                dato.title = item.Espacio.tx_descripcion + " (" + item.cd_usuario + ") " + limpieza;
                lista.Add(dato);
            }

            return Json(lista, JsonRequestBehavior.AllowGet);

        }

        
        // GET: Reserva
        public ActionResult Index()
        {
            if (GetUsuario()==null)
                return RedirectToAction("Login", "Account");

            var usuario = GetUsuario();

            ViewBag.Resultado = "";
           
            if (!usuario.fl_administrador)
            {
                var reserva = db.Reserva.Include(r => r.Espacio).Include(r => r.Usuario).Where(x => x.cd_usuario == usuario.cd_usuario && x.cd_consorcio == usuario.cd_consorcio).OrderBy(x=>x.fc_fecha);
                return View(reserva.ToList());
            }
            else
            {
                var reserva = db.Reserva.Include(r => r.Espacio).Include(r => r.Usuario).Where(x => x.cd_consorcio == usuario.cd_consorcio).OrderBy(x => x.fc_fecha);
                return View(reserva.ToList());
            }
        }

        // GET: Reserva/Details/5
        public ActionResult Details(int espacio, string usuario,DateTime fecha)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador && ((SUM.Models.Usuario)Session["Usuario"]).cd_usuario != usuario)
                return RedirectToAction("Index", "Home");


            if (espacio == 0 || string.IsNullOrEmpty(usuario) || fecha == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reserva reserva = db.Reserva.Find(GetUsuario().cd_consorcio, espacio,usuario,fecha);
            if (reserva == null)
            {
                return HttpNotFound();
            }
            return View(reserva);
        }

        public void enviarMail(string asunto, string cuerpo, string cd_usuario, Usuario usr) {
            try
            {
                var usuario = db.Usuario.Where(x => x.cd_usuario == cd_usuario && x.cd_consorcio == usr.cd_consorcio).FirstOrDefault();
                if (!string.IsNullOrEmpty(usuario.tx_mail))
                {
                    var fromAddress = new MailAddress(ConfigurationManager.AppSettings["mail"].ToString(), "Sistema de Reservas");
                    var toAddress = new MailAddress(usuario.tx_mail, "");
                    string fromPassword = ConfigurationManager.AppSettings["password"].ToString();

                    var smtp = new SmtpClient
                    {
                        Host = ConfigurationManager.AppSettings["host"].ToString(),
                        Port = int.Parse(ConfigurationManager.AppSettings["port"].ToString()),
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                        Timeout = 20000
                    };
                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = asunto,
                        Body = cuerpo
                    })
                    {
                        smtp.Send(message);
                    }
                }
            }
            catch (Exception ex)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        private string renderContacto(Usuario usuario) {
            string contactos = "";
            foreach (var item in db.Contacto.Where(x => x.cd_consorcio == usuario.cd_consorcio))
            {
                contactos += "<li>" + item.tx_nombre + " (" + item.tx_rol + ") - " + item.tx_telefono + "</li>";
            }
            return contactos;
        }
        private string renderValores(Usuario usuario)
        {
            string valores = "";
            foreach (var item in db.Espacio.Where(x => x.cd_consorcio == usuario.cd_consorcio))
            {
                valores += "<li>" + item.tx_descripcion + " Costo Semana: "+ item.fl_costo_semana + " Costo Fin de semana: " + item.fl_costo_fin_de_semana + " Costo Limpieza: " + item.fl_limpieza+" Costo Multa: " + item.fl_multa+ "</li>";
            }
            return valores;
        }
        private int renderTiempo(Usuario usuario)
        {
            int tiempo = 0;
            var parametros = db.Parametros.Where(x => x.cd_consorcio == usuario.cd_consorcio).FirstOrDefault();
            if (parametros != null)
                tiempo = parametros.cd_dias_previos_cancelacion;
            return tiempo;
        }
        // GET: Reserva/Create
        public ActionResult Create()
        {
           

            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            var usuario = GetUsuario();

            ViewBag.cd_usuario = new SelectList(db.Usuario.Where(x => x.cd_consorcio == usuario.cd_consorcio && x.fl_inhabilita_reserva == false), "cd_usuario", "cd_usuario");
            ViewBag.cd_espacio = new SelectList(db.Espacio.Where(x => x.cd_consorcio == usuario.cd_consorcio), "cd_espacio", "tx_descripcion");

            ViewBag.Contactos = renderContacto(usuario);
            ViewBag.TiempoCancelacion = renderTiempo(usuario);
            ViewBag.Valores = renderValores(usuario);
            ViewBag.Resultado = "";
            return View();
        }

        // POST: Reserva/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cd_consorcio,cd_espacio,cd_usuario,fc_fecha,fl_contrata_limpieza,fl_multa,tx_observacion")] Reserva reserva)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");
            
            var usuario = GetUsuario();

            reserva.cd_consorcio = usuario.cd_consorcio;
            //el administrador podra ver el combo y realizar reservas a nombre de otros usuarios, pero el propio usuario se carga a si mismo y no se ve x pantalla eso.
            if (string.IsNullOrEmpty(reserva.cd_usuario))
                reserva.cd_usuario = usuario.cd_usuario;


            if (DateTime.Now.Date <= reserva.fc_fecha || usuario.fl_administrador)
            {

                if (ModelState.IsValid)
                {
                    /*solo para Franklin 2427
                     cd_espacio	tx_descripcion
                        1	SUM Dia
                        2	SUM Noche
                        3	SUM Hora
                        4	SUM Todo el dia
                        6	Parrilla Dia
                        7	Parrilla Noche
                     */
                    DateTime navidad = new DateTime(DateTime.Now.Year, 12, 24);
                    DateTime añonuevo = new DateTime(DateTime.Now.Year, 12, 31);
                    DateTime navidad1 = new DateTime(DateTime.Now.Year, 12, 25);
                    DateTime añonuevo1 = new DateTime(DateTime.Now.Year+1, 1, 1);
                    if ((reserva.fc_fecha != navidad && reserva.fc_fecha != añonuevo && reserva.fc_fecha != añonuevo1 && reserva.fc_fecha != navidad1))
                    {
                        if (db.Reserva.Where(x => x.cd_consorcio == 1 && x.fc_fecha == reserva.fc_fecha && x.cd_espacio == reserva.cd_espacio).Count() > 0)
                        {
                            var res = db.Reserva.Where(x => x.cd_consorcio == 1 && x.fc_fecha == reserva.fc_fecha && x.cd_espacio == reserva.cd_espacio).FirstOrDefault();
                            ViewBag.Resultado = "Ya existe una reserva para ese espacio en esa fecha. La misma esta reserva por " + res.cd_usuario + " (" + res.Usuario.tx_telefono + ")";
                            ViewBag.cd_usuario = new SelectList(db.Usuario.Where(x => x.cd_consorcio == usuario.cd_consorcio && x.fl_inhabilita_reserva == false), "cd_usuario", "cd_usuario");
                            ViewBag.cd_espacio = new SelectList(db.Espacio.Where(x => x.cd_consorcio == usuario.cd_consorcio), "cd_espacio", "tx_descripcion", reserva.cd_consorcio);
                            return View(reserva);
                        }
                        if (db.Reserva.Where(x => x.cd_consorcio == 1 && x.fc_fecha == reserva.fc_fecha && x.cd_espacio == 4).Count() > 0)
                        {
                            var res = db.Reserva.Where(x => x.cd_consorcio == 1 && x.fc_fecha == reserva.fc_fecha && x.cd_espacio == 4).FirstOrDefault();
                            ViewBag.Resultado = "Ya existe una reserva para ese espacio en esa fecha. La misma esta reserva por " + res.cd_usuario + " (" + res.Usuario.tx_telefono + ")";
                            ViewBag.cd_usuario = new SelectList(db.Usuario.Where(x => x.cd_consorcio == usuario.cd_consorcio && x.fl_inhabilita_reserva == false), "cd_usuario", "cd_usuario");
                            ViewBag.cd_espacio = new SelectList(db.Espacio.Where(x => x.cd_consorcio == usuario.cd_consorcio), "cd_espacio", "tx_descripcion", reserva.cd_consorcio);
                            return View(reserva);
                        }
                        if (db.Reserva.Where(x => x.cd_consorcio == 1 && x.fc_fecha == reserva.fc_fecha && x.cd_espacio == 1).Count() > 0 && (reserva.cd_espacio==1 || reserva.cd_espacio==6))
                        {
                            var res = db.Reserva.Where(x => x.cd_consorcio == 1 && x.fc_fecha == reserva.fc_fecha && x.cd_espacio == 1).FirstOrDefault();
                            ViewBag.Resultado = "Ya existe una reserva para ese espacio en esa fecha. La misma esta reserva por " + res.cd_usuario + " (" + res.Usuario.tx_telefono + ")";
                            ViewBag.cd_usuario = new SelectList(db.Usuario.Where(x => x.cd_consorcio == usuario.cd_consorcio && x.fl_inhabilita_reserva == false), "cd_usuario", "cd_usuario");
                            ViewBag.cd_espacio = new SelectList(db.Espacio.Where(x => x.cd_consorcio == usuario.cd_consorcio), "cd_espacio", "tx_descripcion", reserva.cd_consorcio);
                            return View(reserva);
                        }
                        if (db.Reserva.Where(x => x.cd_consorcio == 1 && x.fc_fecha == reserva.fc_fecha && x.cd_espacio == 2).Count() > 0 && (reserva.cd_espacio == 2 || reserva.cd_espacio == 7))
                        {
                            var res = db.Reserva.Where(x => x.cd_consorcio == 1 && x.fc_fecha == reserva.fc_fecha && x.cd_espacio == 2).FirstOrDefault();
                            ViewBag.Resultado = "Ya existe una reserva para ese espacio en esa fecha. La misma esta reserva por " + res.cd_usuario + " (" + res.Usuario.tx_telefono + ")";
                            ViewBag.cd_usuario = new SelectList(db.Usuario.Where(x => x.cd_consorcio == usuario.cd_consorcio && x.fl_inhabilita_reserva == false), "cd_usuario", "cd_usuario");
                            ViewBag.cd_espacio = new SelectList(db.Espacio.Where(x => x.cd_consorcio == usuario.cd_consorcio), "cd_espacio", "tx_descripcion", reserva.cd_consorcio);
                            return View(reserva);
                        }
                        if (db.Reserva.Where(x => x.cd_consorcio == 1 && x.fc_fecha == reserva.fc_fecha && (x.cd_espacio == 1 || x.cd_espacio == 2 || x.cd_espacio == 3)).Count() > 0 && (reserva.cd_espacio == 4))
                        {
                            var res = db.Reserva.Where(x => x.cd_consorcio == 1 && x.fc_fecha == reserva.fc_fecha && x.cd_espacio == 1).FirstOrDefault();
                            if (res==null)
                                res = db.Reserva.Where(x => x.cd_consorcio == 1 && x.fc_fecha == reserva.fc_fecha && x.cd_espacio == 2).FirstOrDefault();
                            if (res==null)
                                res = db.Reserva.Where(x => x.cd_consorcio == 1 && x.fc_fecha == reserva.fc_fecha && x.cd_espacio == 3).FirstOrDefault();
                            ViewBag.Resultado = "Ya existe una reserva para ese espacio en esa fecha. La misma esta reserva por " + res.cd_usuario + " (" + res.Usuario.tx_telefono + ")";
                            ViewBag.cd_usuario = new SelectList(db.Usuario.Where(x => x.cd_consorcio == usuario.cd_consorcio && x.fl_inhabilita_reserva == false), "cd_usuario", "cd_usuario");
                            ViewBag.cd_espacio = new SelectList(db.Espacio.Where(x => x.cd_consorcio == usuario.cd_consorcio), "cd_espacio", "tx_descripcion", reserva.cd_consorcio);
                            return View(reserva);
                        }
                        
                        //generico cualquier consorcio
                        if (db.Reserva.Where(x => x.cd_consorcio == reserva.cd_consorcio && x.fc_fecha == reserva.fc_fecha && x.cd_espacio == reserva.cd_espacio).Count() > 0)
                        {
                            var res = db.Reserva.Where(x => x.cd_consorcio == reserva.cd_consorcio && x.fc_fecha == reserva.fc_fecha).FirstOrDefault();
                            ViewBag.Resultado = "Ya existe una reserva para ese espacio en esa fecha. La misma esta reserva por " + res.cd_usuario + " (" + res.Usuario.tx_telefono + ")";
                            ViewBag.cd_usuario = new SelectList(db.Usuario.Where(x => x.cd_consorcio == usuario.cd_consorcio && x.fl_inhabilita_reserva == false), "cd_usuario", "cd_usuario");
                            ViewBag.cd_espacio = new SelectList(db.Espacio.Where(x => x.cd_consorcio == usuario.cd_consorcio), "cd_espacio", "tx_descripcion", reserva.cd_consorcio);
                            return View(reserva);
                        }
                    }


                    db.Reserva.Add(reserva);
                    try
                    {
                        db.SaveChanges();
                        string cuerpo = "Se confirmó su reserva en " + db.Espacio.Where(x=>x.cd_consorcio==usuario.cd_consorcio && x.cd_espacio==reserva.cd_espacio).FirstOrDefault().tx_descripcion + " para el día " + reserva.fc_fecha.Date.ToString("dd/MM/yyyy") + " para el departamento " + reserva.cd_usuario + ". Datos de la reserva: Limpieza: " + (reserva.fl_contrata_limpieza ? "Si" : "No" ) + ". Multa: "+  (reserva.fl_multa? "Si" : "No") + ". Observación: " + reserva.tx_observacion;
                        enviarMail("Reserva confirmada", cuerpo, reserva.cd_usuario,usuario);
                        
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Resultado = "Ya existe una reserva realizada en esa fecha para ese usuario en ese espacio. Contactese con el administrador si continua el inconveniente.";
                            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                    
                    
                }
            }
            else {
                ViewBag.Resultado = "No se puede hacer una reserva previa al dia de la fecha. Contactese con el administrador si continua el inconveniente.";
            }


            
            ViewBag.cd_usuario = new SelectList(db.Usuario.Where(x => x.cd_consorcio == usuario.cd_consorcio && x.fl_inhabilita_reserva == false), "cd_usuario", "cd_usuario");
            ViewBag.cd_espacio = new SelectList(db.Espacio.Where(x => x.cd_consorcio == usuario.cd_consorcio), "cd_espacio", "tx_descripcion", reserva.cd_consorcio);
            ViewBag.Contactos = renderContacto(usuario);
            ViewBag.TiempoCancelacion = renderTiempo(usuario);
            ViewBag.Valores = renderValores(usuario);
            return View(reserva);
        }

        // GET: Reserva/Edit/5
        public ActionResult Edit(int espacio, string usuario, DateTime fecha)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");
            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador && ((SUM.Models.Usuario)Session["Usuario"]).cd_usuario != usuario)
                return RedirectToAction("Index", "Home");

            if (espacio == 0 || string.IsNullOrEmpty(usuario) || fecha == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reserva reserva = db.Reserva.Find(GetUsuario().cd_consorcio, espacio, usuario, fecha);
            if (reserva == null)
            {
                return HttpNotFound();
            }

            var usuario1 = GetUsuario();

            ViewBag.cd_usuario = new SelectList(db.Usuario.Where(x => x.cd_consorcio == usuario1.cd_consorcio), "cd_usuario", "cd_usuario", reserva.cd_usuario);
            ViewBag.cd_espacio = new SelectList(db.Espacio.Where(x => x.cd_consorcio == usuario1.cd_consorcio), "cd_espacio", "tx_descripcion", reserva.cd_consorcio);
            return View(reserva);
        }

        // POST: Reserva/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cd_consorcio,cd_espacio,cd_usuario,fc_fecha,fl_contrata_limpieza,fl_multa,tx_observacion")] Reserva reserva)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");
            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador && ((SUM.Models.Usuario)Session["Usuario"]).cd_usuario != reserva.cd_usuario)
                return RedirectToAction("Index", "Home");
            var usuario = GetUsuario();
            if (ModelState.IsValid)
            {
                db.Entry(reserva).State = EntityState.Modified;
                db.SaveChanges();
                string cuerpo = "Se actualizó su reserva en " + db.Espacio.Where(x => x.cd_consorcio == usuario.cd_consorcio && x.cd_espacio == reserva.cd_espacio).FirstOrDefault().tx_descripcion + " para el día " + reserva.fc_fecha.Date.ToString("dd/MM/yyyy") + " para el departamento " + reserva.cd_usuario + ". Datos de la reserva: Limpieza: " + (reserva.fl_contrata_limpieza ? "Si" : "No") + ". Multa: " + (reserva.fl_multa ? "Si" : "No") + ". Observación: " + reserva.tx_observacion;
                enviarMail("Reserva actualizada", cuerpo, reserva.cd_usuario,usuario);
                return RedirectToAction("Index");
            }
           
            ViewBag.cd_usuario = new SelectList(db.Usuario.Where(x => x.cd_consorcio == usuario.cd_consorcio && x.fl_inhabilita_reserva == false), "cd_usuario", "cd_usuario");
            ViewBag.cd_espacio = new SelectList(db.Espacio.Where(x => x.cd_consorcio == usuario.cd_consorcio), "cd_espacio", "tx_descripcion", reserva.cd_consorcio);
            return View(reserva);
        }

        // GET: Reserva/Delete/5
        public ActionResult Delete(int espacio, string usuario, DateTime fecha)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");
            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador && ((SUM.Models.Usuario)Session["Usuario"]).cd_usuario != usuario)
                return RedirectToAction("Index", "Home");

            if (espacio == 0 || string.IsNullOrEmpty(usuario) || fecha == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reserva reserva = db.Reserva.Find(GetUsuario().cd_consorcio, espacio, usuario, fecha);
            if (reserva == null)
            {
                return HttpNotFound();
            }
            Parametros p = db.Parametros.Find(GetUsuario().cd_consorcio);
            DateTime fechaValida= reserva.fc_fecha.AddDays(-double.Parse(p.cd_dias_previos_cancelacion.ToString())).Date;
            ViewBag.Fecha = fechaValida;
            ViewBag.PuedeEliminarse = (fechaValida>= DateTime.Now.Date);
            return View(reserva);
        }

        // POST: Reserva/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int espacio, string usuario, DateTime fecha)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");
            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador && ((SUM.Models.Usuario)Session["Usuario"]).cd_usuario != usuario)
                return RedirectToAction("Index", "Home");
            var us = GetUsuario();
            Reserva reserva = db.Reserva.Find(GetUsuario().cd_consorcio, espacio, usuario, fecha);
            db.Reserva.Remove(reserva);
            db.SaveChanges();
            string cuerpo = "Se eliminó su reserva en " + db.Espacio.Where(x => x.cd_consorcio == us.cd_consorcio && x.cd_espacio == reserva.cd_espacio).FirstOrDefault().tx_descripcion + " para el día " + reserva.fc_fecha.Date.ToString("dd/MM/yyyy") + " para el departamento " + reserva.cd_usuario;
            enviarMail("Reserva eliminada", cuerpo, reserva.cd_usuario, us);
            return RedirectToAction("Index","Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private Usuario GetUsuario()
        {
            return ((Usuario)Session["Usuario"]);
        }
    }
}
