using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace Negocio.Managers.Shared
{
    public class EnvioEmailManager
    {
        private const string sender = "transporteflexible@gmail.com";
        private const string senderPassword = "fleXI!123";
        private readonly SmtpClient smtpClient = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(sender, senderPassword)
        };

        private void EnviarEmail(string to, string subject, string body)
        {
            try
            {
                MailMessage mm = new MailMessage();
                mm.To.Add(to);
                mm.From = new MailAddress(sender);
                mm.Subject = subject;
                mm.Body = body;
                mm.IsBodyHtml = false;
                mm.Priority = MailPriority.Normal;
                smtpClient.Send(mm);
                mm.Dispose();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void EnviarEmailRegistro(string receiver, string idEncriptado)
        {
            try
            {
                string subject = "REGISTRO T-FLEX";
                string body = "Hola bienvenido a T-FLEX, para poder activar tu usuario debes hacer click en el siguiente Link: "
                    + ConfigurationManager.AppSettings["validarEmailLink"] + "?id=" + idEncriptado.Replace("+", "%2B");
                EnviarEmail(receiver, subject, body);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
