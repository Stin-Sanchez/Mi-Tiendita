namespace ENTIDADES
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    public partial class CLIENTES
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CLIENTES()
        {
            CARRITO = new HashSet<CARRITO>();
            VENTAS = new HashSet<VENTAS>();
            FECHA_CREACION = DateTime.Now;
            RESTABLECER = true;
            ACTIVO = true;
        }

        [Key]
        public int ID_CLIENTE { get; set; }

        [Required]
        [StringLength(50)]
        public string NOMBRE { get; set; }

        [Required]
        [StringLength(50)]
        public string APELLIDO { get; set; }

        [Required]
        [StringLength(100)]
        public string CORREO { get; set; }

        [Required]
        [StringLength(150)]
        public string CLAVE { get; set; }

        public bool ACTIVO { get; set; }

        [NotMapped]
        public string ConfirmarClave { get; set; }

        public bool? RESTABLECER { get; set; }

        public DateTime FECHA_CREACION { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CARRITO> CARRITO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VENTAS> VENTAS { get; set; }
    }
}
