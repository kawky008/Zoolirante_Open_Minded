using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Zoolirante_Open_Minded.Models;
namespace Zoolirante_Open_Minded.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendTicketConfirmationAsync(User user, EntranceTicket ticket);
    }
    public class Email: IEmailService
    {
        private readonly IConfiguration _config;

        public Email(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var fromEmail = _config["EmailSettings:FromEmail"];
            var fromPassword = _config["EmailSettings:FromPassword"];

            var message = new MailMessage();
            message.To.Add(new MailAddress(toEmail));
            message.From = new MailAddress(fromEmail);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.Credentials = new NetworkCredential(fromEmail, fromPassword);
                client.EnableSsl = true;
                await client.SendMailAsync(message);
            }
        }
        public async Task SendTicketConfirmationAsync(User user, EntranceTicket ticket)
        {
            string subject = "Ticket Purchase Confirmation";
            string body = $@"
<h2>🎟 Ticket Confirmation</h2>
<p>Hi {user.FullName},</p>
<ul>
    <li><strong>Type:</strong> {ticket.Type}</li>
    <li><strong>Price:</strong> ${ticket.Price}</li>
    <li><strong>Issued:</strong> {ticket.IssuedDate:dd/MM/yyyy}</li>
    <li><strong>Expires:</strong> {ticket.ExpiredDate:dd/MM/yyyy}</li>
</ul>
<p>Thank you for visiting <b>Zoolirante Open-Minded</b>!</p>
";
            await SendEmailAsync(user.Email, subject, body);
        }
    }
}

    

    

