using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTIDADES.Repository
{
    public interface ICrudRepository <T> where T: class

    {
        Task<IEnumerable<T>> ObtenerTodos();
        Task<T> ObtenerPorIdAsync(object id);
        Task<T> InsertarAsync(T obj);
        Task<T> ActualizarAsync(T obj);
        Task EliminarAsync(object id);


    }
}
