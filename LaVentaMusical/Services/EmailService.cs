using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace LaVentaMusical.Services
{
    public static class InvoiceEmailService
    {
        public static void Send(string toEmail, string subject, byte[] pdfBytes, string fileName)
        {
            var host = ConfigurationManager.AppSettings["SmtpHost"];
            var port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
            var user = ConfigurationManager.AppSettings["SmtpUser"];
            var pass = ConfigurationManager.AppSettings["SmtpPass"];
            var from = ConfigurationManager.AppSettings["FromEmail"];

            using (var msg = new MailMessage(from, toEmail, subject, "Adjuntamos su factura (demo)."))
            {
                msg.Attachments.Add(new Attachment(new System.IO.MemoryStream(pdfBytes), fileName, "application/pdf"));
                using (var client = new SmtpClient(host, port))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(user, pass);
                    client.Send(msg);
                }
            }
        }

    }
}
