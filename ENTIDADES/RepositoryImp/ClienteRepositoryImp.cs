using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAO;
using ENTIDADES.Repository;
using ENTIDADES.RepositoryImp;


namespace ENTIDADES.RepositoryImp
{
    public class ClienteRepositoryImp : RepositoryImp<CLIENTES>, IClienteRepository
    {
        // Le pasamos el context a la clase padre (base)
        public ClienteRepositoryImp(ModelContext context) : base(context)
        {
        }

        public bool cambiarClave(int idCliente, string nuevaClave)
        {
            try
            {
                // 1. Buscamos el cliente por su ID
                var cliente = _context.USUARIOS.Find(idCliente);

                if (cliente != null)
                {
                    // 2. Actualizamos las propiedades
                    cliente.CLAVE = nuevaClave;


                    cliente.RESTABLECER = false;

                    // 3. Guardamos los cambios. SaveChanges() devuelve el número de filas afectadas.
                    int filasAfectadas = _context.SaveChanges();

                    return filasAfectadas > 0;
                }

                return false; // Retorna false si el usuario no existía
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool restablecerClave(int idCliente, string clave)
        {
            try
            {
                // 1. Buscamos el cliente por su ID
                var cliente = _context.USUARIOS.Find(idCliente);

                if (cliente != null)
                {
                    // 2. Actualizamos las propiedades
                    cliente.CLAVE = clave;

                    // Usamos true (si es bool) o 1 (si es int)
                    cliente.RESTABLECER = true;

                    // 3. Guardamos los cambios
                    int filasAfectadas = _context.SaveChanges();

                    return filasAfectadas > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
