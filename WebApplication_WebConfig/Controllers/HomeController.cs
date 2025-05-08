using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication_WebConfig.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var conexion = ConfigurationManager.ConnectionStrings["MyConnectionString"];
            ViewData["con"] = conexion.ConnectionString;
            ViewData["Example"] = ConfigurationManager.AppSettings["Example"];

            string environment = ConfigurationManager.AppSettings["Environment"];
            if (environment == "Debug")
            {
                // Configuración para debug
            }
            else if (environment == "Release")
            {
                // Configuración para release
            }

            ViewData["environment"] = environment;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}