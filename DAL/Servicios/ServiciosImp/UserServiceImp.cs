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
        private readonly IUsuarioRepository  _UsuarioRepo;

        /// <summary>
        /// Construyo el servicio inyectando sus dependencias.
        /// </summary>
        // Utilizo inyección de dependencias a través del constructor. 
        // De esta forma, no obligo a esta clase a saber cómo construir el repositorio, 
        // lo que me facilitará muchísimo hacer pruebas unitarias (testing) en el futuro.
        public UserServiceImp(IUsuarioRepository userRepo)
        {
            _UsuarioRepo = userRepo;
        }

        /// <summary>
        /// Tomo un usuario existente con datos modificados y consolido los cambios en el sistema.
        /// </summary>
        public async Task<USUARIOS> Actualizar(USUARIOS usuario)
        {
            // Por ahora, delego directamente la actualización al repositorio. 
            // Si en el futuro necesitamos validar que un usuario no cambie su correo a uno ya existente, 
            // este es el lugar exacto donde pondré esa regla de negocio antes de llamar al repositorio.
            return await _UsuarioRepo.ActualizarAsync(usuario);
        }

        /// <summary>
        /// Aquí gestiono la petición de un usuario que desea actualizar su propia contraseña por motivos de seguridad o preferencia.
        /// Mi responsabilidad en esta capa es garantizar que los datos proporcionados sean íntegros antes de procesarlos.
        /// </summary>
        /// <param name="idUsuario">El identificador único del usuario que ejecutará la acción.</param>
        /// <param name="nuevaClave">La nueva credencial de acceso elegida.</param>
        /// <returns>Retorno verdadero si todo el flujo de validación y actualización fue exitoso.</returns>
        public bool cambiarClave(int idUsuario, string nuevaClave)
        {
            // Primero verifico que no me estén enviando un ID inválido o una contraseña en blanco.
            // Si detecto esto, detengo la ejecución inmediatamente para proteger la integridad de los datos y evitar llamadas innecesarias a la base de datos.
            if (idUsuario <= 0 || string.IsNullOrWhiteSpace(nuevaClave))
            {
                return false;
            }

            // Como los datos superaron mis filtros de negocio, delego la responsabilidad de la escritura física al repositorio.
            // Retorno el resultado de esa operación directamente para que mi controlador sepa qué responderle al cliente.
            return _UsuarioRepo.cambiarClave(idUsuario, nuevaClave);
        }

     

        /// <summary>
        /// Realizo un "borrado lógico" (soft delete) del usuario para inhabilitar su acceso.
        /// </summary>
        public async Task Eliminar(long id)
        {
            // Primero, consulto la base de datos para asegurarme de que el usuario que me piden eliminar realmente existe.
            USUARIOS usuario = await _UsuarioRepo.ObtenerPorIdAsync(id);

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
            await _UsuarioRepo.ActualizarAsync(usuario);
        }

        /// <summary>
        /// Valido los datos de un nuevo usuario, le genero credenciales, le notifico por correo
        /// y finalmente lo registro de manera segura.
        /// </summary>
        public  async Task<USUARIOS> Insertar(USUARIOS usuario, string correoEmisor, string claveEmisor)
        {
          
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
            // Construyo una plantilla HTML con estilos en línea (CSS inline) para garantizar 
            // que se vea igual de profesional en Gmail, Outlook o cualquier otro cliente.
            string mensaje_correo = $@"
                         <div style='font-family: Arial, Helvetica, sans-serif; color: #333333; padding: 20px; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; box-shadow: 0 4px 8px rgba(0,0,0,0.05);'>
        
                        <h2 style='color: #004085; text-align: center; border-bottom: 2px solid #004085; padding-bottom: 10px;'>
                            ¡Bienvenido a nuestro sistema!
                        </h2>
        
                        <p>Hola <b>{usuario.NOMBRE} {usuario.APELLIDO}</b>,</p>
        
                        <p>Nos complace informarte que tu cuenta ha sido configurada exitosamente. A continuación, te compartimos tus credenciales de acceso:</p>
        
                        <div style='background-color: #f8f9fa; padding: 15px; border-left: 5px solid #28a745; margin: 20px 0; border-radius: 4px;'>
                            <p style='margin: 5px 0;'><b>Usuario / Correo:</b> {usuario.CORREO}</p>
                            <p style='margin: 5px 0;'><b>Contraseña temporal:</b> <span style='color: #d9534f; font-size: 16px; letter-spacing: 1px;'><b>{clave}</b></span></p>
                        </div>
        
                        <p style='font-size: 13px; color: #856404; background-color: #fff3cd; padding: 10px; border-left: 4px solid #ffeeba;'>
                            <i>⚠️ <b>Importante:</b> Por tu seguridad, te sugerimos cambiar esta contraseña temporal inmediatamente después de tu primer inicio de sesión.</i>
                        </p>
        
                        <hr style='border: 0; border-top: 1px solid #eeeeee; margin: 30px 0 20px 0;' />
        
                        <p style='font-size: 12px; color: #999999; text-align: center; margin: 0;'>
                            Este es un mensaje generado automáticamente. Por favor, no respondas a este correo.<br/>
                            Atentamente,<br/>
                            <b>El Equipo de Soporte</b>
                        </p>
        
                    </div>";

            // Intento enviar el correo electrónico ANTES de guardar al usuario en la base de datos.
            bool correoEnviado = await UtilService.EnviarCorreo(usuario.CORREO, asunto, mensaje_correo,correoEmisor,claveEmisor);

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
            return await _UsuarioRepo.InsertarAsync(usuario);
        }

        /// <summary>
        /// Busco los datos completos de un usuario específico.
        /// </summary>
        public async Task<USUARIOS> ObtenerPorId(long id)
        {
            // Delego la operación de lectura al repositorio.
            return await  _UsuarioRepo.ObtenerPorIdAsync(id);
        }

        /// <summary>
        /// Recupero el listado de todos los usuarios registrados en el sistema.
        /// </summary>
        public async Task<IEnumerable<USUARIOS>> ObtenerTodos()
        {
            // Delego la lectura masiva al repositorio.
            return (await _UsuarioRepo.ObtenerTodos()).Where(u => u.ACTIVO == true);
        }

        /// <summary>
        /// Actúo como intermediario para facilitar el dato del conteo total de usuarios registrados en la plataforma.
        /// Mi objetivo es proveer esta métrica clave, que será consumida por capas superiores (como el Dashboard) para mostrar el estado actual del sistema.
        /// </summary>
        /// <returns>Una tarea que, al completarse, entregará el número entero de usuarios.</returns>
        public async Task<int> ObtenerTotalUsuarios()
        {
            // En este punto, elijo delegar la consulta directamente al repositorio y retorno la 'Task' tal cual hacia quien me llamó.
            // Tomo la decisión técnica de NO usar 'await' aquí para evitar la sobrecarga de crear una máquina de estados asíncrona innecesaria en esta capa,
            // ya que simplemente estoy pasando el paquete del repositorio al controlador sin necesidad de abrirlo o modificarlo.
            return await  _UsuarioRepo.ObtenerTotalUsuariosAsync();
        }

        /// <summary>
        /// Me encargo de manejar el flujo cuando un administrador o el sistema requiere forzar el reseteo de la contraseña de una cuenta, 
        /// generalmente porque el usuario la olvidó o por políticas de expiración.
        /// </summary>
        /// <param name="idUsuario">El identificador único de la cuenta comprometida o a resetear.</param>
        /// <param name="clave">La contraseña temporal o generada por el sistema que se le asignará.</param>
        /// <returns>Retorno verdadero si el reseteo impactó correctamente en el sistema.</returns>
        public async Task<bool> restablecerClave(int idUsuario, string correo, string correoEmisor, string claveEmisor)
        {
            // Primero, valido que el usuario realmente exista en mi base de datos antes de intentar cualquier operación.
            // Necesito sus datos (Nombre, Apellido) para personalizar el correo, así que hago la consulta ahora.
            var usuario = await _UsuarioRepo.ObtenerPorIdAsync(idUsuario);

            if (usuario == null)
            {
                return false;
            }

            // Genero una nueva contraseña aleatoria y procedo a encriptarla inmediatamente.
            // Nunca debo manejar contraseñas en texto plano más allá de lo estrictamente necesario para el correo.
            string nuevaClave = UtilService.GenerarClaveUnica();
            string claveEncriptada = UtilService.EncriptarClave(nuevaClave);

            // Procedo a actualizar la contraseña en la base de datos.
            // Decido hacer esto PRIMERO porque es la operación crítica de persistencia. 
            // Si la base de datos falla, no tiene sentido enviar un correo con una clave que no funcionará.
            bool cambioExitoso = _UsuarioRepo.restablecerClave(idUsuario, claveEncriptada);

            if (cambioExitoso)
            {
                // Preparo el asunto y construyo la plantilla HTML.
                // Uso interpolación ($@"") para inyectar los datos del usuario y la clave temporal (sin encriptar) de forma legible.
                string asunto = "Sistema - Contraseña Restablecida";
                string mensaje_correo = $@"
                <div style='font-family: Arial, Helvetica, sans-serif; color: #333333; padding: 20px; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px;'>
                    <h2 style='color: #004085; text-align: center; border-bottom: 2px solid #004085; padding-bottom: 10px;'>
                        ¡Credenciales Actualizadas!
                    </h2>
                    <p>Hola <b>{usuario.NOMBRE} {usuario.APELLIDO}</b>,</p>
                    <p>Tu solicitud de restablecimiento ha sido procesada. Aquí tienes tus nuevas credenciales temporales:</p>
                    
                    <div style='background-color: #f8f9fa; padding: 15px; border-left: 5px solid #28a745; margin: 20px 0;'>
                        <p><b>Contraseña temporal:</b> <span style='color: #d9534f; font-size: 18px;'><b>{nuevaClave}</b></span></p>
                    </div>

                    <p style='font-size: 13px; background-color: #fff3cd; padding: 10px;'>
                        <i>⚠️ Importante: Por seguridad, cambia esta contraseña inmediatamente al ingresar.</i>
                    </p>
                </div>";

                // Intento enviar el correo a la dirección registrada en el objeto usuario (es más seguro que usar el parámetro 'correo' que viene de afuera).
                bool correoEnviado = await UtilService.EnviarCorreo(usuario.CORREO, asunto, mensaje_correo, correoEmisor, claveEmisor);

                // Si el correo falla, tomo la decisión drástica de lanzar una excepción (o podrías revertir el cambio en BD).
                // Esto es necesario porque si no aviso, el usuario tendrá una clave nueva en BD que desconoce y quedará bloqueado.
                if (!correoEnviado)
                {
                    throw new Exception("La contraseña se cambió, pero falló el envío del correo. Contacte a soporte.");
                }

                return true;
            }

            // Si llegué aquí, falló la actualización en base de datos.
            return false;
        }
    }
}


