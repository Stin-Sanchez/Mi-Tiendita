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
        PRODUCTOS ObtenerPorId(long id);
        PRODUCTOS Insertar(PRODUCTOS producto);
        PRODUCTOS Actualizar(PRODUCTOS producto);
        void Eliminar(long id);

        Task<int> ObtenerTotalProductos();
        Task<int> ObtenerProductosConStockCritico(int umbralMinimo = 5);
    }
}
