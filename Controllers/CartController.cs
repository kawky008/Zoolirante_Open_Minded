using Microsoft.AspNetCore.Mvc;
using Zoolirante_Open_Minded.ViewModels;
using Zoolirante_Open_Minded.Helpers;
using Zoolirante_Open_Minded.Models;   
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;   
using Microsoft.EntityFrameworkCore;         
using System.Data;                           


namespace Zoolirante_Open_Minded.Controllers
{
    public class CartController : Controller
    {
        private const string CartKey = "CART_V1";
        private readonly ZooliranteDatabaseContext _db;  


        public CartController(ZooliranteDatabaseContext db)
        {
            _db = db;
        }

        private async Task<List<SelectListItem>> LoadPickupOptionsAsync()
        {
            var items = new List<SelectListItem>();
            var conn = _db.Database.GetDbConnection();

            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
        SELECT PickupLocationId, Name
        FROM dbo.PickupLocations
        WHERE IsActive = 1
        ORDER BY Name";
            using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            while (await reader.ReadAsync())
            {
                items.Add(new SelectListItem
                {
                    Value = reader.GetInt32(0).ToString(),
                    Text = reader.GetString(1)
                });
            }
            return items;
        }

		[HttpGet]
		public async Task<IActionResult> Index()
		{

			var cart = HttpContext.Session.GetObject<CartVM>(CartKey) ?? new CartVM();

			var uid = HttpContext.Session.GetInt32("UserId");
			var isMember = false;
			if (uid.HasValue)
			{
				var now = DateTime.UtcNow;
				isMember = await _db.Memberships.AnyAsync(m => m.UserId == uid.Value && m.EndDate > now);
			}

			foreach (var it in cart.Items)
			{
				if (it.OriginalPrice <= 0) it.OriginalPrice = it.Price;
				it.Price = isMember ? it.OriginalPrice * 0.90m : it.OriginalPrice;
			}
			HttpContext.Session.SetObject(CartKey, cart);

			return View(cart);
		}


		public async Task<IActionResult> Checkout()
        {
            var cart = HttpContext.Session.GetObject<CartVM>(CartKey) ?? new CartVM();
            ViewBag.PickupOptions = await LoadPickupOptionsAsync(); // for the dropdown
            return View(cart); // Views/Cart/Checkout.cshtml
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPickup(int id)
        {
            var cart = HttpContext.Session.GetObject<CartVM>(CartKey) ?? new CartVM();

            
            string? name = null;
            var conn = _db.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
            SELECT Name
            FROM dbo.PickupLocations
            WHERE PickupLocationId = @id AND IsActive = 1";
                var p = cmd.CreateParameter();
                p.ParameterName = "@id";
                p.Value = id;
                cmd.Parameters.Add(p);

                var result = await cmd.ExecuteScalarAsync();
                name = result as string;
            }
            await conn.CloseAsync();

            if (name == null) return NotFound(); // invalid id / inactive

            cart.PickupLocationId = id;
            cart.PickupLocationName = name;
            HttpContext.Session.SetObject(CartKey, cart);

            TempData["CartMessage"] = $"Pickup location set: {name}";
            return RedirectToAction(nameof(Checkout));
        }



        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Update(int productId, int qty)
        {
            var cart = HttpContext.Session.GetObject<CartVM>(CartKey) ?? new CartVM();
            var line = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (line != null)
            {
                if (qty <= 0)
                {
                    cart.Items.Remove(line);
                }
                else
                {
                    
                    var stock = _db.Merchandises
                                   .Where(p => p.ProductId == productId)
                                   .Select(p => p.Stock)
                                   .FirstOrDefault();
                    if (stock > 0)
                        line.Qty = Math.Min(qty, stock);
                    else
                        line.Qty = Math.Max(1, qty);
                }
            }
            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Remove(int productId)
        {
            var cart = HttpContext.Session.GetObject<CartVM>(CartKey) ?? new CartVM();
            cart.Items.RemoveAll(i => i.ProductId == productId);
            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            HttpContext.Session.SetObject(CartKey, new CartVM());
            return RedirectToAction(nameof(Index));
        }
    }
}
