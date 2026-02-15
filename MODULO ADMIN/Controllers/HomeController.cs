using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ENTIDADES;
using DAL;
using DAL.Servicios;

namespace MODULO_ADMIN.Controllers
{
    public class HomeController : Controller
    {
        // 1. Declaras tu servicio a nivel de clase, de solo lectura
        private readonly UserServiceImp _userService;


        public HomeController(UserServiceImp userService)
        {
            _userService = userService;
        }

      

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Usuarios()
        {
            return View();
        }

     
        [HttpGet]
        public JsonResult ListarUsuarios()
        {
            // Llamamos a la capa de negocio
            List<USUARIOS> listaUsuarios = _userService.ObtenerTodos().ToList();

            // Retornamos el JSON. (En MVC 5, JsonRequestBehavior.AllowGet es obligatorio para peticiones GET)
            return Json(new { data = listaUsuarios }, JsonRequestBehavior.AllowGet);
        }




    }
}