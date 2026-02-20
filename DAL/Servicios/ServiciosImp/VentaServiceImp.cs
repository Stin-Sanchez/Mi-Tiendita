using ENTIDADES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTIDADES;
using ENTIDADES.Repository;
using DAO.DTOS;

namespace DAL.Servicios
{
    public class VentaServiceImp : IVentaService
    {
        // Declaro la interfaz del repositorio para mantener el bajo acoplamiento.
        private readonly IVentaRepository _VentaRepo;

        public VentaServiceImp(IVentaRepository ventaRepo)
        {
            _VentaRepo = ventaRepo;
        }

        public VENTAS Actualizar(VENTAS venta)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(long id)
        {
            throw new NotImplementedException();
        }

        public VENTAS Insertar(VENTAS venta)
        {
            throw new NotImplementedException();
        }

        public async Task<List<HistorialVentasDTO>> ObtenerHistorialVentasAsync(string fechaInicio, string fechaFin, string idTransaccion)
        {
            return await _VentaRepo.ObtenerHistorialVentasAsync(fechaInicio, fechaFin, idTransaccion);
        } 

        public VENTAS ObtenerPorId(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<decimal> ObtenerSumaVentasDelDia()
        {
            return await _VentaRepo.ObtenerSumaVentasDelDiaAsync();
        }

        public IEnumerable<VENTAS> ObtenerTodos()
        {
            throw new NotImplementedException();
        }
    }
}
