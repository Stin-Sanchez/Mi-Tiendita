using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ENTIDADES;
using DAL;
using DAL.Servicios;
using System.Threading.Tasks;
using DAO.DTOS;
using System.Data;
using ClosedXML.Excel;
using System.IO;
using System.Configuration;

namespace MODULO_ADMIN.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        //Inyectamos las dependecias correspondientes
        private readonly IUserService _userService;
        private readonly IVentaService _ventaService;
        private readonly IProductoService _productoService;

        public HomeController(IUserService userService, IVentaService ventaService, IProductoService productoService)
        {
            _userService = userService;
            _ventaService = ventaService;
            _productoService = productoService;
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
        public async Task<JsonResult> ListarUsuarios()
        {
            // Llamamos a la capa de negocio
            List<USUARIOS> listaUsuarios = (await _userService.ObtenerTodos()).ToList();

            // Retornamos el JSON. (En MVC 5, JsonRequestBehavior.AllowGet es obligatorio para peticiones GET)
            return Json(new { data = listaUsuarios }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task <JsonResult> GuardarUsuario(USUARIOS usuario)
        {
            //Inicializamos las variables de respuesta
            object resultado=null;
            string mensajeRespuesta = string.Empty;

            // Leemos el Web.config aquí en la capa de Presentación
            string correoSoporte = ConfigurationManager.AppSettings["CorreoSoporte"];
            string claveSoporte = ConfigurationManager.AppSettings["ClaveCorreoSoporte"];

            try
            {

                if (usuario.ID_USUARIO == 0)
                {

                    resultado = await _userService.Insertar(usuario,correoSoporte,claveSoporte);
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



        [HttpGet]
        public async Task<JsonResult> ObtenerResumenDashboard()
        {
            // Hacemos las 4 consultas en una sola petición HTTP.
            // Esto hace que la pantalla principal de tu sistema cargue casi instantáneamente.
            var dashboardData = new
            {
                totalUsuarios = await _userService.ObtenerTotalUsuarios(),
                ventasDia = await _ventaService.ObtenerSumaVentasDelDia(),
                totalProductos = await _productoService.ObtenerTotalProductos(),
                stockCritico = await _productoService.ObtenerProductosConStockCritico(5)
            };

            return Json(dashboardData, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public async Task<JsonResult> ListarHistorialVentas(string fechaInicio, string fechaFin, string idTransaccion)
        {
            // Llamas a tu capa de negocio/repositorio pasando los filtros
            var lista = await _ventaService.ObtenerHistorialVentasAsync(fechaInicio, fechaFin, idTransaccion);

            // Retornamos un JSON con la propiedad "data", que es lo que DataTables usa por defecto
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]

        public async Task<FileResult> ExportarVenta (string fechaInicio, string fechaFin, string IdTransaccion)
        {
            List<HistorialVentasDTO> oLista = new List<HistorialVentasDTO>();
            var lista = await _ventaService.ObtenerHistorialVentasAsync(fechaInicio, fechaFin, IdTransaccion);
            DataTable dt = new DataTable();
            dt.Locale = new System.Globalization.CultureInfo("es-EC");
            dt.Columns.Add("Fecha venta", typeof(string));
            dt.Columns.Add("Cliente", typeof(string));
            dt.Columns.Add("Producto", typeof(string));
            dt.Columns.Add("Precio", typeof(decimal));
            dt.Columns.Add("Cantidad", typeof(int));
            dt.Columns.Add("Total", typeof(decimal));
            dt.Columns.Add("IdTransaccion", typeof(string));

            foreach(HistorialVentasDTO rp in lista)
            {
                dt.Rows.Add(new object[]
                {
                    rp.FechaVenta,
                    rp.Cliente,
                    rp.Producto,
                    rp.Precio,
                    rp.Cantidad,
                    rp.Total,
                    rp.IdTransaccion
                });
            }

            dt.TableName = "Datos";

            using (XLWorkbook wb= new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    // Formateamos la fecha para evitar caracteres inválidos en el nombre
                    string nombreArchivo = "ReporteVenta_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".xlsx";
                    wb.SaveAs(stream);
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        nombreArchivo);
                }
            }


        }

    }
}