namespace ENTIDADES
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("PROVINCIA")]
    public partial class PROVINCIA
    {
        [StringLength(4)]
        public string ID_PROVINCIA { get; set; }

        [StringLength(2)]
        public string ID_DEPARTAMENTO { get; set; }

        [StringLength(50)]
        public string DESCRIPCION { get; set; }

        [Key]
        public bool ACTIVO { get; set; }

        public DateTime? FECHA_CREACION { get; set; }
    }
}
