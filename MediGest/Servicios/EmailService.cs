using System.Net;
using System.Net.Mail;
using System.IO;

namespace MediGest.Servicios
{
    public class EmailService
    {
        private readonly string smtpUser;
        private readonly string smtpPass;

        public EmailService(string user)
        {
            smtpUser = user;   // correo del médico
            smtpPass = "bydh ghmt ufrw lbmc";   // contraseña de aplicación de Gmail
        }

        public void EnviarCorreo(string destinatario, string asunto, string htmlBody, string rutaLogo)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(smtpUser);
            mail.To.Add(destinatario);
            mail.Subject = asunto;

            // *** IMPORTANTE: NO USAR mail.Body ni IsBodyHtml ***
            mail.Body = "";
            mail.IsBodyHtml = false;

            // Crear vista HTML
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");

            // Agregar logo como recurso CID
            if (File.Exists(rutaLogo))
            {
                LinkedResource logo = new LinkedResource(rutaLogo, "image/jpeg");
                logo.ContentId = "logo_medigest";
                logo.ContentType.MediaType = "image/jpeg";
                logo.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;

                htmlView.LinkedResources.Add(logo);
            }

            mail.AlternateViews.Add(htmlView);

            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
        }

        public string CargarPlantilla(string ruta)
        {
            return File.ReadAllText(ruta);
        }
    }
}
