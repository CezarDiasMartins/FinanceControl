using System.Configuration;
using System.Net;
using System.Net.Mail;
using FinanceControl.Core.Interfaces.IServices;

namespace FinanceControl.Core.Services
{
    public class EmailService : IEmailService
    {
        public void EnviarRecuperacaoSenha(string destinatario, string nome, string link)
        {
            var host = ConfigurationManager.AppSettings["SmtpHost"];
            var portaTexto = ConfigurationManager.AppSettings["SmtpPort"];
            var usuario = ConfigurationManager.AppSettings["SmtpUser"];
            var senha = ConfigurationManager.AppSettings["SmtpPassword"];
            var remetente = ConfigurationManager.AppSettings["SmtpFrom"];

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(remetente))
                return;

            var porta = string.IsNullOrWhiteSpace(portaTexto) ? 587 : int.Parse(portaTexto);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(remetente, "FinanceControl");
                mail.To.Add(destinatario);
                mail.Subject = "Recuperação de senha - FinanceControl";
                mail.Body = string.Format("Olá, {0}. Acesse o link abaixo para criar uma nova senha:\n\n{1}", nome, link);
                mail.IsBodyHtml = false;

                using (var smtp = new SmtpClient(host, porta))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(usuario, senha);
                    smtp.EnableSsl = true;
                    smtp.Timeout = 15000;

                    smtp.Send(mail);
                }
            }
        }
    }
}