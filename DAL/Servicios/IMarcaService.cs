using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTIDADES;

namespace DAL.Servicios
{
    public interface IMarcaService
    {
        Task<IEnumerable<MARCAS>> ObtenerTodasLasMarcas();
        
        Task<IEnumerable<MARCAS>> ObtenerMarcasPorCategoria(long idCategoria);

        Task<MARCAS> ObtenerPorId(long id);
        Task<MARCAS> Insertar(MARCAS marca);
        Task<MARCAS> Actualizar(MARCAS marca);
        Task Eliminar(long id);
    }
}
