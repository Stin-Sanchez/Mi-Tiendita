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
        private readonly IUserService _userService;
       

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

        [HttpPost]
        public JsonResult GuardarUsuario(USUARIOS usuario)
        {
            // 1. Inicializamos las variables de respuesta
            object resultado=null;
            string mensajeRespuesta = string.Empty;

            try
            {

                if (usuario.ID_USUARIO == 0)
                {

                    resultado = _userService.Insertar(usuario);
                    mensajeRespuesta = "Usuario creado correctamente.";
                }
                else
                {
                    // 1. Buscamos el usuario tal como está en la base de datos (con su CLAVE y FECHA intactas)
                   
                    var usuarioOriginal = _userService.ObtenerPorId(usuario.ID_USUARIO);

                    if (usuarioOriginal != null)
                    {
                        // 2. Sobrescribimos SOLO las propiedades que el usuario editó en el modal
                        usuarioOriginal.NOMBRE = usuario.NOMBRE;
                        usuarioOriginal.APELLIDO = usuario.APELLIDO;
                        usuarioOriginal.CORREO = usuario.CORREO;
                        usuarioOriginal.ACTIVO = usuario.ACTIVO;

                        // 3. Mandamos a actualizar el objeto original. 
                        // Como nunca tocamos usuarioOriginal.CLAVE, mantiene su valor encriptado y pasa la validación.
                        resultado = _userService.Actualizar(usuarioOriginal);
                        mensajeRespuesta = "Usuario actualizado correctamente.";
                    }
                    else
                    {
                        mensajeRespuesta = "Error: No se encontró el usuario a actualizar.";
                    }
                }
            }
            catch (Exception ex)
            {
                // Si algo explota en la base de datos, caemos aquí
                mensajeRespuesta = "Error al guardar el usuario: " + ex.Message;
            }

            //Retornamos el objeto anónimo. 

            return Json(new { resultado = resultado, mensaje = mensajeRespuesta });
        }

        [HttpPost]
        public JsonResult EliminarUsuario(int id)
        {
            bool respuesta = false;
            string mensajeRespuesta = string.Empty;

            try
            {
                // Llamamos al metodo
                _userService.Eliminar(id);

                // Si la línea anterior no explota, significa que se eliminó/desactivó con éxito
                respuesta = true;
                mensajeRespuesta = "Usuario desactivado correctamente.";
            }
            catch (Exception ex)
            {
                // Si hay algún problema en la base de datos, lo atrapamos aquí
                respuesta = false;
                mensajeRespuesta = "Error al desactivar el usuario: " + ex.Message;
            }

            // Devolvemos el JSON
            return Json(new { resultado = respuesta, mensaje = mensajeRespuesta });
        }





    }
}