using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Skimur.Email;

namespace Skimur.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly IOptions<EmailOptions> _emailOptions;

        public EmailSender(IOptions<EmailOptions> emailOptions)
        {
            _emailOptions = emailOptions;
        }

        public async Task SendEmailAsync(string toAddress, string subject, string body)
        {
            var options = _emailOptions.Value;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(options.FromName, options.FromEmail));
            message.To.Add(new MailboxAddress(toAddress, toAddress));
            
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = body
            };
            
            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync(options.Host,
                    options.Port,
                    options.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                smtpClient.AuthenticationMechanisms.Remove("XOATH2");

                if (options.UseDefaultCredentials)
                {
                    await smtpClient.AuthenticateAsync(options.UserName, options.Password);
                }

                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);
            }
        }
    }
}
