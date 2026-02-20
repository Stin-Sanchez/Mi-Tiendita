using ENTIDADES;
using ENTIDADES.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Servicios
{
    /// <summary>
    /// Aquí implemento la lógica de negocio central para la entidad CATEGORIAS, asegurándome de que 
    /// los datos sean íntegros antes de tocar la base de datos. Como Líder Técnico, mi objetivo aquí 
    /// es proteger nuestra capa de datos de inconsistencias, datos corruptos y errores de captura.
    /// </summary>
    public class CategoriasServiceImp : ICategoriasService
    {
        // Guardo una referencia inmutable a mi repositorio genérico para centralizar el acceso a datos.
        private readonly ICrudRepository<CATEGORIAS> _repository;

        /// <summary>
        /// Solicito el repositorio por inyección de dependencias. 
        /// Hago esto para no atar esta clase a una implementación específica de base de datos, 
        /// permitiéndonos hacer pruebas unitarias (mocks) fácilmente en el futuro.
        /// </summary>
        public CategoriasServiceImp(ICrudRepository<CATEGORIAS> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Proceso la solicitud de creación de una nueva categoría, aplicando todas nuestras 
        /// reglas de negocio y validaciones de formato antes de permitir su persistencia.
        /// </summary>
        public CATEGORIAS Insertar(CATEGORIAS categoria)
        {
            // Primero verifico si el objeto categoría llegó nulo desde el controlador. 
            // Si es así, detengo la ejecución lanzando una excepción para evitar un NullReferenceException profundo que tumbe el sistema.
            if (categoria == null)
                throw new ArgumentNullException(nameof(categoria), "La categoría proporcionada no puede ser nula.");

            // Valido que el nombre no venga vacío y no exceda los 50 caracteres que soporta nuestra base de datos.
            // Tomo esta decisión aquí porque prefiero capturar el error amigablemente en el código y darle un buen mensaje al usuario, 
            // en lugar de dejar que el motor de SQL explote de forma fea con un error de truncamiento (String or binary data would be truncated).
            if (string.IsNullOrWhiteSpace(categoria.NOMBRE) || categoria.NOMBRE.Length > 50)
                throw new ArgumentException("El nombre de la categoría es obligatorio y no debe superar los 50 caracteres.");

            // Aplico la misma protección estricta para la descripción, respetando el límite de 100 caracteres de nuestro modelo.
            if (string.IsNullOrWhiteSpace(categoria.DESCRIPCION) || categoria.DESCRIPCION.Length > 100)
                throw new ArgumentException("La descripción es obligatoria y no debe superar los 100 caracteres.");

            // Asigno la fecha de creación basándome exclusivamente en el reloj del servidor. 
            // Hago esto para evitar discrepancias si el frontend del cliente tiene la hora o zona horaria mal configurada.
            categoria.FECHA_CREACION = DateTime.Now;

            // Fuerzo a que toda nueva categoría nazca activa por defecto. 
            // Esto agiliza el flujo de trabajo del usuario para que pueda asignarle productos inmediatamente.
            categoria.ACTIVO = true;

            // Una vez que los datos pasaron mis filtros de seguridad, le doy luz verde al repositorio para guardarlos.
            return _repository.Insertar(categoria);
        }

        /// <summary>
        /// Valido y proceso la actualización de una categoría que ya existe en el sistema.
        /// </summary>
        public CATEGORIAS Actualizar(CATEGORIAS categoria)
        {
            // Al igual que en la inserción, me aseguro de que me estén enviando un objeto real.
            if (categoria == null)
                throw new ArgumentNullException(nameof(categoria), "La categoría a actualizar no puede ser nula.");

            // Verifico que el ID tenga sentido lógico. Si es 0 o negativo, significa que es un dato corrupto del frontend 
            // o un intento de actualizar un registro fantasma. Bloqueo la acción de inmediato.
            if (categoria.ID_CATEGORIA <= 0)
                throw new ArgumentException("El ID de la categoría no es válido para realizar una actualización.");

            // Reutilizo mis validaciones de longitud y obligatoriedad. 
            // No asumo que los datos vienen bien solo porque es una actualización; la seguridad siempre va primero.
            if (string.IsNullOrWhiteSpace(categoria.NOMBRE) || categoria.NOMBRE.Length > 50)
                throw new ArgumentException("El nombre de la categoría es obligatorio y no debe superar los 50 caracteres.");

            if (string.IsNullOrWhiteSpace(categoria.DESCRIPCION) || categoria.DESCRIPCION.Length > 100)
                throw new ArgumentException("La descripción es obligatoria y no debe superar los 100 caracteres.");

            // Estando seguro de la integridad de la información, delego el UPDATE a la capa de datos.
            return _repository.Actualizar(categoria);
        }

        /// <summary>
        /// Gestiono la eliminación de una categoría en base a su ID.
        /// </summary>
        public void Eliminar(long id)
        {
            // Evito viajes innecesarios a la base de datos bloqueando IDs que lógicamente son inválidos.
            if (id <= 0)
                throw new ArgumentException("El identificador proporcionado no es válido.");

            // Recupero la categoría actual para verificar si existe antes de mandar la orden de borrarla.
            // Hago esto para poder darle retroalimentación precisa al controlador en caso de que alguien ya la haya borrado segundos antes.
            var categoriaExistente = _repository.ObtenerPorId(id);
            if (categoriaExistente == null)
                throw new InvalidOperationException("La categoría que intentas eliminar no existe en el registro.");

            // Mando la orden de eliminación física al repositorio. 
            // (Ojo equipo: Si nuestra regla de negocio futura dicta que no se pueden borrar categorías con productos ligados, 
            // yo agregaría aquí un chequeo a 'categoriaExistente.PRODUCTOS.Any()' y lanzaría una excepción, o bien, 
            // cambiaría la lógica para hacer un borrado lógico poniendo ACTIVO = false).
            _repository.Eliminar(id);
        }

        /// <summary>
        /// Recupero el detalle completo de una categoría específica.
        /// </summary>
        public CATEGORIAS ObtenerPorId(long id)
        {
            // Valido la entrada para no saturar al motor de SQL con búsquedas de IDs negativos o ceros.
            if (id <= 0)
                throw new ArgumentException("El identificador proporcionado no es válido.");

            return _repository.ObtenerPorId(id);
        }

        /// <summary>
        /// Retorno el catálogo completo de categorías activas e inactivas.
        /// </summary>
        public async Task <IEnumerable<CATEGORIAS>> ObtenerTodasLasCategorias()
        {
            // Solicito la colección completa. 
            // Dejo asentado que si nuestro volumen de categorías crece a miles de registros, 
            // yo sugeriría refactorizar este método para incluir paginación y no saturar la memoria RAM del servidor.
          
            return (await _repository.ObtenerTodos()).Where(c => c.ACTIVO == true);
        }
    }
}

