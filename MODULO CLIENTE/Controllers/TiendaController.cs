using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ENTIDADES;
using DAL.Servicios;

namespace MODULO_CLIENTE.Controllers
{
    public class TiendaController : Controller
    {
        //Inyectamos las dependecias correspondientes
     
      
        private readonly IMarcaService _marcaService;
        private readonly ICategoriasService _categoriaService;

        public TiendaController(IMarcaService marcaService, ICategoriasService categoriaService)
        {
            _marcaService = marcaService;
            _categoriaService = categoriaService;
        }



        // GET: Tienda
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]

        public async Task<JsonResult> ListaCategorias()
        {
            // Llamamos a la capa de negocio
            List<CATEGORIAS> lista = (await _categoriaService.ObtenerTodasLasCategorias()).ToList();

            // Retornamos el JSON. (En MVC 5, JsonRequestBehavior.AllowGet es obligatorio para peticiones GET)
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]

        public async Task<JsonResult> ListarMarcaPorCategorias(long idCategoria)
        {
            // Llamamos a la capa de negocio
            List<MARCAS> lista = (await  _marcaService.ObtenerMarcasPorCategoria(idCategoria)).ToList();

            // Retornamos el JSON. (En MVC 5, JsonRequestBehavior.AllowGet es obligatorio para peticiones GET)
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }



        public async Task<JsonResult> ListarProductos(long idCategoria, long idMarca)
        {
            // Llamamos a la capa de negocio
            List<MARCAS> lista = (await _marcaService.ObtenerMarcasPorCategoria(idCategoria)).ToList();

            // Retornamos el JSON. (En MVC 5, JsonRequestBehavior.AllowGet es obligatorio para peticiones GET)
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }

    }
}