using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DAL.Servicios;
using ENTIDADES;

namespace MODULO_ADMIN.Controllers
{

    [Authorize]
    public class MantenedorController : Controller
    {

        // 1. Declaras tu servicio a nivel de clase, de solo lectura
        private readonly IMarcaService _marcaService;
        private readonly ICategoriasService _categoriaService;
        private readonly IProductoService _productoService;

        public MantenedorController(IMarcaService marcaService, ICategoriasService categoriaService, IProductoService productoService)
        {
            _marcaService = marcaService;
            _categoriaService = categoriaService;
            _productoService = productoService;
        }




        // GET: Mantenedor/Categorias
        public ActionResult Categorias()
        {
            return View();
        }

        // GET: Mantenedor/Marca
        public ActionResult Marca()
        {
            return View();
        }

        // GET: Mantenedor/Producto
        public ActionResult Producto()
        {
            return View();
        }


        #region Marcas
        //API MARCAS

        /// <summary>
        /// Obtengo el catálogo de marcas y lo transformo en una lista plana.
        /// Tomo esta decisión para evitar el error de referencia circular al serializar, 
        /// ya que las marcas tienen una propiedad virtual que apunta hacia los productos.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> ObtenerMarcas()
        {
            // 1. Traigo la data cruda desde la base de datos (con los Proxies Dinámicos de EF).
            var listaMarcasBD =  (await _marcaService.ObtenerTodasLasMarcas()).ToList();

            // 2. Extraigo solo lo que el frontend necesita usando LINQ, rompiendo el enlace circular.
            var listaMarcasPlana = listaMarcasBD.Select(m => new
            {
                ID_MARCA = m.ID_MARCA,
                NOMBRE = m.NOMBRE,
                DESCRIPCION = m.DESCRIPCION,
                ACTIVO = m.ACTIVO
            }).ToList();

            // 3. Retorno el JSON limpio.
            return Json(new { data = listaMarcasPlana }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardarMarca(MARCAS marca)
        {
            // 1. Inicializamos las variables de respuesta
            object resultado = null;
            string mensajeRespuesta = string.Empty;

            try
            {

                if (marca.ID_MARCA == 0)
                {

                    resultado = _marcaService.Insertar(marca);
                    mensajeRespuesta = "Marca creada correctamente.";
                }
                else
                {
                    // 1. Buscamos la marca tal como está en la base de datos

                    var marcaOriginal = _marcaService.ObtenerPorId(marca.ID_MARCA);

                    if (marcaOriginal != null)
                    {
                        // 2. Sobrescribimos SOLO las propiedades que el usuario editó en el modal
                        marcaOriginal.NOMBRE = marca.NOMBRE;
                        marcaOriginal.DESCRIPCION = marca.DESCRIPCION;
                        marcaOriginal.ACTIVO = marca.ACTIVO;

                        // 3. Mandamos a actualizar el objeto original. s
                        resultado = _marcaService.Actualizar(marcaOriginal);
                        mensajeRespuesta = "Marca actualizada correctamente.";
                    }
                    else
                    {
                        mensajeRespuesta = "Error: No se encontró la marca a actualizar.";
                    }
                }
            }
            catch (Exception ex)
            {
                // Si algo explota en la base de datos, caemos aquí
                mensajeRespuesta = "Error al guardar la marca: " + ex.Message;
            }

            //Retornamos el objeto anónimo. 

            return Json(new { resultado = resultado, mensaje = mensajeRespuesta });
        }

        [HttpPost]
        public JsonResult EliminarMarca(long id)
        {
            bool respuesta = false;
            string mensajeRespuesta = string.Empty;

            try
            {
                // Llamamos al metodo
                _marcaService.Eliminar(id);

                // Si la línea anterior no explota, significa que se eliminó/desactivó con éxito
                respuesta = true;
                mensajeRespuesta = "Marca desactivada correctamente.";
            }
            catch (Exception ex)
            {
                // Si hay algún problema en la base de datos, lo atrapamos aquí
                respuesta = false;
                mensajeRespuesta = "Error al desactivar la marca, porque esta asociada a productos activos: " + ex.Message;
            }

            // Devolvemos el JSON
            return Json(new { resultado = respuesta, mensaje = mensajeRespuesta });
        }

        #endregion



        #region Categorias
        //API CATEGORIAS
        /// <summary>
        /// Obtengo el catálogo de marcas y lo transformo en una lista plana.
        /// Tomo esta decisión para evitar el error de referencia circular al serializar, 
        /// ya que las marcas tienen una propiedad virtual que apunta hacia los productos.
        /// </summary>
        /// <summary>
        /// Obtengo el catálogo de categorías y lo convierto a objetos anónimos seguros.
        /// Al igual que con las marcas, esto evita que el serializador JSON colapse intentando 
        /// leer todos los productos asociados a cada categoría.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> ObtenerCategorias()
        {
            // Nota de Líder Técnico: Si arreglaste el nombre del método en el servicio, usa ObtenerTodasLasCategorias().
            // Si dejaste el nombre anterior por error, usa ObtenerTodasLasMarcas().
            var listaCategoriasBD = (await _categoriaService.ObtenerTodasLasCategorias()) .ToList();

            var listaCategoriasPlana = listaCategoriasBD.Select(c => new
            {
                ID_CATEGORIA = c.ID_CATEGORIA,
                NOMBRE = c.NOMBRE,
                DESCRIPCION = c.DESCRIPCION,
                ACTIVO = c.ACTIVO
            }).ToList();

            return Json(new { data = listaCategoriasPlana }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardarCategoria(CATEGORIAS categoria)
        {
            // 1. Inicializamos las variables de respuesta
            object resultado = null;
            string mensajeRespuesta = string.Empty;

            try
            {

                if (categoria.ID_CATEGORIA == 0)
                {

                    resultado = _categoriaService.Insertar(categoria);
                    mensajeRespuesta = "Categoria creada correctamente.";
                }
                else
                {
                    // 1. Buscamos la marca tal como está en la base de datos

                    var categoriaOriginal = _categoriaService.ObtenerPorId(categoria.ID_CATEGORIA);

                    if (categoriaOriginal != null)
                    {
                        // 2. Sobrescribimos SOLO las propiedades que el usuario editó en el modal
                        categoriaOriginal.NOMBRE = categoria.NOMBRE;
                        categoriaOriginal.DESCRIPCION = categoria.DESCRIPCION;
                        categoriaOriginal.ACTIVO = categoria.ACTIVO;

                        // 3. Mandamos a actualizar el objeto original. s
                        resultado = _categoriaService.Actualizar(categoriaOriginal);
                        mensajeRespuesta = "Categoria actualizada correctamente.";
                    }
                    else
                    {
                        mensajeRespuesta = "Error: No se encontró la categoria a actualizar.";
                    }
                }
            }
            catch (Exception ex)
            {
                // Si algo explota en la base de datos, caemos aquí
                mensajeRespuesta = "Error al guardar la categoria: " + ex.Message;
            }

            //Retornamos el objeto anónimo. 

            return Json(new { resultado = resultado, mensaje = mensajeRespuesta });
        }


        [HttpPost]
        public JsonResult EliminarCategoria(long id)
        {
            bool respuesta = false;
            string mensajeRespuesta = string.Empty;

            try
            {
                // Llamamos al metodo
                _categoriaService.Eliminar(id);

                // Si la línea anterior no explota, significa que se eliminó/desactivó con éxito
                respuesta = true;
                mensajeRespuesta = "Marca desactivada correctamente.";
            }
            catch (Exception ex)
            {
                // Si hay algún problema en la base de datos, lo atrapamos aquí
                respuesta = false;
                mensajeRespuesta = "Error al desactivar la marca, porque esta asociada a productos activos: " + ex.Message;
            }

            // Devolvemos el JSON
            return Json(new { resultado = respuesta, mensaje = mensajeRespuesta });
        }
        #endregion




        #region Productos
        //API PRODUCTOS
        /// <summary>
        /// Expongo el catálogo completo de productos hacia el frontend.
        /// Al hacerlo, intercepto y transformo los datos en objetos anónimos (Data Transfer Objects al vuelo) 
        /// para evitar el clásico error de referencia circular que genera Entity Framework 
        /// al intentar serializar propiedades de navegación bidireccionales.
        /// </summary>
        /// <returns>Un JSON con la lista de productos plana y segura para el navegador.</returns>
        [HttpGet]
        public async Task<ActionResult> ObtenerProductos()
        {
            // Recupero la lista completa de entidades desde la capa de servicio.
            // Aquí los datos vienen con los "DynamicProxies" de Entity Framework, que son los causantes del bucle infinito si los serializo directo.
            var listaProductosBD = (await _productoService.ObtenerTodos()).ToList();

            // Transformo la lista compleja de base de datos en una lista plana y sencilla usando LINQ.
            // Tomo esta decisión técnica porque el serializador JSON nativo de MVC se marea 
            // cuando un Producto apunta a una Categoría, y esa Categoría apunta de vuelta al Producto.
            // Al mapear a un objeto anónimo nuevo, extraigo solo los valores primitivos y rompo esa cadena de relaciones.
            var listaProductosPlana = listaProductosBD.Select(p => new
            {
                ID_PRODUCTO = p.ID_PRODUCTO,
                NOMBRE = p.NOMBRE,
                DESCRIPCION = p.DESCRIPCION,
                PRECIO = p.PRECIO,
                STOCK = p.STOCK,
                RUTA_IMAGEN = p.RUTA_IMAGEN,
                NOMBRE_IMAGEN = p.NOMBRE_IMAGEN,
                ACTIVO = p.ACTIVO,

                // Mantengo los IDs para que el modal de edición (abrirModal) pueda pre-seleccionar los comboboxes correctamente.
                ID_MARCA = p.ID_MARCA,
                ID_CATEGORIA = p.ID_CATEGORIA,

                // Como valor agregado, extraigo los nombres en texto plano de las asociaciones.
                // Hago esto validando si son nulos primero, para evitar una excepción de "Referencia a objeto no establecida" 
                // en caso de que algún producto haya quedado huérfano en la base de datos.
                MarcaDescripcion = p.MARCAS != null ? p.MARCAS.NOMBRE : "Sin Marca",
                CategoriaDescripcion = p.CATEGORIAS != null ? p.CATEGORIAS.NOMBRE : "Sin Categoría"
            }).ToList();

            // Retorno la lista plana empaquetada en la propiedad 'data' que DataTables espera.
            // Ahora el serializador JSON funcionará a la perfección y de manera mucho más rápida.
            return Json(new { data = listaProductosPlana }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Proceso la solicitud de guardado de un producto. 
        /// Además de los datos de texto, ahora tengo la responsabilidad de recibir un archivo físico (la foto del producto), 
        /// guardarlo de forma segura en el servidor y registrar su ubicación en la base de datos.
        /// </summary>
        /// <param name="producto">El objeto producto con los datos capturados en el formulario.</param>
        /// <param name="archivoImagen">El archivo físico que intercepto directamente desde la petición multipart del frontend.</param>
        /// <returns>Un JSON con el estado de la operación para que la vista muestre la alerta correspondiente.</returns>
        [HttpPost]
        public JsonResult GuardarProducto(PRODUCTOS producto, HttpPostedFileBase archivoImagen)
        {
            // Variables nativas (simples), NINGÚN objeto de base de datos aquí.
            string mensajeRespuesta = string.Empty;
            bool operacionExitosa = false;

            try
            {
                // --- 1. PROCESAMIENTO DE LA IMAGEN ---
                if (archivoImagen != null && archivoImagen.ContentLength > 0)
                {
                    string extension = Path.GetExtension(archivoImagen.FileName);
                    string nombreImagen = Guid.NewGuid().ToString() + extension;
                    string rutaLogica = "~/Imagenes/Productos/";
                    string rutaFisica = Server.MapPath(rutaLogica);

                    if (!Directory.Exists(rutaFisica))
                    {
                        Directory.CreateDirectory(rutaFisica);
                    }

                    archivoImagen.SaveAs(Path.Combine(rutaFisica, nombreImagen));
                    producto.NOMBRE_IMAGEN = nombreImagen;
                    producto.RUTA_IMAGEN = rutaLogica;
                }

                // --- 2. LÓGICA DE BASE DE DATOS ---
                if (producto.ID_PRODUCTO == 0)
                {
                    // Insertar
                    _productoService.Insertar(producto);
                    operacionExitosa = true;
                    mensajeRespuesta = "Producto creado correctamente.";
                }
                else
                {
                    // Actualizar
                    var productoOriginal = _productoService.ObtenerPorId(producto.ID_PRODUCTO);

                    if (productoOriginal != null)
                    {
                        productoOriginal.NOMBRE = producto.NOMBRE;
                        productoOriginal.DESCRIPCION = producto.DESCRIPCION;
                        productoOriginal.ACTIVO = producto.ACTIVO;
                        productoOriginal.PRECIO = producto.PRECIO;
                        productoOriginal.STOCK = producto.STOCK;

                      

                        if (archivoImagen != null && archivoImagen.ContentLength > 0)
                        {
                            productoOriginal.RUTA_IMAGEN = producto.RUTA_IMAGEN;
                            productoOriginal.NOMBRE_IMAGEN = producto.NOMBRE_IMAGEN;
                        }

                        _productoService.Actualizar(productoOriginal);
                        operacionExitosa = true;
                        mensajeRespuesta = "Producto actualizado correctamente.";


                    }
                    else
                    {
                        operacionExitosa = false;
                        mensajeRespuesta = "Error: No se encontró el producto a actualizar.";
                    }
                }
            }
            catch (Exception ex)
            {
                operacionExitosa = false;
                mensajeRespuesta = "Error al procesar el producto: " + ex.Message;
            }

            // --- LA SOLUCIÓN DEFINITIVA ---
            // Retornamos ESTRICTAMENTE dos valores simples (un booleano y un string). 
            // Es físicamente imposible que esto cause un error de referencia circular.
            return Json(new { exito = operacionExitosa, mensaje = mensajeRespuesta });
        }


        [HttpPost]
        public JsonResult EliminarProducto(long id)
        {
            bool respuesta = false;
            string mensajeRespuesta = string.Empty;

            try
            {
                // Llamamos al metodo
                _productoService.Eliminar(id);

                // Si la línea anterior no explota, significa que se eliminó/desactivó con éxito
                respuesta = true;
                mensajeRespuesta = "Producto desactivado correctamente.";
            }
            catch (Exception ex)
            {
                // Si hay algún problema en la base de datos, lo atrapamos aquí
                respuesta = false;
                mensajeRespuesta = "Error al desactivar el producto: " + ex.Message;
            }

            // Devolvemos el JSON
            return Json(new { resultado = respuesta, mensaje = mensajeRespuesta });
        }
        #endregion
    }
}