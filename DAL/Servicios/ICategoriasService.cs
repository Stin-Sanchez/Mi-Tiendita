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
        Task<CATEGORIAS> ObtenerPorId(long id);
        Task<CATEGORIAS> Insertar(CATEGORIAS categoria);
        Task<CATEGORIAS> Actualizar(CATEGORIAS categoria);
        Task Eliminar(long id);
    }
}
