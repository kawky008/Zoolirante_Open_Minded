using Microsoft.AspNetCore.Mvc;
using Zoolirante_Open_Minded.ViewModels;
using Zoolirante_Open_Minded.Helpers;
using Zoolirante_Open_Minded.Models;   
using System.Linq;

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

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<CartVM>(CartKey) ?? new CartVM();
            return View(cart);
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
