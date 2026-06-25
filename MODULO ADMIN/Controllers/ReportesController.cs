using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DAL.Servicios;

namespace MODULO_ADMIN.Controllers
{
    [Authorize]
    public class ReportesController : Controller
    {
        private readonly IVentaService _ventaService;
        private readonly IProductoService _productoService;
        private readonly IClienteService _clienteService;

        public ReportesController(IVentaService ventaService, IProductoService productoService, IClienteService clienteService)
        {
            _ventaService = ventaService;
            _productoService = productoService;
            _clienteService = clienteService;
        }

        public ActionResult Index() => View();

        [HttpGet]
        public async Task<JsonResult> VentasPorPeriodo(string fechaInicio, string fechaFin)
        {
            if (string.IsNullOrEmpty(fechaInicio)) fechaInicio = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            if (string.IsNullOrEmpty(fechaFin)) fechaFin = DateTime.Now.ToString("yyyy-MM-dd");

            var ventas = await _ventaService.ObtenerHistorialVentasAsync(fechaInicio, fechaFin, null);
            var totalMonto = ventas.Sum(v => v.Total);
            var totalTransacciones = ventas.Select(v => v.IdTransaccion).Distinct().Count();
            var ticketPromedio = totalTransacciones > 0 ? totalMonto / totalTransacciones : 0;

            // Group by date for chart
            var porFecha = ventas
                .GroupBy(v => v.FechaVenta?.Split(' ')[0] ?? "")
                .OrderBy(g => g.Key)
                .Select(g => new { fecha = g.Key, monto = g.Sum(v => v.Total), pedidos = g.Select(v => v.IdTransaccion).Distinct().Count() })
                .ToList();

            // Top productos
            var topProductos = ventas
                .GroupBy(v => v.Producto)
                .Select(g => new { producto = g.Key, cantidad = g.Sum(v => v.Cantidad), total = g.Sum(v => v.Total) })
                .OrderByDescending(x => x.total)
                .Take(10)
                .ToList();

            // Top clientes
            var topClientes = ventas
                .Where(v => !string.IsNullOrEmpty(v.Cliente))
                .GroupBy(v => v.Cliente)
                .Select(g => new { cliente = g.Key, pedidos = g.Select(v => v.IdTransaccion).Distinct().Count(), total = g.Sum(v => v.Total) })
                .OrderByDescending(x => x.total)
                .Take(10)
                .ToList();

            return Json(new {
                resultado = true,
                kpi = new { totalMonto, totalTransacciones, ticketPromedio },
                porFecha,
                topProductos,
                topClientes
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> StockPorCategoria()
        {
            var productos = await _productoService.ObtenerTodos();
            var data = productos
                .GroupBy(p => p.CATEGORIAS?.NOMBRE ?? "Sin categoría")
                .Select(g => new { categoria = g.Key, totalProductos = g.Count(), stockTotal = g.Sum(p => p.STOCK ?? 0), criticos = g.Count(p => (p.STOCK ?? 0) <= 5) })
                .OrderByDescending(x => x.totalProductos)
                .ToList();
            return Json(new { resultado = true, data }, JsonRequestBehavior.AllowGet);
        }
    }
}
