using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTIDADES.Repository
{
    public interface ICarritoRepository :  ICrudRepository<CARRITO>
    {
        Task<bool> existeCarrito(int idCliente , int idProducto);
        Task<(bool resultado, string mensaje)> operacionCarrito(int idCliente, int idProducto, bool sumar);
    }
}
