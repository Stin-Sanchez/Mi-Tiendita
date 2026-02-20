using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAO.DTOS;
using ENTIDADES;

namespace DAL.Servicios
{
    public interface IVentaService
    {
        IEnumerable<VENTAS> ObtenerTodos();
        VENTAS ObtenerPorId(long id);
        VENTAS Insertar(VENTAS venta);
        VENTAS Actualizar(VENTAS venta);
        void Eliminar(long id);
        Task<decimal> ObtenerSumaVentasDelDia();

         Task<List<HistorialVentasDTO>> ObtenerHistorialVentasAsync(string fechaInicio, string fechaFin, string idTransaccion);
    }
}
