using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTIDADES;
using ENTIDADES.Repository;

namespace DAL.Servicios
{
    public class UserServiceImp : IUserService
    {
        // 1. Declaramos la interfaz especificando la entidad PRODUCTOS
        private readonly ICrudRepository<USUARIOS> _UsuarioRepo;

        // 2. Inyección de Dependencias a través del constructor
        public UserServiceImp(ICrudRepository<USUARIOS> userRepo)
        {
            _UsuarioRepo = userRepo;
        }

        public void Actualizar(USUARIOS usuario)
        {
            _UsuarioRepo.Actualizar(usuario);
        }

        public void Eliminar(long id)
        {
            USUARIOS usuario = _UsuarioRepo.ObtenerPorId(id);

            if(usuario.ID_USUARIO != id)
            {
                Console.WriteLine("Usuario no encontrado");
            }

            _UsuarioRepo.Eliminar(id);
        }

        public void Guardar()
        {
            _UsuarioRepo.Guardar();
        }

        public void Insertar(USUARIOS usuario)
        {
            //  Validamos antes de tocar la base de datos
            if (usuario == null)
            {
                throw new ArgumentNullException("El user no puede ser vacio or null.");
            }



            // Si pasa las reglas, usamos el repositorio genérico para guardar
            _UsuarioRepo.Insertar(usuario);
            _UsuarioRepo.Guardar();
        }

        public USUARIOS ObtenerPorId(USUARIOS id)
        {
            return _UsuarioRepo.ObtenerPorId(id);
        }

        public IEnumerable<USUARIOS> ObtenerTodos()
        {
           return  _UsuarioRepo.ObtenerTodos();
        }
    }
}

