using IdentityDemo.Classes;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace IdentityDemo.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<EmailSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public EmailSenderOptions Options { get; } //set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string body)
        {
            //return Execute(Options, subject, body, email, true);
            return Task.CompletedTask; //For Testing
        }

        public Task Execute(EmailSenderOptions options, string subject, string body, string email, bool isBodyHtml)
        {
            MailMessage msg = new MailMessage(options.SenderEmailAddress, email);
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = isBodyHtml;
            using (SmtpClient client = new SmtpClient())// "smtp.gmail.com", 465)
            {
                client.Host = options.Host;
                client.Port = options.Port;
                client.EnableSsl = options.EnableSsl;
                client.Credentials = new System.Net.NetworkCredential(options.UserName, options.Password);
                return client.SendMailAsync(msg);
            }
        }
    }
}
