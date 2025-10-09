using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Zoolirante_Open_Minded.Models;
namespace Zoolirante_Open_Minded.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendTicketConfirmationAsync(User user, EntranceTicket ticket);
        Task BookingReminder(ZooliranteDatabaseContext context, User user, EntranceTicket ticket, List<string> animalFavourite);
    }
    public class Email : IEmailService
    {
        private readonly IConfiguration _config;
        private object favouriteAnimals;

        public Email(IConfiguration config)
        {
            _config = config;
        }
        public async Task BookingReminder(ZooliranteDatabaseContext context, User user, EntranceTicket ticket, List<string> animalFavourite)
        {


            var events = await context.Events.Include(e=> e.EventAnimal)
    .Where(e => e.StartTime.Date == ticket.VisitDate.Date)
    .OrderBy(e => e.StartTime)
    .ToListAsync();


            string eventsHtml;
            if (events.Any() )
            {
                eventsHtml = "<h3>Events on visit day:</h3>";
                foreach (var e in events)
                {
                    eventsHtml += $"<li><strong>{e.Title}</strong> - {e.StartTime:HH:mm} to {e.EndTime:HH:mm} animals: {e.EventAnimal.Animals}</li>";
                    string[] animalEvent = e.EventAnimal.Animals.Split(",");

                    List<string> commonAnimals = new List<string>();

                    foreach (var animal in animalEvent)
                    {
                        var trimmedAnimal = animal.Trim(); 
                        foreach (var fav in animalFavourite)
                        {
                            if (string.Equals(trimmedAnimal, fav, StringComparison.OrdinalIgnoreCase))
                            {
                                
                                if (!commonAnimals.Contains(trimmedAnimal, StringComparer.OrdinalIgnoreCase))
                                {
                                    commonAnimals.Add(trimmedAnimal);
                                }
                            }
                        }
                    }

                    if (commonAnimals.Count() > 0)
                    {
                        eventsHtml += $" (This event has your favourite animals</strong>: {string.Join(", ", commonAnimals)})";
                    }
                           
                }
                eventsHtml += "</ul>";
               
            }
            else
            {
                
                eventsHtml = "<p>There are no events related to your favourite animals on your visit day.</p>";

            }

            
            var favouriteAnimalNames = string.Join(", ", animalFavourite);
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

<p>Your favourite animals: {favouriteAnimalNames}</p>

{eventsHtml}

<p>We can't wait to see you and hope you enjoy the amazing events we have lined up! 🐾</p>
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





