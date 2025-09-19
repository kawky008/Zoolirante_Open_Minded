using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zoolirante_Open_Minded.Models;

namespace Zoolirante_Open_Minded.Controllers
{
    public class AnimalFavouriteController : Controller
    {
        private readonly ZooliranteDatabaseContext _context;

        public AnimalFavouriteController(ZooliranteDatabaseContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> UserFavorites(int userId)
        {
            var favourites = await _context.AnimalFavourite
                .Include(f => f.Animal)
                .Where(f => f.UserId == userId)
                .ToListAsync();

            return View(favourites);
        }

       
        [HttpPost]
        public async Task<IActionResult> AddFavourite(int userId, int animalId)
        {
            var exists = await _context.AnimalFavourite
                .AnyAsync(f => f.UserId == userId && f.AnimalId == animalId);

            if (!exists)
            {
                _context.AnimalFavourite.Add(new AnimalFavourite
                {
                    UserId = userId,
                    AnimalId = animalId
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("UserFavourites", new { userId });
        }

        
        [HttpPost]
        public async Task<IActionResult> RemoveFavourite(int userId, int animalId)
        {
            var favourite = await _context.AnimalFavourite
                .FirstOrDefaultAsync(f => f.UserId == userId && f.AnimalId == animalId);

            if (favourite != null)
            {
                _context.AnimalFavourite.Remove(favourite);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("UserFavourites", new { userId });
        }
    }
}
