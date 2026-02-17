using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTIDADES;

namespace DAL.Servicios
{
    public interface IUserService
    {
        IEnumerable<USUARIOS> ObtenerTodos();
        USUARIOS ObtenerPorId(long id);
        USUARIOS Insertar(USUARIOS usuario);
        USUARIOS Actualizar(USUARIOS usuario);
        void Eliminar(long id);
      
    }
}
