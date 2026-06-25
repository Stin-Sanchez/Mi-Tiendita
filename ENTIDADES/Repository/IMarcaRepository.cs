using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTIDADES.Repository
{
   public  interface IMarcaRepository : ICrudRepository<MARCAS>
    {
        Task<IEnumerable<MARCAS>> ObtenerMarcasPorCategoria(long idCategoria);
    }
}
