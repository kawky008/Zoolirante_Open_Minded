using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Zoolirante_Open_Minded.Models;
using Zoolirante_Open_Minded.Services;

namespace Zoolirante_Open_Minded.Controllers
{
    public class EntranceTicketsController : Controller
    {
        private readonly ZooliranteDatabaseContext _context;
        private readonly IEmailService _emailService;

        public EntranceTicketsController(ZooliranteDatabaseContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Buy()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Users");

            var ticket = new EntranceTicket { UserId = userId.Value };
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy(EntranceTicket ticket)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Users");

            ticket.UserId = userId.Value;

            if (ModelState.IsValid)
            {
                ticket.Price = ticket.Type == "Child" ? 20m : 30m;
                ticket.IssuedDate = DateTime.Now;
                ticket.ExpiredDate = DateTime.Now.AddMonths(1);

                _context.EntranceTickets.Add(ticket);
                await _context.SaveChangesAsync();

                var user = await _context.Users.FindAsync(userId.Value);
                if (user != null)
                {
                    
                    await _emailService.SendTicketConfirmationAsync(user, ticket);
                }

                return RedirectToAction("Confirmation", new { id = ticket.TicketId });
            }

            return View(ticket);
        }
        public async Task<IActionResult> Confirmation(int id)
        {
            var ticket = await _context.EntranceTickets.FindAsync(id);
            if (ticket == null) return NotFound();

            return View(ticket);
        }
    }
}
