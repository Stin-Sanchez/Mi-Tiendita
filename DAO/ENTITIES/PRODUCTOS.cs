namespace ENTIDADES
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    public partial class PRODUCTOS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PRODUCTOS()
        {
            CARRITO = new HashSet<CARRITO>();
            DETALLE_VENTAS = new HashSet<DETALLE_VENTAS>();
        }

        [Key]
        public int ID_PRODUCTO { get; set; }

        [Required]
        [StringLength(50)]
        public string NOMBRE { get; set; }

        [Required]
        [StringLength(100)]
        public string DESCRIPCION { get; set; }

        public int? ID_MARCA { get; set; }

        public int? ID_CATEGORIA { get; set; }

        public decimal PRECIO { get; set; }

        public int? STOCK { get; set; }

        [StringLength(100)]
        public string RUTA_IMAGEN { get; set; }

        [StringLength(100)]
        public string NOMBRE_IMAGEN { get; set; }

        public bool ACTIVO { get; set; }

        public DateTime FECHA_CREACION { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CARRITO> CARRITO { get; set; }

        public virtual CATEGORIAS CATEGORIAS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DETALLE_VENTAS> DETALLE_VENTAS { get; set; }

        public virtual MARCAS MARCAS { get; set; }
    }
}
