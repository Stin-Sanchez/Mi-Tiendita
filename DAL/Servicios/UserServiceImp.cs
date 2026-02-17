using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTIDADES;
using ENTIDADES.Repository;

namespace DAL.Servicios
{
    /// <summary>
    /// Aquí implemento la lógica de negocio para la gestión de usuarios.
    /// Actúo como la capa intermedia entre el controlador (lo que pide el cliente) y el repositorio (la base de datos),
    /// asegurándome de que todas las reglas de negocio se cumplan antes de guardar cualquier dato.
    /// </summary>
    public class UserServiceImp : IUserService
    {
        // Declaro la interfaz del repositorio para mantener el bajo acoplamiento.
        private readonly ICrudRepository<USUARIOS> _UsuarioRepo;

        /// <summary>
        /// Construyo el servicio inyectando sus dependencias.
        /// </summary>
        // Utilizo inyección de dependencias a través del constructor. 
        // De esta forma, no obligo a esta clase a saber cómo construir el repositorio, 
        // lo que me facilitará muchísimo hacer pruebas unitarias (testing) en el futuro.
        public UserServiceImp(ICrudRepository<USUARIOS> userRepo)
        {
            _UsuarioRepo = userRepo;
        }

        /// <summary>
        /// Tomo un usuario existente con datos modificados y consolido los cambios en el sistema.
        /// </summary>
        public USUARIOS Actualizar(USUARIOS usuario)
        {
            // Por ahora, delego directamente la actualización al repositorio. 
            // Si en el futuro necesitamos validar que un usuario no cambie su correo a uno ya existente, 
            // este es el lugar exacto donde pondré esa regla de negocio antes de llamar al repositorio.
            return _UsuarioRepo.Actualizar(usuario);
        }

        /// <summary>
        /// Realizo un "borrado lógico" (soft delete) del usuario para inhabilitar su acceso.
        /// </summary>
        public void Eliminar(long id)
        {
            // Primero, consulto la base de datos para asegurarme de que el usuario que me piden eliminar realmente existe.
            USUARIOS usuario = _UsuarioRepo.ObtenerPorId(id);

            // Si no encuentro al usuario, detengo la ejecución lanzando una excepción inmediatamente.
            // Hago esto para que el bloque 'catch' del controlador atrape el error y le muestre 
            // un mensaje amigable al usuario final, en lugar de que la aplicación explote silenciosamente.
            if (usuario == null)
            {
                throw new Exception("El usuario que intenta desactivar no existe.");
            }

            // En lugar de hacer un DELETE en SQL, aplico un borrado lógico.
            // Cambio su estado a inactivo para mantener la integridad referencial y el historial en la base de datos.
            usuario.ACTIVO = false;

            // Finalmente, reutilizo mi método de actualización para guardar este cambio de estado.
            _UsuarioRepo.Actualizar(usuario);
        }

        /// <summary>
        /// Valido los datos de un nuevo usuario, le genero credenciales, le notifico por correo
        /// y finalmente lo registro de manera segura.
        /// </summary>
        public USUARIOS Insertar(USUARIOS usuario)
        {
            // Aplico el patrón "Fail-Fast" (fallar rápido). Valido los campos obligatorios uno por uno.
            // Si detecto que falta información crucial, lanzo una excepción enseguida para no desperdiciar
            // tiempo de procesamiento ni recursos intentando guardar un registro inválido.
            if (string.IsNullOrWhiteSpace(usuario.NOMBRE))
                throw new Exception("El nombre del usuario no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(usuario.APELLIDO))
                throw new Exception("El apellido del usuario no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(usuario.CORREO))
                throw new Exception("El correo del usuario no puede estar vacío.");

            // Como el usuario no elige su propia clave al registrarse en este flujo, 
            // le genero una contraseña segura de manera automática.
            string clave = UtilService.GenerarClaveUnica();
            string asunto = "Creacion de cuenta en Sistema Mi Tiendita";

            // Preparo el cuerpo del correo utilizando interpolación de strings ($"") 
            // para inyectar la clave generada directamente en el texto HTML de forma más legible.
            string mensaje_correo = $"<h3>Su cuenta fue creada exitosamente</h3><br/><p>Su contraseña para acceder al sistema es: <b>{clave}</b></p>";

            // Intento enviar el correo electrónico ANTES de guardar al usuario en la base de datos.
            bool correoEnviado = UtilService.EnviarCorreo(usuario.CORREO, asunto, mensaje_correo);

            // Hago esta validación porque si el servicio de correo falla (ej: sin internet o servidor de correo caído), 
            // prefiero detener todo. Si lo guardara primero, tendríamos un "usuario fantasma" 
            // en la base de datos que nunca recibió su clave y no podría iniciar sesión.
            if (!correoEnviado)
            {
                throw new Exception("No se pudo enviar el correo electrónico. El usuario no fue registrado.");
            }

            // Como superamos todas las barreras y el correo se envió con éxito, protejo la credencial.
            // Nunca guardo contraseñas en texto plano; la encripto para proteger la privacidad del usuario.
            usuario.CLAVE = UtilService.EncriptarClave(clave);

            // Finalmente, le paso el objeto validado y seguro al repositorio para que lo inserte en SQL.
            return _UsuarioRepo.Insertar(usuario);
        }

        /// <summary>
        /// Busco los datos completos de un usuario específico.
        /// </summary>
        public USUARIOS ObtenerPorId(USUARIOS id)
        {
            // Delego la operación de lectura al repositorio.
            return _UsuarioRepo.ObtenerPorId(id);
        }

        /// <summary>
        /// Recupero el listado de todos los usuarios registrados en el sistema.
        /// </summary>
        public IEnumerable<USUARIOS> ObtenerTodos()
        {
            // Delego la lectura masiva al repositorio.
            return _UsuarioRepo.ObtenerTodos();
        }
    }
}


