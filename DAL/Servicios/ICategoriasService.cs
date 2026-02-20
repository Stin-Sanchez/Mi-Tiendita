using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTIDADES;

namespace DAL.Servicios
{
    public interface ICategoriasService
    {
        Task<IEnumerable<CATEGORIAS>> ObtenerTodasLasCategorias();
        CATEGORIAS ObtenerPorId(long id);
        CATEGORIAS Insertar(CATEGORIAS categoria);
        CATEGORIAS Actualizar(CATEGORIAS categoria);
        void Eliminar(long id);
    }
}
