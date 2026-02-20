using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTIDADES.Repository
{
    public interface IClienteRepository : ICrudRepository<CLIENTES>
    {
        bool cambiarClave(int idUsuario, string nuevaClave);
        bool restablecerClave(int idUsuario, string clave);
    }
}
