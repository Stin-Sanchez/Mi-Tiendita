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
        T ObtenerPorId(object id);
        T Insertar(T obj);
        T Actualizar(T obj);
        void Eliminar(object id);
     

    }
}
