namespace ENTIDADES
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
  

    public partial class USUARIOS
    {
        public USUARIOS()
        {
            this.FECHA_CREACION = DateTime.Now;
            this.RESTABLECER = true;
        }

        [Key]
        public int ID_USUARIO { get; set; }

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

        public bool? RESTABLECER { get; set; }

        public bool ACTIVO { get; set; }

        public DateTime? FECHA_CREACION { get; set; }
    }
}
