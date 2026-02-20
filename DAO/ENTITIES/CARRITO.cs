namespace ENTIDADES
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
   

    [Table("CARRITO")]
    public partial class CARRITO
    {
        [Key]
        public int ID_CARRITO { get; set; }

        public int? ID_CLIENTE { get; set; }

        public int? ID_PRODUCTO { get; set; }

        public int? CANTIDAD { get; set; }

        public DateTime FECHA_CREACION { get; set; }

        public virtual CLIENTES CLIENTES { get; set; }

        public virtual PRODUCTOS PRODUCTOS { get; set; }
    }
}
