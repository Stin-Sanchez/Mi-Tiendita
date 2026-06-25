using DAO;
using ENTIDADES.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ENTIDADES.RepositoryImp
{
    public class ProductoRepositoryImp : RepositoryImp<PRODUCTOS>, IProductoRepository
    {
        public ProductoRepositoryImp(ModelContext context) : base(context) { }

        public async Task<int> ObtenerTotalProductosAsync()
        {
            return await _context.PRODUCTOS.CountAsync();
        }

        public async Task<int> ObtenerProductosConStockCriticoAsync(int umbralMinimo = 5)
        {
            return await _context.PRODUCTOS.CountAsync(p => p.STOCK <= umbralMinimo);
        }
    }
}
