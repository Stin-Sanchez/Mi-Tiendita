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
    /// Aquí centralizo toda la lógica de negocio para la gestión de PRODUCTOS.
    /// Mi responsabilidad principal es garantizar que un producto no se guarde con datos incongruentes 
    /// (como precios negativos o asociaciones fantasma) antes de pasarlo a la capa de datos.
    /// </summary>
    public class ProductoServiceImp : IProductoService
    {
        // Utilizo nuestro repositorio genérico que ya configuramos en la inyección de dependencias.
        private readonly IProductoRepository _repository;

        /// <summary>
        /// Inyecto el repositorio a través del constructor para mantener el bajo acoplamiento.
        /// </summary>
        public ProductoServiceImp(IProductoRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Proceso la creación de un nuevo producto asegurándome de que sus asociaciones 
        /// (Marca y Categoría) se manejen correctamente a nivel de llaves foráneas.
        /// </summary>
        public async Task<PRODUCTOS> Insertar(PRODUCTOS producto)
        {
            // Primero, detengo la ejecución si el objeto llega nulo para evitar caídas del sistema.
            if (producto == null)
                throw new ArgumentNullException(nameof(producto), "El producto no puede ser nulo.");

            // Valido que los campos de texto obligatorios cumplan con la longitud de la base de datos.
            if (string.IsNullOrWhiteSpace(producto.NOMBRE) || producto.NOMBRE.Length > 50)
                throw new ArgumentException("El nombre del producto es obligatorio y no debe superar los 50 caracteres.");

            if (string.IsNullOrWhiteSpace(producto.DESCRIPCION) || producto.DESCRIPCION.Length > 100)
                throw new ArgumentException("La descripción es obligatoria y no debe superar los 100 caracteres.");

            // Valido las reglas de negocio financieras y de inventario. 
            // No permito regalar productos (precio 0 o negativo) ni inventarios ilógicos.
            if (producto.PRECIO <= 0)
                throw new ArgumentException("El precio del producto debe ser mayor a cero.");

            if (producto.STOCK < 0)
                throw new ArgumentException("El stock no puede ser negativo.");

            // Valido que el producto venga asociado a una marca y categoría. 
            // Aunque en la entidad son nullables (int?), para nuestro negocio un producto huérfano no tiene sentido.
            if ((producto.ID_MARCA ?? 0) <= 0 || (producto.ID_CATEGORIA ?? 0) <= 0)
                throw new ArgumentException("El producto debe estar asociado obligatoriamente a una Marca y a una Categoría válidas.");

            // ESTE ES EL TRUCO PARA LAS ASOCIACIONES:
            // Limpio las propiedades de navegación (los objetos virtuales). 
            // Hago esto porque solo me interesan el ID_MARCA y el ID_CATEGORIA. Si dejo estos objetos llenos, 
            // Entity Framework podría pensar que son registros nuevos e intentará insertarlos de nuevo, duplicando datos.
            producto.CATEGORIAS = null;
            producto.MARCAS = null;
            producto.CARRITO = null;
            producto.DETALLE_VENTAS = null;

            // Establezco los valores por defecto del sistema.
            producto.FECHA_CREACION = DateTime.Now;
            producto.ACTIVO = true;

            // Finalmente, delego la persistencia al repositorio.
            return await _repository.InsertarAsync(producto);
        }

        /// <summary>
        /// Aplico las mismas reglas estrictas para la actualización de un producto existente.
        /// </summary>
        public async Task<PRODUCTOS> Actualizar(PRODUCTOS producto)
        {
            if (producto == null || producto.ID_PRODUCTO <= 0)
                throw new ArgumentException("Datos de producto inválidos para actualizar.");

            if (producto.PRECIO <= 0)
                throw new ArgumentException("El precio del producto debe ser mayor a cero.");

            if ((producto.ID_MARCA ?? 0) <= 0 || (producto.ID_CATEGORIA ?? 0) <= 0)
                throw new ArgumentException("El producto debe estar asociado a una Marca y Categoría.");

            // Limpio las navegaciones al igual que en la inserción para evitar conflictos con el Tracking de Entity Framework.
            producto.CATEGORIAS = null;
            producto.MARCAS = null;

            return await  _repository.ActualizarAsync(producto);
        }

        /// <summary>
        /// Doy de baja un producto del sistema.
        /// </summary>
        public async Task Eliminar(long id)
        {
            if (id <= 0) throw new ArgumentException("ID de producto inválido.");

            // Ojo equipo: Al igual que con las categorías, si este producto ya está en el CARRITO de un cliente 
            // o en un DETALLE_VENTAS, el motor de SQL lanzará una excepción por llave foránea. 
            // En el futuro, podríamos cambiar esto por un borrado lógico (producto.ACTIVO = false).
            await _repository.EliminarAsync(id);
        }

        /// <summary>
        /// Busco el detalle de un producto en particular.
        /// </summary>
        public async Task<PRODUCTOS> ObtenerPorId(long id)
        {
            return await _repository.ObtenerPorIdAsync(id);
        }

        /// <summary>
        /// Obtengo el catálogo completo de productos.
        /// </summary>
        public async Task <IEnumerable<PRODUCTOS>> ObtenerTodos()
        {
            return (await _repository.ObtenerTodos()).Where(p => p.ACTIVO ==true);
           
        }

        public async Task<int> ObtenerTotalProductos()
        {
            return await _repository.ObtenerTotalProductosAsync();
        }

        public async Task<int> ObtenerProductosConStockCritico(int umbralMinimo = 5)
        {
            return await _repository.ObtenerProductosConStockCriticoAsync();
        }
    }
}

