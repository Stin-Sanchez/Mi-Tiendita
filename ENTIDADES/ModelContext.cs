using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ENTIDADES;

namespace DAO
{
    public class ModelInitializer : CreateDatabaseIfNotExists<ModelContext>
    {
        protected override void Seed(ModelContext context)
        {
            if (!context.USUARIOS.Any())
            {
                context.USUARIOS.Add(new USUARIOS
                {
                    NOMBRE = "Admin",
                    APELLIDO = "Sistema",
                    CORREO = "admin@admin.com",
                    CLAVE = Hash("admin123"),
                    ACTIVO = true,
                    RESTABLECER = false,
                    FECHA_CREACION = DateTime.Now
                });
                context.SaveChanges();
            }
        }

        private static string Hash(string texto)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(texto));
                var sb = new StringBuilder();
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }

    public partial class ModelContext : DbContext
    {
        static ModelContext()
        {
            Database.SetInitializer(new System.Data.Entity.MigrateDatabaseToLatestVersion<ModelContext, ENTIDADES.Migrations.Configuration>());
        }

        public ModelContext()
            : base("name=ModelContext")
        {
        }

        public virtual DbSet<CARRITO> CARRITO { get; set; }
        public virtual DbSet<CATEGORIAS> CATEGORIAS { get; set; }
        public virtual DbSet<CLIENTES> CLIENTES { get; set; }
        public virtual DbSet<DETALLE_VENTAS> DETALLE_VENTAS { get; set; }
        public virtual DbSet<MARCAS> MARCAS { get; set; }
        public virtual DbSet<PRODUCTOS> PRODUCTOS { get; set; }
        public virtual DbSet<USUARIOS> USUARIOS { get; set; }
        public virtual DbSet<VENTAS> VENTAS { get; set; }
        public virtual DbSet<DEPARTAMENTO> DEPARTAMENTO { get; set; }
        public virtual DbSet<DISTRITO> DISTRITO { get; set; }
        public virtual DbSet<PROVINCIA> PROVINCIA { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CATEGORIAS>()
                .Property(e => e.NOMBRE)
                .IsUnicode(false);

            modelBuilder.Entity<CATEGORIAS>()
                .Property(e => e.DESCRIPCION)
                .IsUnicode(false);

            modelBuilder.Entity<CLIENTES>()
                .Property(e => e.NOMBRE)
                .IsUnicode(false);

            modelBuilder.Entity<CLIENTES>()
                .Property(e => e.APELLIDO)
                .IsUnicode(false);

            modelBuilder.Entity<CLIENTES>()
                .Property(e => e.CORREO)
                .IsUnicode(false);

            modelBuilder.Entity<CLIENTES>()
                .Property(e => e.CLAVE)
                .IsUnicode(false);

            modelBuilder.Entity<DETALLE_VENTAS>()
                .Property(e => e.TOTAL)
                .HasPrecision(10, 2);

            modelBuilder.Entity<MARCAS>()
                .Property(e => e.NOMBRE)
                .IsUnicode(false);

            modelBuilder.Entity<MARCAS>()
                .Property(e => e.DESCRIPCION)
                .IsUnicode(false);

            modelBuilder.Entity<PRODUCTOS>()
                .Property(e => e.NOMBRE)
                .IsUnicode(false);

            modelBuilder.Entity<PRODUCTOS>()
                .Property(e => e.DESCRIPCION)
                .IsUnicode(false);

            modelBuilder.Entity<PRODUCTOS>()
                .Property(e => e.PRECIO)
                .HasPrecision(10, 2);

            modelBuilder.Entity<PRODUCTOS>()
                .Property(e => e.RUTA_IMAGEN)
                .IsUnicode(false);

            modelBuilder.Entity<PRODUCTOS>()
                .Property(e => e.NOMBRE_IMAGEN)
                .IsUnicode(false);

            modelBuilder.Entity<USUARIOS>()
                .Property(e => e.NOMBRE)
                .IsUnicode(false);

            modelBuilder.Entity<USUARIOS>()
                .Property(e => e.APELLIDO)
                .IsUnicode(false);

            modelBuilder.Entity<USUARIOS>()
                .Property(e => e.CORREO)
                .IsUnicode(false);

            modelBuilder.Entity<USUARIOS>()
                .Property(e => e.CLAVE)
                .IsUnicode(false);

            modelBuilder.Entity<VENTAS>()
                .Property(e => e.MONTO_TOTAL)
                .HasPrecision(10, 2);

            modelBuilder.Entity<VENTAS>()
                .Property(e => e.CONTACTO)
                .IsUnicode(false);

            modelBuilder.Entity<VENTAS>()
                .Property(e => e.ID_DISTRITO)
                .IsUnicode(false);

            modelBuilder.Entity<VENTAS>()
                .Property(e => e.TELEFONO)
                .IsUnicode(false);

            modelBuilder.Entity<VENTAS>()
                .Property(e => e.DIRECCION)
                .IsUnicode(false);

            modelBuilder.Entity<VENTAS>()
                .Property(e => e.ID_TRANSACCION)
                .IsUnicode(false);

            modelBuilder.Entity<DEPARTAMENTO>()
                .Property(e => e.ID_DEPARTAMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<DEPARTAMENTO>()
                .Property(e => e.DESCRIPCION)
                .IsUnicode(false);

            modelBuilder.Entity<DISTRITO>()
                .Property(e => e.ID_DISTRITO)
                .IsUnicode(false);

            modelBuilder.Entity<DISTRITO>()
                .Property(e => e.ID_PROVINCIA)
                .IsUnicode(false);

            modelBuilder.Entity<DISTRITO>()
                .Property(e => e.ID_DEPARTAMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<DISTRITO>()
                .Property(e => e.DESCRIPCION)
                .IsUnicode(false);

            modelBuilder.Entity<PROVINCIA>()
                .Property(e => e.ID_PROVINCIA)
                .IsUnicode(false);

            modelBuilder.Entity<PROVINCIA>()
                .Property(e => e.ID_DEPARTAMENTO)
                .IsUnicode(false);

            modelBuilder.Entity<PROVINCIA>()
                .Property(e => e.DESCRIPCION)
                .IsUnicode(false);
        }
    }
}
