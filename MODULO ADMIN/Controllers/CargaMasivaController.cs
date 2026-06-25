using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DAL.Servicios;
using ENTIDADES;

namespace MODULO_ADMIN.Controllers
{
    [Authorize]
    public class CargaMasivaController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriasService _categoriasService;
        private readonly IMarcaService _marcaService;

        public CargaMasivaController(IProductoService productoService, ICategoriasService categoriasService, IMarcaService marcaService)
        {
            _productoService = productoService;
            _categoriasService = categoriasService;
            _marcaService = marcaService;
        }

        // GET: CargaMasiva
        public ActionResult Index() => View();

        [HttpGet]
        public ActionResult DescargarPlantilla(string tipo)
        {
            string csv = "";
            string fileName = "";

            switch (tipo?.ToLower())
            {
                case "productos":
                    csv = "NOMBRE,DESCRIPCION,PRECIO,STOCK,ID_CATEGORIA,ID_MARCA\r\nEjemplo Producto,Descripcion del producto,99.99,50,1,1";
                    fileName = "plantilla_productos.csv";
                    break;
                case "categorias":
                    csv = "NOMBRE,DESCRIPCION\r\nEjemplo Categoria,Descripcion de la categoria";
                    fileName = "plantilla_categorias.csv";
                    break;
                case "marcas":
                    csv = "NOMBRE,DESCRIPCION\r\nEjemplo Marca,Descripcion de la marca";
                    fileName = "plantilla_marcas.csv";
                    break;
                default:
                    return HttpNotFound();
            }

            byte[] bytes = Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", fileName);
        }

        [HttpPost]
        public async Task<JsonResult> ImportarCSV(HttpPostedFileBase archivo, string tipo)
        {
            if (archivo == null || archivo.ContentLength == 0)
                return Json(new { resultado = false, mensaje = "No se seleccionó ningún archivo" });

            var ext = Path.GetExtension(archivo.FileName)?.ToLower();
            if (ext != ".csv")
                return Json(new { resultado = false, mensaje = "Solo se aceptan archivos .csv" });

            try
            {
                using (var reader = new StreamReader(archivo.InputStream, Encoding.UTF8))
                {
                    var lines = new List<string>();
                    string line;
                    while ((line = reader.ReadLine()) != null)
                        lines.Add(line);

                    if (lines.Count < 2)
                        return Json(new { resultado = false, mensaje = "El archivo no tiene datos (solo encabezado)" });

                    int insertados = 0, errores = 0;
                    var erroresMsgs = new List<string>();

                    // Skip header (line 0)
                    for (int i = 1; i < lines.Count; i++)
                    {
                        if (string.IsNullOrWhiteSpace(lines[i])) continue;
                        var cols = lines[i].Split(',');

                        try
                        {
                            switch (tipo?.ToLower())
                            {
                                case "productos":
                                    if (cols.Length < 6) throw new Exception("Columnas insuficientes");
                                    await _productoService.Insertar(new PRODUCTOS
                                    {
                                        NOMBRE = cols[0].Trim(),
                                        DESCRIPCION = cols[1].Trim(),
                                        PRECIO = decimal.Parse(cols[2].Trim()),
                                        STOCK = int.Parse(cols[3].Trim()),
                                        ID_CATEGORIA = int.TryParse(cols[4].Trim(), out int idCat) ? (int?)idCat : null,
                                        ID_MARCA = int.TryParse(cols[5].Trim(), out int idMar) ? (int?)idMar : null,
                                        ACTIVO = true,
                                        FECHA_CREACION = DateTime.Now
                                    });
                                    insertados++;
                                    break;
                                case "categorias":
                                    if (cols.Length < 2) throw new Exception("Columnas insuficientes");
                                    await _categoriasService.Insertar(new CATEGORIAS
                                    {
                                        NOMBRE = cols[0].Trim(),
                                        DESCRIPCION = cols[1].Trim(),
                                        ACTIVO = true,
                                        FECHA_CREACION = DateTime.Now
                                    });
                                    insertados++;
                                    break;
                                case "marcas":
                                    if (cols.Length < 2) throw new Exception("Columnas insuficientes");
                                    await _marcaService.Insertar(new MARCAS
                                    {
                                        NOMBRE = cols[0].Trim(),
                                        DESCRIPCION = cols[1].Trim(),
                                        ACTIVO = true,
                                        FECHA_CREACION = DateTime.Now
                                    });
                                    insertados++;
                                    break;
                                default:
                                    return Json(new { resultado = false, mensaje = "Tipo de importación no válido" });
                            }
                        }
                        catch (Exception ex)
                        {
                            errores++;
                            erroresMsgs.Add($"Fila {i + 1}: {ex.Message}");
                        }
                    }

                    return Json(new
                    {
                        resultado = true,
                        mensaje = $"{insertados} registros importados correctamente. {errores} errores.",
                        insertados,
                        errores,
                        detalleErrores = erroresMsgs
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { resultado = false, mensaje = "Error al procesar el archivo: " + ex.Message });
            }
        }
    }
}
