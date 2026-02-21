using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTIDADES;

namespace DAL.Servicios
{
    public interface IProductoService
    {
        Task<IEnumerable<PRODUCTOS>> ObtenerTodos();
        Task <PRODUCTOS> ObtenerPorId(long id);
        Task<PRODUCTOS> Insertar(PRODUCTOS producto);
        Task<PRODUCTOS> Actualizar(PRODUCTOS producto);
        Task Eliminar(long id);

        Task<int> ObtenerTotalProductos();
        Task<int> ObtenerProductosConStockCritico(int umbralMinimo = 5);
    }
}
