using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTIDADES.Repository
{
    public interface IProductoRepository : ICrudRepository<PRODUCTOS>
    {
        Task<int> ObtenerTotalProductosAsync();
        Task<int> ObtenerProductosConStockCriticoAsync(int umbralMinimo = 5);
    }
}
