using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace Negocio.Managers.Shared
{
    public class SendEmailManager
    {
        private readonly SmtpClient smtpClient = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(ConfigurationManager.AppSettings["tFlexEmail"], 
                ConfigurationManager.AppSettings["tFlexEmailPass"])
        };

        private void SendEmail(string to, string subject, string body)
        {
            try
            {
                MailMessage mm = new MailMessage();
                mm.To.Add(to);
                mm.From = new MailAddress(ConfigurationManager.AppSettings["tFlexEmail"]);
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

        public void SendRegisterEmail(string receiver, string encriptedUserId)
        {
            try
            {
                string subject = "REGISTRO T-FLEX";
                string body = "Hola bienvenido a T-FLEX, para poder activar tu usuario debes hacer click en el siguiente Link: "
                    + ConfigurationManager.AppSettings["validarEmailLink"] + "?id=" + encriptedUserId.Replace("+", "%2B");
                SendEmail(receiver, subject, body);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SendValidationEmail(string receiver, string emailId)
        {
            try
            {
                string subject = "T-FLEX Validación de Correo";
                string body = "Hola haga click en el siguiente link para validar la cuenta de correo agregada en T-FLEX. /n" + System.Configuration.ConfigurationManager.AppSettings["validarEmailLink"] + "?id=" + emailId.Replace("+", "%2B")
                    + " Si desconoce el proceso mencionado desestime este correo.";
                SendEmail(receiver, subject, body);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}
