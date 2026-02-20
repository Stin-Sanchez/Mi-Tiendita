using ENTIDADES;
using ENTIDADES.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Servicios.ServiciosImp
{
    public class ClienteServiceImp : IClienteService
    {

        // Declaro la interfaz del repositorio para mantener el bajo acoplamiento.
        private readonly IClienteRepository _ClienteRepo;
        public CLIENTES Actualizar(CLIENTES usuario)
        {
            throw new NotImplementedException();
        }

        public bool cambiarClave(int idCliente, string nuevaClave)
        {
            // Primero verifico que no me estén enviando un ID inválido o una contraseña en blanco.
            // Si detecto esto, detengo la ejecución inmediatamente para proteger la integridad de los datos y evitar llamadas innecesarias a la base de datos.
            if (idCliente <= 0 || string.IsNullOrWhiteSpace(nuevaClave))
            {
                return false;
            }

            // Como los datos superaron mis filtros de negocio, delego la responsabilidad de la escritura física al repositorio.
            // Retorno el resultado de esa operación directamente para que mi controlador sepa qué responderle al cliente.
            return _ClienteRepo.cambiarClave(idCliente, nuevaClave);
        }

        public void Eliminar(long id)
        {
            throw new NotImplementedException();
        }

        public  async Task<CLIENTES> Insertar(CLIENTES cliente)
        {
            // Si detecto que falta información crucial, lanzo una excepción enseguida para no desperdiciar
            // tiempo de procesamiento ni recursos intentando guardar un registro inválido.
            if (string.IsNullOrWhiteSpace(cliente.NOMBRE))
                throw new Exception("El nombre del cliente no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(cliente.APELLIDO))
                throw new Exception("El apellido del cliente no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(cliente.CORREO))
                throw new Exception("El correo del cliente no puede estar vacío.");



            // Como superamos todas las barreras y el correo se envió con éxito, protejo la credencial.
            // Nunca guardo contraseñas en texto plano; la encripto para proteger la privacidad del usuario.
            cliente.CLAVE = UtilService.EncriptarClave(cliente.CLAVE);

            // Finalmente, le paso el objeto validado y seguro al repositorio para que lo inserte en SQL.
            return  _ClienteRepo.Insertar(cliente);
        }

        public CLIENTES ObtenerPorId(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CLIENTES>> ObtenerTodos()
        {
            return (await _ClienteRepo.ObtenerTodos());
        }

        public Task<int> ObtenerTotalUsuarios()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> restablecerClave(int idUsuario, string correo, string correoEmisor, string claveEmisor)
        {
            // Primero, valido que el usuario realmente exista en mi base de datos antes de intentar cualquier operación.
            // Necesito sus datos (Nombre, Apellido) para personalizar el correo, así que hago la consulta ahora.
            var usuario = _ClienteRepo.ObtenerPorId(idUsuario);

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
            bool cambioExitoso = _ClienteRepo.restablecerClave(idUsuario, claveEncriptada);

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
    

