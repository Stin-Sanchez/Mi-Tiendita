using DAL.Servicios;
using ENTIDADES;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MODULO_CLIENTE.Controllers
{
    public class AccesoController : Controller
    {

        private readonly IClienteService clienteService;

        public AccesoController(IClienteService clienteService)
        {
            this.clienteService = clienteService;
        }


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
        public async Task<ActionResult> Registrar(CLIENTES cliente)
        {
            try
            {
                // Validación de contraseñas
                if (cliente.CLAVE != cliente.ConfirmarClave)
                {
                    ViewBag.Error = "Las contraseñas no coinciden.";
                    return View(cliente); // Retornamos el objeto para que no se borren los datos del formulario
                }

                var clienteresultado = await clienteService.Insertar(cliente);

                if (clienteresultado != null)
                {
                    return RedirectToAction("Index", "Acceso");
                }
                else
                {
                    ViewBag.Error = "No se pudo registrar el cliente. Inténtalo de nuevo.";
                    return View(cliente);
                }
            }
            catch (Exception ex)
            {
                // Si el servicio lanza tu excepción personalizada (ej. "El correo ya existe" o campos vacíos)
                ViewBag.Error = ex.Message;
                return View(cliente);
            }
        }


        [HttpPost]
        public async Task<ActionResult> Index(string correo, string clave)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(clave))
                {
                    ViewBag.Error = "Debe ingresar su correo y contraseña.";
                    return View();
                }

                var clientes = await clienteService.ObtenerTodos();
                CLIENTES cliente = clientes.FirstOrDefault(c => c.CORREO == correo && c.CLAVE == UtilService.EncriptarClave(clave));

                if (cliente == null)
                {
                    ViewBag.Error = "Correo electrónico o contraseña incorrectos.";
                    return View();
                }

                 //Si el cliente existe, evaluamos si debe restablecer clave
                if (cliente.RESTABLECER == true)
                {
                    TempData["idCliente"] = cliente.ID_CLIENTE;
                    return RedirectToAction("CambiarClave", "Acceso");
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(cliente.CORREO, false);
                    Session["cliente"] = cliente;
                    return RedirectToAction("Index", "Tienda");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ocurrió un error al intentar iniciar sesión: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Restablecer(string correo)
        {
            try
            {
                string correoLimpio = correo?.Trim();

                if (string.IsNullOrWhiteSpace(correoLimpio))
                {
                    ViewBag.Error = "Por favor, ingrese un correo válido.";
                    return View();
                }

                var clientes = await clienteService.ObtenerTodos();
                CLIENTES cliente = clientes.FirstOrDefault(u => u.CORREO == correoLimpio);

                if (cliente == null)
                {
                    ViewBag.Error = "No existe ninguna cuenta registrada con este correo electrónico.";
                    return View();
                }

                // Leemos la configuración
                string correoEmisor = ConfigurationManager.AppSettings["CorreoSoporte"];
                string claveEmisor = ConfigurationManager.AppSettings["ClaveCorreoSoporte"];

                bool respuesta = await clienteService.restablecerClave(cliente.ID_CLIENTE, correoLimpio, correoEmisor, claveEmisor);

                if (respuesta)
                {
                    // Opcional: Puedes usar TempData para enviar un mensaje de éxito a la vista de Login
                    TempData["Mensaje"] = "Se ha enviado una nueva contraseña a tu correo.";
                    return RedirectToAction("Index", "Acceso");
                }
                else
                {
                    ViewBag.Error = "Ocurrió un error interno al intentar enviar el correo. Intenta más tarde.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error crítico al restablecer la contraseña: " + ex.Message;
                return View();
            }
        }


        [HttpPost]
        public async Task<ActionResult> CambiarClave(string idCliente, string claveActual, string nuevaClave, string confirmarClave)
        {
            try
            {
                // 1. Validar que el ID no venga nulo o corrupto para evitar que int.Parse rompa la app
                if (!int.TryParse(idCliente, out int idParsed))
                {
                    ViewBag.Error = "Identificador de cliente inválido.";
                    return View();
                }

                var clientes = await clienteService.ObtenerTodos();
                CLIENTES cliente = clientes.FirstOrDefault(u => u.ID_CLIENTE == idParsed);

                if (cliente == null)
                {
                    ViewBag.Error = "No se encontró la información del usuario.";
                    return View();
                }

                // 2. Validar contraseña actual
                if (cliente.CLAVE != UtilService.EncriptarClave(claveActual))
                {
                    TempData["idCliente"] = idCliente;
                    ViewBag.Error = "La contraseña actual ingresada no es correcta.";
                    return View();
                }

                // 3. Validar coincidencia de nueva clave
                if (nuevaClave != confirmarClave)
                {
                    TempData["idCliente"] = idCliente;
                    ViewBag.Error = "Las nuevas contraseñas no coinciden.";
                    return View();
                }

                // 4. Procesar el cambio
                string claveEncriptada = UtilService.EncriptarClave(nuevaClave);
                bool respuesta = clienteService.cambiarClave(idParsed, claveEncriptada);

                if (respuesta)
                {
                    ViewBag.Exito = "Tu contraseña ha sido actualizada correctamente.";
                    return View();
                   
                }
                else
                {
                    TempData["idCliente"] = idCliente;
                    ViewBag.Error = "Ocurrió un error en la base de datos al cambiar la clave.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["idCliente"] = idCliente;
                ViewBag.Error = "Error inesperado al procesar el cambio de contraseña: " + ex.Message;
                return View();
            }
        }

        public ActionResult logout()
        {
            Session["cliente"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Acceso");
        }
    }
}