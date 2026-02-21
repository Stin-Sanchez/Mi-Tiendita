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
    /// Aquí centralizo todas las reglas de negocio relacionadas con las Marcas del sistema.
    /// Actúo como el filtro de seguridad entre el controlador y la base de datos, asegurándome 
    /// de que ninguna marca inválida, sin nombre o con textos muy largos llegue a guardarse.
    /// </summary>
    public class MarcaServiceImp : IMarcaService
    {
        // Declaro la dependencia hacia nuestro repositorio genérico para no atarme a Entity Framework directamente.
        private readonly IMarcaRepository _MarcaRepo;

        public MarcaServiceImp(IMarcaRepository marcaRepo)
        {
            _MarcaRepo = marcaRepo;
        }

        /// <summary>
        /// Inyecto el repositorio a través del constructor. 
        /// Hago esto para mantener un bajo acoplamiento y poder enviar un repositorio falso (mock) 
        /// el día de mañana si necesitamos hacer pruebas unitarias.
        /// </summary>


        /// <summary>
        /// Valido estrictamente los datos antes de permitir que una nueva marca nazca en el sistema.
        /// </summary>
        public async Task<MARCAS> Insertar(MARCAS marca)
        {
            // Aplico el patrón "Fail-Fast": verifico inmediatamente si los datos requeridos están ausentes.
            if (string.IsNullOrWhiteSpace(marca.NOMBRE))
                throw new Exception("El nombre de la marca es obligatorio.");

            // Protejo la base de datos contra errores de truncamiento (String or binary data would be truncated).
            // La entidad dice [StringLength(50)], así que me aseguro de que el código lo respete.
            if (marca.NOMBRE.Length > 50)
                throw new Exception("El nombre de la marca no puede superar los 50 caracteres.");

            // Repito mi malla de seguridad para la descripción, respetando su límite de 100 caracteres.
            if (string.IsNullOrWhiteSpace(marca.DESCRIPCION))
                throw new Exception("La descripción de la marca es obligatoria.");

            if (marca.DESCRIPCION.Length > 100)
                throw new Exception("La descripción no puede superar los 100 caracteres.");

            // Nota mental de diseño: No necesito asignar 'marca.ACTIVO = true' ni 'marca.FECHA_CREACION = DateTime.Now' 
            // aquí, porque el constructor de la entidad MARCAS que creaste ya se encarga de eso automáticamente.

            // Habiendo superado las barreras de seguridad, procedo a insertar.
            return await _MarcaRepo.InsertarAsync(marca);
        }

        /// <summary>
        /// Reviso que los cambios propuestos para una marca sigan cumpliendo con nuestras reglas de negocio.
        /// </summary>
        public async Task<MARCAS> Actualizar(MARCAS marca)
        {
            // Reutilizo las reglas lógicas básicas porque un nombre en blanco es igual de inválido 
            // al crear que al editar.
            if (string.IsNullOrWhiteSpace(marca.NOMBRE))
                throw new Exception("El nombre de la marca no puede quedar vacío.");

            if (string.IsNullOrWhiteSpace(marca.DESCRIPCION))
                throw new Exception("La descripción de la marca no puede quedar vacía.");

            // Le paso la responsabilidad de guardar los cambios al repositorio.
            return await _MarcaRepo.ActualizarAsync(marca);
        }

        /// <summary>
        /// Protejo el historial de nuestro sistema aplicando un borrado lógico en lugar de uno físico.
        /// </summary>
        public async Task Eliminar(long id)
        {
            // Primero busco el registro para asegurarme de que no estoy intentando borrar un fantasma.
            MARCAS marca = await _MarcaRepo.ObtenerPorIdAsync(id);

            // Si el objeto no existe, corto el flujo de inmediato para que el controlador notifique al cliente.
            if (marca == null)
            {
                throw new Exception("La marca que intenta eliminar no existe.");
            }

            // Realizo un "Soft Delete" (borrado lógico). 
            // Como la tabla MARCAS tiene una colección de PRODUCTOS, si hiciera un DELETE físico en SQL, 
            // rompería la integridad referencial y me daría un error por llave foránea.
            marca.ACTIVO = false;

            // Consolido el cambio de estado usando el método de actualizar.
            await _MarcaRepo.ActualizarAsync(marca);
        }

        /// <summary>
        /// Rescato una marca específica de la base de datos usando su llave primaria.
        /// </summary>
        public async Task<MARCAS> ObtenerPorId(long id)
        {
            // Simplemente delego la consulta al repositorio.
            return await _MarcaRepo.ObtenerPorIdAsync(id);
        }

        /// <summary>
        /// Retorno el catálogo completo de marcas, asegurándome de no incluir aquellas que fueron dadas de baja.
        /// </summary>
        public async Task<IEnumerable<MARCAS>> ObtenerTodasLasMarcas()
        {
            // Agrego una cláusula LINQ para filtrar y entregar únicamente las marcas operativas.
            // Si nuestro repositorio acepta expresiones lambda, esto se traducirá a un 'WHERE ACTIVO = 1' en SQL.

            return (await _MarcaRepo.ObtenerTodos()).Where(u => u.ACTIVO == true);
        }

        public async Task<IEnumerable<MARCAS>> ObtenerMarcasPorCategoria(long idCategoria)
        {
            return await _MarcaRepo.ObtenerMarcasPorCategoria(idCategoria);
    }
    }
}
