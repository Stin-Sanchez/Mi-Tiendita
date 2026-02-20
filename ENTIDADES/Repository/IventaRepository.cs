using DAO.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTIDADES.Repository
{
    public interface IVentaRepository : ICrudRepository<VENTAS>
    {
        Task<decimal> ObtenerSumaVentasDelDiaAsync();
          Task<List<HistorialVentasDTO>> ObtenerHistorialVentasAsync(string fechaInicio, string fechaFin, string idTransaccion);
    }
}
