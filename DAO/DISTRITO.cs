namespace ENTIDADES
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
   

    [Table("DISTRITO")]
    public partial class DISTRITO
    {
        [StringLength(6)]
        public string ID_DISTRITO { get; set; }

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
