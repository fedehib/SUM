using SUM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SUM.Controllers
{
    public class HomeController : Controller
    {
        private Sum1Entities db = new Sum1Entities();
        private Usuario GetUsuario()
        {
            return ((Usuario)Session["Usuario"]);
        }

        public ActionResult Index()
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        public ActionResult GastosMensuales()
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");

            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");

            ViewBag.fecha = null;
            return View("GastosMensuales", db.sp_GastosMensuales(null, 0).ToList());
        }
        [HttpPost]
        public ActionResult GastosMensuales(DateTime mesano)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");
            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
            var gastosMensuales = db.sp_GastosMensuales(mesano, GetUsuario().cd_consorcio).OrderBy(x => x.cd_usuario).ToList();
            ViewBag.fecha = mesano;
            return View("GastosMensuales", gastosMensuales);
            
        }

        // GET: Reserva/Details/5
        public ActionResult Details(DateTime mesano, string id)
        {
            if (GetUsuario() == null)
                return RedirectToAction("Login", "Account");
            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");


            ViewBag.id = id;
            ViewBag.fecha = mesano;
            var gastosMensuales = db.sp_GastosMensualesDetalle(mesano, GetUsuario().cd_consorcio, id).OrderBy(x=>x.fc_fecha).ToList();
            return View("Details", gastosMensuales);
        }

        public ActionResult DescargarGastosMensuales(DateTime mesano)
        {
            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
            var gv = new System.Web.UI.WebControls.GridView();
            var gastosMensuales = db.sp_GastosMensuales(mesano, GetUsuario().cd_consorcio).OrderBy(x => x.cd_usuario).ToList();
            gv.DataSource = gastosMensuales;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=GastosMensuales" + mesano.ToString("MM-yyyy") + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            System.IO.StringWriter objStringWriter = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter objHtmlTextWriter = new System.Web.UI.HtmlTextWriter(objStringWriter);
            Response.Output.WriteLine("<b>Gastos Mensuales del consorcio "+ GetUsuario().Consorcio.tx_descripcion + "</b>");
            Response.Output.WriteLine("<br>");
            Response.Output.WriteLine("<u>Periodo " + mesano.ToString("MM-yyyy") + "</u>");
            Response.Output.WriteLine("<br>");
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.WriteLine(objStringWriter.ToString());
            Response.Output.WriteLine("<br>");
            Response.Flush();
            Response.End();
            return Redirect("GastosMensuales");
            
        }

        public ActionResult DescargarGastosMensualesDetalle(DateTime mesano, string id)
        {
            if (!((SUM.Models.Usuario)Session["Usuario"]).fl_administrador)
                return RedirectToAction("Index", "Home");
            var gv = new System.Web.UI.WebControls.GridView();
            var gastosMensuales = db.sp_GastosMensualesDetalle(mesano, GetUsuario().cd_consorcio, id).OrderBy(x => x.fc_fecha).ToList();
            gv.DataSource = gastosMensuales;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=GastosMensualesDetalle" + mesano.ToString("MM-yyyy") + id + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            System.IO.StringWriter objStringWriter = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter objHtmlTextWriter = new System.Web.UI.HtmlTextWriter(objStringWriter);
            Response.Output.WriteLine("<b>Gastos Mensuales del consorcio " + GetUsuario().Consorcio.tx_descripcion + "</b>");
            Response.Output.WriteLine("<br>");
            Response.Output.WriteLine("<u>Periodo " + mesano.ToString("MM-yyyy") + "</u>");
            Response.Output.WriteLine("<br>");
            var usuario = GetUsuario();
            Response.Output.WriteLine("<u>Departamento " + id + "</u>");
            Response.Output.WriteLine("<br>");
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.WriteLine(objStringWriter.ToString());
            Response.Output.WriteLine("<br>");
            Response.Flush();
            Response.End();
            return Redirect("GastosMensuales");
        }


    }
}