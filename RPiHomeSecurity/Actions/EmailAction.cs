using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RPiHomeSecurity
{
    internal class EmailAction : Action
    {
        private string subject;
        private string body;

        public EmailAction(String subject, String body)
        {
            this.subject = subject;
            this.body = body;
        }

        public override void RunAction(Alarm AlarmController)
        {
            log.LogDebugMessage("running Email action. Subject: " + subject);
            SendEmail(subject, body, AlarmController.Config);
        }

        //send an email
        public void SendEmail(String subject, String body, Config config)
        {
            try
            {
                log.LogDebugMessage("Send email: '" + subject + "' to " + config.EmailAddress);

                String toAddress = config.EmailAddress;
                MailAddress from = new MailAddress(config.SmtpFromAddress);
                MailAddress to = new MailAddress(toAddress);

                string fromPassword = config.SmtpPassword;
                var smtpClient = new SmtpClient
                {
                    Host = config.SmtpServer,
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(from.Address, fromPassword)
                };
                ServicePointManager.ServerCertificateValidationCallback =
                    delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    { return true; };

                MailMessage msg = new MailMessage(from, to)
                {
                    Subject = subject,
                    Body = DateTime.Now.ToLongDateString() + Environment.NewLine +
                    DateTime.Now.ToLongTimeString() + Environment.NewLine +
                    config.EmailBody + Environment.NewLine +
                    body
                };

                smtpClient.SendCompleted += (s, e) =>
                {
                    smtpClient.Dispose();
                    msg.Dispose();
                };

                smtpClient.SendAsync(msg, null);
            }
            catch (Exception e)
            {
                log.LogError("Error sending email: " + e.ToString());
            }
        }
    }
}