using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zoolirante_Open_Minded.Models;

namespace Zoolirante_Open_Minded.Controllers
{	public class ToggleFavoriteDto
	{
		public int AnimalId { get; set; }
	}

	[Route("api/[controller]")]
	[ApiController]
	public class AnimalFavouriteController : Controller
    {
        private readonly ZooliranteDatabaseContext _context;

        public AnimalFavouriteController(ZooliranteDatabaseContext context)
        {
            _context = context;
        }

		[HttpPost("Toggle")]
		public IActionResult Toggle([FromBody] ToggleFavoriteDto fav)
		{
			var userId = HttpContext.Session.GetInt32("UserId");
			if (userId == null)
			{
				return Unauthorized();
			}

			var existing = _context.AnimalFavourite
				.FirstOrDefault(f => f.UserId == userId && f.AnimalId == fav.AnimalId);

			if (existing != null)
			{
				_context.AnimalFavourite.Remove(existing);
				_context.SaveChanges();
				return Ok(new { added = false });
			}
			else
			{
				var newFav = new AnimalFavourite
				{
					UserId = userId.Value,
					AnimalId = fav.AnimalId
				};
				_context.AnimalFavourite.Add(newFav);
				_context.SaveChanges();
				return Ok(new { added = true });
			}
		}

		[HttpGet("UserFavorites")]
		public IActionResult UserFavorites()
		{
			var userId = HttpContext.Session.GetInt32("UserId");
			if (userId == null)
				return Unauthorized();

			var favorites = _context.AnimalFavourite
				.Where(f => f.UserId == userId)
				.Select(f => f.AnimalId)
				.ToList();

			return Ok(favorites);
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
