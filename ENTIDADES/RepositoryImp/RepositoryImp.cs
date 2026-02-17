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
        private ModelContext _context = null;
        private DbSet<T> table = null;

        // Constructor
        public RepositoryImp(ModelContext context)
        {
            this._context = context;
            table = _context.Set<T>(); // Convierte la T en la tabla correspondiente
        }

        public IEnumerable<T> ObtenerTodos()
        {
            return table.ToList();
        }

        public T ObtenerPorId(object id)
        {
            return table.Find(id);
        }

        public T Insertar(T obj)
        {
            table.Add(obj);
            _context.SaveChanges();

            return obj; 
        }

        public T Actualizar(T obj)
        {
           table.Attach(obj);
           _context.Entry(obj).State = EntityState.Modified;
            _context.SaveChanges();

            return obj;
        }

        public void Eliminar(object id)
        {
            T existing = table.Find(id);
            if(existing != null)
            {
                table.Remove(existing);
                _context.SaveChanges();
            }
           
        }

      
    }
}
