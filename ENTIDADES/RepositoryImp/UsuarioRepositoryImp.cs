using DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ENTIDADES.RepositoryImp;

namespace ENTIDADES.Repository
{
    public class UsuarioRepositoryImp : RepositoryImp<USUARIOS>, IUsuarioRepository
    {


        // Le pasamos el context a la clase padre (base)
        public UsuarioRepositoryImp(ModelContext context) : base(context)
        {
        }

        public bool cambiarClave(int idUsuario, string nuevaClave)
        {
            try
            {
                // 1. Buscamos el usuario por su ID
                var usuario = _context.USUARIOS.Find(idUsuario);

                if (usuario != null)
                {
                    // 2. Actualizamos las propiedades
                    usuario.CLAVE = nuevaClave;

                  
                    usuario.RESTABLECER = false;

                    // 3. Guardamos los cambios. SaveChanges() devuelve el número de filas afectadas.
                    int filasAfectadas = _context.SaveChanges();

                    return filasAfectadas > 0;
                }

                return false; // Retorna false si el usuario no existía
            }
            catch (Exception ex)
            {
               
               Console.WriteLine( ex.Message);
                return false;
            }
        }



        public async Task<int> ObtenerTotalUsuariosAsync()
        {
            return await _context.USUARIOS.CountAsync();
        }

        public bool restablecerClave(int idUsuario, string clave)
        {
            try
            {
                // 1. Buscamos el usuario por su ID
                var usuario = _context.USUARIOS.Find(idUsuario);

                if (usuario != null)
                {
                    // 2. Actualizamos las propiedades
                    usuario.CLAVE = clave;

                    // Usamos true (si es bool) o 1 (si es int)
                    usuario.RESTABLECER = true;

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
