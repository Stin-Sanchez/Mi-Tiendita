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
        USUARIOS ObtenerPorId(USUARIOS id);
        void Insertar(USUARIOS usuario);
        void Actualizar(USUARIOS usuario);
        void Eliminar(long id);
        void Guardar();
    }
}
