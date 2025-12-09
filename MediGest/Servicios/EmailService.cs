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

        public void EnviarCorreo(string destinatario, string asunto, string htmlBody)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(smtpUser);
            mail.To.Add(destinatario);
            mail.Subject = asunto;
            mail.Body = htmlBody;
            mail.IsBodyHtml = true;

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

        public string InsertarLogo(string html, string rutaLogo)
        {
            //byte[] bytes = File.ReadAllBytes(rutaLogo);
            //string base64Logo = Convert.ToBase64String(bytes);
            //return html.Replace("{{LogoBase64}}", base64Logo);
            return html.Replace("{{LogoBase64}}", rutaLogo);
        }

    }
}
