namespace ENTIDADES.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    internal sealed class Configuration : DbMigrationsConfiguration<DAO.ModelContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DAO.ModelContext context)
        {
            if (!context.USUARIOS.Any())
            {
                context.USUARIOS.AddOrUpdate(u => u.CORREO, new ENTIDADES.USUARIOS
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
}
