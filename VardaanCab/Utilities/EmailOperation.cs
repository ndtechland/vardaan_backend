using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace VardaanCab.Utilities
{
    public class EmailOperation
    {
        public static bool SendEmail(string recipeint, string subject, string msg, bool IsBodyHtml,string attachmentPath="")
        {
            try
            {
                string sender = ConfigurationManager.AppSettings["smtpUser"];
                string password = ConfigurationManager.AppSettings["smtpPass"];
                MailMessage message = new MailMessage();
                message.From = new MailAddress(sender);
                message.To.Add(recipeint);
                message.Subject = subject;
                message.Body = msg;
                message.IsBodyHtml = IsBodyHtml;
                if(!string.IsNullOrEmpty(attachmentPath))
                {
                    System.Net.Mail.Attachment attachment;
                    attachment = new System.Net.Mail.Attachment(attachmentPath);
                    message.Attachments.Add(attachment);
                }
                SmtpClient client = new SmtpClient();

                client.Host = "smtp.gmail.com";
                client.Port = 587;
                //client.Port = 25;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(sender, password);
                client.EnableSsl = true;
                client.Send(message);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }

        }

        public static string SendEmailStr(string recipeint, string subject, string msg, bool IsBodyHtml, string attachmentPath = "")
        {
            try
            {
                string sender = ConfigurationManager.AppSettings["smtpUser"];
                string password = ConfigurationManager.AppSettings["smtpPass"];
                MailMessage message = new MailMessage();
                message.From = new MailAddress(sender);
                message.To.Add(recipeint);
                message.Subject = subject;
                message.Body = msg;
                message.IsBodyHtml = IsBodyHtml;
                if (!string.IsNullOrEmpty(attachmentPath))
                {
                    System.Net.Mail.Attachment attachment;
                    attachment = new System.Net.Mail.Attachment(attachmentPath);
                    message.Attachments.Add(attachment);
                }
                SmtpClient client = new SmtpClient();

                client.Host = "smtp.gmail.com";
                client.Port = 587;
                //client.Port = 25;
                client.UseDefaultCredentials = false;               
                client.Credentials = new System.Net.NetworkCredential(sender, password);
                client.EnableSsl = true;
               //
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(message);
                return "true";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}