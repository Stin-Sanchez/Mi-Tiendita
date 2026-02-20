using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTIDADES.Repository
{
   public  interface IUsuarioRepository : ICrudRepository<USUARIOS>
    {
        Task<int> ObtenerTotalUsuariosAsync();
        bool cambiarClave(int idUsuario, string nuevaClave);
        bool restablecerClave(int idUsuario, string clave);
    }
}

