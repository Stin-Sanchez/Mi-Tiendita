using ENTIDADES.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAO;
using System.Data.Entity;

namespace ENTIDADES.RepositoryImp
{
    public class RepositoryImp<T> : ICrudRepository<T> where T : class
    {
        protected ModelContext _context = null;
        private DbSet<T> table = null;

        // Constructor
        public RepositoryImp(ModelContext context)
        {
            this._context = context;
            table = _context.Set<T>(); // Convierte la T en la tabla correspondiente
        }

        public async Task<IEnumerable<T>> ObtenerTodos()
        {
            return await table.ToListAsync();
        }

        public async Task <T> ObtenerPorIdAsync(object id)
        {
             return await table.FindAsync(id);

        }

        public async Task <T> InsertarAsync(T obj)
        {
            table.Add(obj);
          await  _context.SaveChangesAsync();

            return obj; 
        }

        public async Task <T> ActualizarAsync(T obj)
        {
           table.Attach(obj);
          _context.Entry(obj).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return obj;
        }

        public async Task EliminarAsync(object id)
        {
            T existing = await table.FindAsync(id);
            if(existing != null)
            {
                table.Remove(existing);
               await _context.SaveChangesAsync();
            }
           
        }

      
    }
}
