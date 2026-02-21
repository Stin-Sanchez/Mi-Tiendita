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
        Task<IEnumerable<USUARIOS>> ObtenerTodos();
        Task<USUARIOS> ObtenerPorId(long id);
      
        Task<USUARIOS> Insertar(USUARIOS usuario, string correoEmisor, string claveEmisor);
        Task<USUARIOS> Actualizar(USUARIOS usuario);
        Task Eliminar(long id);

        Task<int> ObtenerTotalUsuarios();

        bool cambiarClave(int idUsuario, string nuevaClave);
        Task<bool> restablecerClave(int idUsuario, string correo, string correoEmisor, string claveEmisor);

    }
}
