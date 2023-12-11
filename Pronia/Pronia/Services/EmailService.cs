using Pronia.Intefaces;
using System.Net;
using System.Net.Mail;

namespace Pronia.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration )
        {
            _configuration = configuration;
        }
        public async Task SendMailAsync(string emailTo,string subject,string body,bool isHtml)
        {
            SmtpClient smtpClient = new SmtpClient(_configuration["Email:Host"], Convert.ToInt32(_configuration["Email:Port"]));
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(_configuration["Email:LoginEmail"], _configuration["Email:Password"]);

            MailAddress from = new MailAddress(_configuration["Email:LoginEmail"]);
            MailAddress to = new MailAddress(emailTo);

            MailMessage msg = new MailMessage(from,to);
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = isHtml;
            await smtpClient.SendMailAsync(msg);
        }
    }
}
