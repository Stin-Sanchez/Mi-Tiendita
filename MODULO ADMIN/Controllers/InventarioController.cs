using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DAL.Servicios;

namespace MODULO_ADMIN.Controllers
{
    [Authorize]
    public class InventarioController : Controller
    {
        private readonly IProductoService _productoService;

        public InventarioController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        public ActionResult Index() => View();

        [HttpGet]
        public async Task<JsonResult> ListarInventario()
        {
            var productos = await _productoService.ObtenerTodos();
            var data = productos.Select(p => new {
                p.ID_PRODUCTO,
                p.NOMBRE,
                Categoria = p.CATEGORIAS != null ? p.CATEGORIAS.NOMBRE : "—",
                Marca     = p.MARCAS    != null ? p.MARCAS.NOMBRE    : "—",
                p.PRECIO,
                Stock  = p.STOCK ?? 0,
                Estado = p.ACTIVO ? "Activo" : "Inactivo",
                Alerta = (p.STOCK ?? 0) <= 5 ? "critico" : (p.STOCK ?? 0) <= 15 ? "bajo" : "ok"
            });
            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> AjustarStock(int idProducto, int cantidad, string tipo)
        {
            try
            {
                var producto = await _productoService.ObtenerPorId(idProducto);
                if (producto == null)
                    return Json(new { resultado = false, mensaje = "Producto no encontrado" });

                int stockActual = producto.STOCK ?? 0;

                if (tipo == "entrada")
                    producto.STOCK = stockActual + cantidad;
                else if (tipo == "salida")
                    producto.STOCK = Math.Max(0, stockActual - cantidad);
                else
                    return Json(new { resultado = false, mensaje = "Tipo no válido" });

                await _productoService.Actualizar(producto);

                return Json(new {
                    resultado  = true,
                    nuevoStock = producto.STOCK,
                    nuevaAlerta = (producto.STOCK ?? 0) <= 5 ? "critico" : (producto.STOCK ?? 0) <= 15 ? "bajo" : "ok",
                    mensaje    = $"Stock actualizado a {producto.STOCK} unidades"
                });
            }
            catch (Exception ex)
            {
                return Json(new { resultado = false, mensaje = ex.Message });
            }
        }
    }
}
