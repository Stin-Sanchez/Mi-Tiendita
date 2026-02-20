using ENTIDADES.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ENTIDADES.RepositoryImp;
using DAO;
using DAO.DTOS;

namespace ENTIDADES.RepositoryImp
{
   public  class VentaRepositoryImp : RepositoryImp<VENTAS>, IVentaRepository
    {

        // Le pasamos el context a la clase padre (base)
        public VentaRepositoryImp(ModelContext context) : base(context)
        {
        }

        public async Task<List<HistorialVentasDTO>> ObtenerHistorialVentasAsync(string fechaInicio, string fechaFin, string idTransaccion)
        {
            // 1. Construimos la consulta base con los INNER JOIN idénticos  SQL
            var query = from dv in _context.DETALLE_VENTAS
                        join p in _context.PRODUCTOS on dv.ID_PRODUCTO equals p.ID_PRODUCTO
                        join v in _context.VENTAS on dv.ID_VENTA equals v.ID_VENTA
                        join c in _context.CLIENTES on v.ID_CLIENTE equals c.ID_CLIENTE
                        select new
                        {
                            v.FECHA_VENTA, // Lo mantenemos como DateTime temporalmente para poder filtrar
                            ClienteNombreCompleto = c.NOMBRE + " " + c.APELLIDO,
                            ProductoNombre = p.NOMBRE,
                            p.PRECIO,
                            dv.CANTIDAD,
                            dv.TOTAL,
                            v.ID_TRANSACCION
                        };

            // 2. Aplicamos filtros dinámicos si el usuario envió datos
            if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFin))
            {
                // Convertimos los strings del input a DateTime
                DateTime dtInicio = Convert.ToDateTime(fechaInicio);
                DateTime dtFin = Convert.ToDateTime(fechaFin).AddDays(1); // Sumamos 1 día para incluir todas las horas del día final

                query = query.Where(x => x.FECHA_VENTA >= dtInicio && x.FECHA_VENTA < dtFin);
            }

            if (!string.IsNullOrEmpty(idTransaccion))
            {
                query = query.Where(x => x.ID_TRANSACCION == idTransaccion);
            }

            // 3. Ejecutamos la consulta en la BD
            var resultadoDb = await query.ToListAsync();

            // 4. Mapeamos al DTO final, dándole formato a la fecha
            var listaFinal = resultadoDb.Select(x => new HistorialVentasDTO
            {
                FechaVenta = x.FECHA_VENTA.ToString("dd/MM/yyyy"), // Formateamos la fecha para la vista
                Cliente = x.ClienteNombreCompleto,
                Producto = x.ProductoNombre,
                Precio = x.PRECIO, // Asegúrate de que p.Precio sea decimal en tu modelo
                Cantidad = x.CANTIDAD ?? 0,
                Total = x.TOTAL,
                IdTransaccion = x.ID_TRANSACCION
            }).ToList();

            return listaFinal;
        }

        public async Task<decimal> ObtenerSumaVentasDelDiaAsync()
        {
            DateTime hoy = DateTime.Today;
            DateTime mañana = hoy.AddDays(1);

            // El casteo (decimal?) previene errores si no hay ventas ese día (devuelve null, y el ?? 0m lo convierte en 0)
            return await _context.VENTAS
                .Where(v => v.FECHA_VENTA >= hoy && v.FECHA_VENTA < mañana)
                .SumAsync(v => (decimal?)v.MONTO_TOTAL) ?? 0m;
        }
    }
}
