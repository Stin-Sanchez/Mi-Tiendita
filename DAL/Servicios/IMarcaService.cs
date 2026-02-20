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
        MARCAS ObtenerPorId(long id);
        MARCAS Insertar(MARCAS marca);
        MARCAS Actualizar(MARCAS marca);
        void Eliminar(long id);
    }
}
