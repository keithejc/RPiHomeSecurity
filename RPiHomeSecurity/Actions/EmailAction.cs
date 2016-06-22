using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using RPiHomeSecurity.Utils;

namespace RPiHomeSecurity
{
    internal class EmailAction : Action
    {
        public string Subject;
        public string Body;

        public EmailAction(String subject, String body)
        {
            this.Subject = subject;
            this.Body = body;
        }

        public override void RunAction(Alarm alarmController)
        {
            Log.LogMessage("running Email action. Subject: " + Subject);
            SendEmail(Subject, Body, alarmController.Config);
        }

        public String ReplaceTokens(String text)
        {
            var ret = text;

            if (text.Contains(@"%PUBLICIP%"))
            {
                var pIp = WebUtils.GetPublicIp();
                ret = text.Replace(@"%PUBLICIP%", pIp);
            }

            return ret;
        }


        //send an email
        public void SendEmail(String subject, String body, Config config)
        {
            try
            {
                Log.LogMessage("Send email: '" + subject + "' to " + config.EmailAddress);

                var toAddress = config.EmailAddress;
                var from = new MailAddress(config.SmtpFromAddress);
                var to = new MailAddress(toAddress);

                var fromPassword = config.SmtpPassword;
                Log.LogMessage("Send email: password: " + fromPassword);
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
                    {
                        Log.LogMessage("validation callback");
                        return true;
                    };

                var msg = new MailMessage(from, to)
                {
                    Subject = ReplaceTokens(subject),
                    Body = ReplaceTokens(DateTime.Now.ToLongDateString() + Environment.NewLine +
                    DateTime.Now.ToLongTimeString() + Environment.NewLine +
                    config.EmailBody + Environment.NewLine + body)
                };

                smtpClient.SendCompleted += (s, e) =>
                {
                    Log.LogMessage("Send email: send complete");

                    smtpClient.Dispose();
                    msg.Dispose();
                };

                smtpClient.SendAsync(msg, null);
            }
            catch (Exception e)
            {
                Log.LogError("Error sending email: " + e.ToString());
            }
        }
    }
}