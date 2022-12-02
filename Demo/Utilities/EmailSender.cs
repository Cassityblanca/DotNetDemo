using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MailKit.Net.Smtp;
using System.Net.Mail;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
//using SendGrid;
//using SendGrid.Helpers.Mail;

namespace Demo.Utilities
{
    public class EmailSender : IEmailSender
    {

        //public string SendGridSecret { get; set; }

        //public EmailSender(IConfiguration _config)
        //{
        //    SendGridSecret = _config.GetValue<string>("SendGrid:SecretKey");
        //}
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailToSend = new MimeMessage();

            emailToSend.From.Add(MailboxAddress.Parse("\"Richard Fry\"<cassityblanca.junk@gmail.com>"));

            emailToSend.To.Add(MailboxAddress.Parse(email));
            emailToSend.Subject = subject;
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            //send email with G-mail
            using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
            {
                emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                emailClient.Authenticate("cassityblanca.junk@gmail.com", "etnvinyhnmszdkqg");
                emailClient.Send(emailToSend);
                emailClient.Disconnect(true);
            }

            return Task.CompletedTask;

            //Twillio SendGrid

            //var client = new SendGridClient(SendGridSecret);
            //var from = new EmailAddress("rich@richfry.com", "Sender Display Name");
            //var to = new EmailAddress(email);
            //var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);
            //return client.SendEmailAsync(msg);
        }
    }
}
