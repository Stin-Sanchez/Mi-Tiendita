using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ENTIDADES;
using DAL;
using DAL.Servicios;
using System.Threading.Tasks;
using System.Configuration;
using System.Web.Security;

namespace MODULO_ADMIN.Controllers
{
   
    public class AccesoController : Controller
    {

        private readonly IUserService _userService;

        public AccesoController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: Acceso
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CambiarClave()
        {
            return View();
        }
        public ActionResult RestablecerClave()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(string correo, string clave)
        {
            USUARIOS usuario = new USUARIOS();

            // 1. Encriptamos la clave primero para que quede más limpio
            string claveEncriptada = UtilService.EncriptarClave(clave);

            // 2. Usamos paréntesis para esperar la lista, y aplicamos FirstOrDefault directo
            usuario = (await _userService.ObtenerTodos())
                      .FirstOrDefault(u => u.CORREO == correo && u.CLAVE == claveEncriptada);

            if (usuario == null)
            {
                ViewBag.Error = "Correo o contraseña no correcta";
                return View();
            }
            else
            {
                if ((bool)usuario.RESTABLECER)
                {
                    TempData["idUsuario"] = usuario.ID_USUARIO;
                    return RedirectToAction("CambiarClave");
                }

                FormsAuthentication.SetAuthCookie(usuario.CORREO,false);


                ViewBag.Error = null;
                return RedirectToAction("Index", "Home");
            }


        }

        [HttpPost]
        public async Task<ActionResult> CambiarClave(string idUsuario, string claveActual, string nuevaClave, string confirmarClave)
        {
            USUARIOS usuario = new USUARIOS();

            usuario = (await  _userService.ObtenerTodos()).Where(u => u.ID_USUARIO == int.Parse(idUsuario)).FirstOrDefault();

            if (usuario.CLAVE != UtilService.EncriptarClave(claveActual))
            {
                TempData["idUsuario"] = idUsuario;
                ViewData["vclave"] = "";
                ViewBag.Error = "La contraseña actual no es correcta";

                return View();

            } else if (nuevaClave != confirmarClave)
            {
                TempData["idUsuario"] = idUsuario;
                ViewData["vclave"] = claveActual;
                ViewBag.Error = "Las contraseñas no coinciden";
                return View();
            }
            ViewData["vclave"] = "";

            nuevaClave = UtilService.EncriptarClave(nuevaClave);

            bool respuesta = _userService.cambiarClave(int.Parse(idUsuario), nuevaClave);

            if (respuesta)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["idUsuario"] = idUsuario;
                ViewBag.Error = "Ocurrio un error al cambiar la clave , usuario o contraseña incorrectos";
                return View();
            }



        }


        [HttpPost]
        public async Task <ActionResult> RestablecerClave( string correo)
        {

            //  Leemos la configuración en la capa principal
            string correoEmisor = ConfigurationManager.AppSettings["CorreoSoporte"];
            string claveEmisor = ConfigurationManager.AppSettings["ClaveCorreoSoporte"];


            USUARIOS usuario = new USUARIOS();

            string correoLimpio = correo.Trim();

            if (string.IsNullOrWhiteSpace(correo))
            {
                ViewBag.Error = "Por favor, ingrese un correo válido.";
                return View();
            }


            usuario = (await _userService.ObtenerTodos()).Where(u => u.CORREO == correoLimpio).FirstOrDefault();

       
            if (usuario == null)
            {
            
                ViewBag.Error = "No se encontró un usuario relacionado al correo";

                return View();

            }

            string mensaje = string.Empty;
            bool respuesta = await _userService.restablecerClave(usuario.ID_USUARIO,correo,correoEmisor,claveEmisor);


            if (respuesta)
            {
                ViewBag.Error = null;
                return RedirectToAction("Index","Acceso");
            }
            else
            {
              
                ViewBag.Error = "Ocurrió un  error al restablecer la contraseña";
                return View();
            }

        }

        public ActionResult logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Acceso");
        }





    }
}