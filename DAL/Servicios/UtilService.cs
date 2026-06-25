using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace DAL.Servicios
{
    public class UtilService
    {
        public static string GenerarClaveUnica()
        {
            string clave = Guid.NewGuid().ToString("N").Substring(0, 6);

            return clave;
        }

        public static string EncriptarClave(string texto)
        {
            StringBuilder sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }

        }

        // Agregamos "async Task<bool>"
        public static async Task<bool> EnviarCorreo(string correo, string asunto, string mensaje,string correoEmisor, string claveEmisor)
        {
            bool resultado = false;
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(correo);
                mail.From = new MailAddress(correoEmisor);
                mail.Subject = asunto;
                mail.Body = mensaje;
                mail.IsBodyHtml = true;

                var smtp = new SmtpClient()
                {
                    Credentials = new NetworkCredential(correoEmisor, claveEmisor),
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true
                };

                //Usamos await y SendMailAsync
                await smtp.SendMailAsync(mail);
                resultado = true;
            }
            catch (Exception e)
            {
                resultado = false;
                Console.WriteLine(e.Message);
            }

            return resultado;
        }

    }
}

