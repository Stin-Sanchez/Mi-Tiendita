using ENTIDADES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MODULO_CLIENTE.Controllers
{
    public class AccesoController : Controller
    {
        // GET: Acceso
        public ActionResult Index()
        {
            return View();
        }
        
        
        public ActionResult Registrar()
        {
            return View();
        } 
        
        
        
        public ActionResult Restablecer()
        {

            return View();
        }
        
        
        public ActionResult CambiarClave()
        {

            return View();
        }


        [HttpPost]
        public ActionResult Registrar(USUARIOS usuario)
        {
            return View();
        }
    }
}