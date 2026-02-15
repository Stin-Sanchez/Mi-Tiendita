namespace ENTIDADES
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
   

    public partial class DETALLE_VENTAS
    {
        [Key]
        public int ID_DETALLE { get; set; }

        public int? ID_VENTA { get; set; }

        public int? ID_PRODUCTO { get; set; }

        public int? CANTIDAD { get; set; }

        public decimal TOTAL { get; set; }

        public virtual PRODUCTOS PRODUCTOS { get; set; }

        public virtual VENTAS VENTAS { get; set; }
    }
}
