using DAO;
using ENTIDADES.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTIDADES.RepositoryImp
{
    public class MarcaRepositoryImp : RepositoryImp<MARCAS>, IMarcaRepository
    {
        // Le pasamos el context a la clase padre (base)
        public MarcaRepositoryImp(ModelContext context) : base(context)
        {
        }

        public async  Task<IEnumerable<MARCAS>> ObtenerMarcasPorCategoria(long idCategoria)
        {
            var marcas = (from p in _context.PRODUCTOS
                          join c in _context.CATEGORIAS on p.ID_CATEGORIA equals c.ID_CATEGORIA
                          join m in _context.MARCAS on p.ID_MARCA equals m.ID_MARCA
                          where m.ACTIVO == true &&
                                (idCategoria == 0 || c.ID_CATEGORIA == idCategoria)
                          select m).Distinct();

            return await marcas.ToListAsync();
        }
    }
}
