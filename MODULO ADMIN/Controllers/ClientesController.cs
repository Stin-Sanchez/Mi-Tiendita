using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DAL.Servicios;
using ENTIDADES;

namespace MODULO_ADMIN.Controllers
{
    public class ClientesController : Controller
    {
        private readonly IClienteService _clienteService;
        public ClientesController(IClienteService clienteService) { _clienteService = clienteService; }

        public ActionResult Index() => View();

        [HttpGet]
        public async Task<JsonResult> ListarClientes()
        {
            var lista = await _clienteService.ObtenerTodos();
            var data = lista.Select(c => new {
                c.ID_CLIENTE,
                c.NOMBRE,
                c.APELLIDO,
                c.CORREO,
                c.ACTIVO,
                FechaCreacion = c.FECHA_CREACION.ToString("dd/MM/yyyy")
            });
            return Json(new { data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> ToggleActivo(int id)
        {
            try {
                var cliente = await _clienteService.ObtenerPorId(id);
                if (cliente == null) return Json(new { resultado = false, mensaje = "Cliente no encontrado" });
                cliente.ACTIVO = !cliente.ACTIVO;
                await _clienteService.Actualizar(cliente);
                return Json(new { resultado = true, activo = cliente.ACTIVO });
            } catch (System.Exception ex) {
                return Json(new { resultado = false, mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> EliminarCliente(int id)
        {
            try {
                await _clienteService.Eliminar(id);
                return Json(new { resultado = true });
            } catch (System.Exception ex) {
                return Json(new { resultado = false, mensaje = ex.Message });
            }
        }
    }
}
