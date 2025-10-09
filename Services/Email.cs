using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Zoolirante_Open_Minded.Models;
namespace Zoolirante_Open_Minded.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendTicketConfirmationAsync(User user, EntranceTicket ticket);
        Task BookingReminder(ZooliranteDatabaseContext context, User user, EntranceTicket ticket);
    }
    public class Email : IEmailService
    {
        private readonly IConfiguration _config;

        public Email(IConfiguration config)
        {
            _config = config;
        }
        public async Task BookingReminder(ZooliranteDatabaseContext context, User user, EntranceTicket ticket)
        {
            var startOfDay = ticket.VisitDate;
            var endOfDay = startOfDay.AddDays(1);

            var events = await context.Events
                //.Where(e => e.StartTime >= startOfDay && e.StartTime < endOfDay)
                .OrderBy(e => e.StartTime)
                .ToListAsync();


            string eventsHtml = "";

            eventsHtml = "<h3>Today's Events:</h3><ul>";
            foreach (var e in events)
            {
                eventsHtml += $"<li><strong>{e.Title}</strong> - {e.StartTime:HH:mm} to {e.EndTime:HH:mm}</li>";
            }
            eventsHtml += "</ul>";




            string subject = "🎫 Your Zoolirante Visit is Coming Soon!";

            string body = $@"
<h2>Hi {user.FullName},</h2>
<p>Just a friendly reminder that your visit to <b>Zoolirante Open-Minded</b> is coming up soon!</p>

<h3>Your Ticket Details:</h3>
<ul>
    <li><strong>Type:</strong> {ticket.Type}</li>
    <li><strong>Price:</strong> ${ticket.Price}</li>
    <li><strong>Issued:</strong> {ticket.CreatedAt:dd/MM/yyyy}</li>
    <li><strong>Visit Date:</strong> {ticket.VisitDate:dd/MM/yyyy}</li>
</ul>

{eventsHtml}

<p>We can't wait to see you and hope you enjoy the amazing events we have lined up for today! 🐾</p>
<p>Thank you for choosing <b>Zoolirante Open-Minded</b>!</p>
<p>See you soon!</p>
";


            await SendEmailAsync(user.Email, subject, body);
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
    <li><strong>Issued:</strong> {ticket.CreatedAt:dd/MM/yyyy}</li>
    <li><strong>Expires:</strong> {ticket.ExpiredAt:dd/MM/yyyy}</li>
</ul>
<p>Thank you for visiting <b>Zoolirante Open-Minded</b>!</p>

";
            await SendEmailAsync(user.Email, subject, body);
        }
    }
}





