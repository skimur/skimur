using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Skimur.Email
{
    /// <summary>
    /// Email sender
    /// </summary>
    public class EmailSender : IEmailSender
    {
        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="emailAccount">Email account to use</param>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="fromAddress">From address</param>
        /// <param name="fromName">From display name</param>
        /// <param name="toAddress">To address</param>
        /// <param name="toName">To display name</param>
        /// <param name="replyTo">ReplyTo address</param>
        /// <param name="replyToName">ReplyTo display name</param>
        /// <param name="bcc">BCC addresses list</param>
        /// <param name="cc">CC addresses list</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        public void SendEmail(EmailServerSettings emailAccount,
            string subject,
            string body,
            string fromAddress,
            string fromName,
            string toAddress,
            string toName,
             string replyTo = null,
            string replyToName = null,
            IEnumerable<string> bcc = null,
            IEnumerable<string> cc = null,
            string attachmentFilePath = null,
            string attachmentFileName = null)
        {
            fromAddress = fromAddress ?? emailAccount.FromEmail;
            fromName = fromName ?? emailAccount.FromName;
            var message = new MailMessage { From = new MailAddress(fromAddress, fromName) };
            message.To.Add(new MailAddress(toAddress, toName));

            if (!string.IsNullOrEmpty(replyTo))
                message.ReplyToList.Add(new MailAddress(replyTo, replyToName));

            if (bcc != null)
                foreach (var address in bcc.Where(bccValue => !string.IsNullOrWhiteSpace(bccValue)))
                    message.Bcc.Add(address.Trim());

            if (cc != null)
                foreach (var address in cc.Where(ccValue => !string.IsNullOrWhiteSpace(ccValue)))
                    message.CC.Add(address.Trim());

            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            if (!string.IsNullOrEmpty(attachmentFilePath) &&
                File.Exists(attachmentFilePath))
            {
                var attachment = new Attachment(attachmentFilePath);
                attachment.ContentDisposition.CreationDate = File.GetCreationTime(attachmentFilePath);
                attachment.ContentDisposition.ModificationDate = File.GetLastWriteTime(attachmentFilePath);
                attachment.ContentDisposition.ReadDate = File.GetLastAccessTime(attachmentFilePath);
                if (!String.IsNullOrEmpty(attachmentFileName))
                {
                    attachment.Name = attachmentFileName;
                }
                message.Attachments.Add(attachment);
            }

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.UseDefaultCredentials = emailAccount.UseDefaultCredentials;
                smtpClient.Host = emailAccount.Host;
                smtpClient.Port = emailAccount.Port;
                smtpClient.EnableSsl = emailAccount.EnableSsl;
                smtpClient.Credentials = emailAccount.UseDefaultCredentials ? CredentialCache.DefaultNetworkCredentials : new NetworkCredential(emailAccount.UserName, emailAccount.Password);
                smtpClient.Send(message);
            }
        }
    }
}
