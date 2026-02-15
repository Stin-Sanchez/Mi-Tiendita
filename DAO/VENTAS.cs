namespace ENTIDADES
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
   

    public partial class VENTAS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VENTAS()
        {
            DETALLE_VENTAS = new HashSet<DETALLE_VENTAS>();
        }

        [Key]
        public int ID_VENTA { get; set; }

        public int? ID_CLIENTE { get; set; }

        public int? TOTAL_PRODUCTOS { get; set; }

        public decimal MONTO_TOTAL { get; set; }

        [StringLength(100)]
        public string CONTACTO { get; set; }

        [StringLength(50)]
        public string ID_DISTRITO { get; set; }

        [StringLength(25)]
        public string TELEFONO { get; set; }

        [StringLength(200)]
        public string DIRECCION { get; set; }

        [StringLength(50)]
        public string ID_TRANSACCION { get; set; }

        public DateTime FECHA_VENTA { get; set; }

        public virtual CLIENTES CLIENTES { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DETALLE_VENTAS> DETALLE_VENTAS { get; set; }
    }
}
