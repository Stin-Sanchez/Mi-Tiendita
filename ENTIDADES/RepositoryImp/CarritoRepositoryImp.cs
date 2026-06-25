using DAO;
using ENTIDADES.Repository;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ENTIDADES.RepositoryImp
{
    public class CarritoRepositoryImp : RepositoryImp<CARRITO>, ICarritoRepository
    {
        public CarritoRepositoryImp(ModelContext context) : base(context) { }

        public async Task<bool> existeCarrito(int idCliente, int idProducto)
        {
            var pIdCliente  = new SqlParameter("@idCliente",  idCliente);
            var pIdProducto = new SqlParameter("@idProducto", idProducto);
            var pResultado  = new SqlParameter("@Resultado",  SqlDbType.Bit) { Direction = ParameterDirection.Output };

            await _context.Database.ExecuteSqlCommandAsync(
                "EXEC spExisteCarrito @idCliente, @idProducto, @Resultado OUTPUT",
                pIdCliente, pIdProducto, pResultado
            );

            return (bool)pResultado.Value;
        }

        public async Task<(bool resultado, string mensaje)> operacionCarrito(int idCliente, int idProducto, bool sumar)
        {
            var pIdCliente  = new SqlParameter("@idCliente",  idCliente);
            var pIdProducto = new SqlParameter("@idProducto", idProducto);
            var pSumar      = new SqlParameter("@Sumar",      sumar);
            var pMensaje    = new SqlParameter("@Mensaje",    SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            var pResultado  = new SqlParameter("@Resultado",  SqlDbType.Bit)          { Direction = ParameterDirection.Output };

            await _context.Database.ExecuteSqlCommandAsync(
                "EXEC spCarrito @idCliente, @idProducto, @Sumar, @Mensaje OUTPUT, @Resultado OUTPUT",
                pIdCliente, pIdProducto, pSumar, pMensaje, pResultado
            );

            return ((bool)pResultado.Value, pMensaje.Value?.ToString() ?? string.Empty);
        }
    }
}
