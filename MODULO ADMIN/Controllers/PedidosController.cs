using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DAL.Servicios;
using ENTIDADES;

namespace MODULO_ADMIN.Controllers
{
    [Authorize]
    public class PedidosController : Controller
    {
        private readonly IVentaService _ventaService;

        public PedidosController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        public ActionResult Index() => View();

        [HttpGet]
        public async Task<JsonResult> ListarPedidos(string fechaInicio, string fechaFin, string idTransaccion)
        {
            // Default to last 30 days if no filter provided
            if (string.IsNullOrEmpty(fechaInicio))
                fechaInicio = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            if (string.IsNullOrEmpty(fechaFin))
                fechaFin = DateTime.Now.ToString("yyyy-MM-dd");

            var lista = await _ventaService.ObtenerHistorialVentasAsync(fechaInicio, fechaFin, idTransaccion);
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DetallePedido(int id)
        {
            var venta = _ventaService.ObtenerPorId(id);
            if (venta == null)
                return Json(new { resultado = false }, JsonRequestBehavior.AllowGet);

            var detalle = venta.DETALLE_VENTAS?.Select(d => new {
                d.ID_DETALLE,
                Producto = d.PRODUCTOS?.NOMBRE ?? "—",
                d.CANTIDAD,
                d.TOTAL
            });

            return Json(new {
                resultado = true,
                idTransaccion = venta.ID_TRANSACCION,
                cliente    = venta.CLIENTES?.NOMBRE + " " + venta.CLIENTES?.APELLIDO,
                contacto   = venta.CONTACTO,
                telefono   = venta.TELEFONO,
                direccion  = venta.DIRECCION,
                fecha      = venta.FECHA_VENTA.ToString("dd/MM/yyyy HH:mm"),
                montoTotal = venta.MONTO_TOTAL,
                detalle
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
