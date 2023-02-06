using AppSecurityPracticalAssignment210733T.Settings;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace AppSecurityPracticalAssignment210733T.Services
{
    public class EmailSenderService
    {
        private const string EMAIL_SENDER = "freshfarmmarketbusiness@gmail.com";
        private readonly EmailConfiguration _emailConfiguration;

        public EmailSenderService(EmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }

        public async Task SendEmailAsync(string emailAddressReceiver, string subject, string message)
        {
            MailMessage newMail = new MailMessage();
            // use the Gmail SMTP Host
            SmtpClient client = new SmtpClient("smtp.gmail.com");

            // Follow the RFS 5321 Email Standard
            newMail.From = new MailAddress(EMAIL_SENDER, "freshfarmmarketbusiness@gmail.com");

            newMail.To.Add(emailAddressReceiver);// declare the email subject

            newMail.Subject = subject; // use HTML for the email body

            newMail.IsBodyHtml = false;
            newMail.Body = message;
            client.UseDefaultCredentials = false;
            // enable SSL for encryption across channels
            client.EnableSsl = true;
            // Port 465 for SSL communication
            client.Port = 587;
            // Provide authentication information with Gmail SMTP server to authenticate your sender account
            client.Credentials = new System.Net.NetworkCredential(EMAIL_SENDER, _emailConfiguration.API);

            await client.SendMailAsync(newMail); // Send the constructed mail
            Console.WriteLine("Email Sent");

        }

    }
}
