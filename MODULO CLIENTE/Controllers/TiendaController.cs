using DAL.Servicios;
using ENTIDADES;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;


namespace MODULO_CLIENTE.Controllers
{
    public class TiendaController : Controller
    {
        //Inyectamos las dependecias correspondientes


        private readonly IMarcaService _marcaService;
        private readonly ICategoriasService _categoriaService;
        private readonly IProductoService _productoService;
        private static MemoryCache _cache = MemoryCache.Default;

        public TiendaController(IMarcaService marcaService, ICategoriasService categoriaService, IProductoService productoService)
        {
            _marcaService = marcaService;
            _categoriaService = categoriaService;
            _productoService = productoService;
        }


        [OutputCache(Duration = 300, Location = OutputCacheLocation.Client)]
        public ActionResult Index()
        {
            return View();
        }



        public async Task<ActionResult> DetalleProducto(int idProducto = 0)
        {
            PRODUCTOS producto = new PRODUCTOS();
          

            producto = await _productoService.ObtenerPorId(idProducto);

            if(producto != null)
            {
               ObtenerImagen(producto.NOMBRE_IMAGEN);
            }else
            {
                return HttpNotFound("Product not found");
            }
            return View(producto);
        }

        [HttpGet]

        public async Task<JsonResult> ListaCategorias()
         {
            const string cacheKey = "lista_categorias";
            var lista = _cache[cacheKey] as List<object>;

            if (lista == null)
            {
                lista = (await _categoriaService.ObtenerTodasLasCategorias())
                    .Select(c => new { ID_CATEGORIA = c.ID_CATEGORIA, NOMBRE = c.NOMBRE })
                    .Cast<object>()
                    .ToList();

                _cache.Set(cacheKey, lista, new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30)
                });
            }

            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]

        public async Task<JsonResult> ListarMarcaPorCategorias(long idCategoria)
        {
            string cacheKey = $"marcas_categoria_{idCategoria}";
            var lista = _cache[cacheKey] as List<object>;

            if (lista == null)
            {
                lista = (await _marcaService.ObtenerMarcasPorCategoria(idCategoria))
                    .Select(m => new { ID_MARCA = m.ID_MARCA, NOMBRE = m.NOMBRE })
                    .Cast<object>()
                    .ToList();

                _cache.Set(cacheKey, lista, new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15)
                });
            }

            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }
        



        [HttpPost]
        public async Task<JsonResult> ListarProductos(long idCategoria, long idMarca)
        {
            string cacheKey = $"productos_{idCategoria}_{idMarca}";
            var productos = _cache[cacheKey] as List<object>;

            if (productos == null)
            {
                productos = (await _productoService.ObtenerTodos())
                    .Where(p => (idCategoria == 0 || p.ID_CATEGORIA == idCategoria)
                                && (idMarca == 0 || p.ID_MARCA == idMarca)
                                && p.STOCK.HasValue && p.STOCK > 0)
                    .Select(p => new
                    {
                        ID_PRODUCTO = p.ID_PRODUCTO,
                        NOMBRE = p.NOMBRE,
                        DESCRIPCION = p.DESCRIPCION,
                        PRECIO = p.PRECIO,
                        STOCK = p.STOCK ?? 0,
                        IMAGEN_URL = p.RUTA_IMAGEN,
                        NOMBRE_IMAGEN = p.NOMBRE_IMAGEN,
                        ID_CATEGORIA = p.ID_CATEGORIA ?? 0,
                        ID_MARCA = p.ID_MARCA ?? 0,
                        MARCA = p.MARCAS != null ? p.MARCAS.NOMBRE : string.Empty,
                        CATEGORIA = p.CATEGORIAS != null ? p.CATEGORIAS.NOMBRE : string.Empty
                    })
                    .Cast<object>()
                    .ToList();

                _cache.Set(cacheKey, productos, new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10)
                });
            }

            return Json(new { data = productos }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult ObtenerImagen(string nombreImagen)
        {
            // Nota de Arquitectura: Dado que MODULO ADMIN y MODULO CLIENTE son proyectos separados,
            // las imágenes físicas residen en la carpeta del administrador.
            // Accedemos a ellas subiendo un nivel en el árbol de directorios del servidor.
            
            string pathBase = AppDomain.CurrentDomain.BaseDirectory;
            string pathImagen = Path.Combine(pathBase, "..", "MODULO ADMIN", "Imagenes", "Productos", nombreImagen);

            if (System.IO.File.Exists(pathImagen))
            {
                string extension = Path.GetExtension(nombreImagen).ToLower();
                string contentType = "image/jpeg"; // Default

                switch (extension)
                {
                    case ".png": contentType = "image/png"; break;
                    case ".gif": contentType = "image/gif"; break;
                    case ".webp": contentType = "image/webp"; break;
                }

                return File(pathImagen, contentType);
            }

            // Si no existe, podrías retornar una imagen por defecto o un 404 controlado.
            return HttpNotFound();
        }
    }
}