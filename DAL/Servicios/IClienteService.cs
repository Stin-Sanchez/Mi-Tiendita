using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTIDADES;

namespace DAL.Servicios
{
   public  interface IClienteService
    {
        Task<IEnumerable<CLIENTES>> ObtenerTodos();
        Task <CLIENTES> ObtenerPorId(long id);
        Task<CLIENTES> Insertar(CLIENTES usuario);
        Task <CLIENTES> Actualizar(CLIENTES usuario);
        Task Eliminar(long id);

        Task<int> ObtenerTotalUsuarios();

        bool cambiarClave(int idUsuario, string nuevaClave);
        Task<bool> restablecerClave(int idUsuario, string correo, string correoEmisor, string claveEmisor);
    }
}
