using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zoolirante_Open_Minded.Helpers;
using Zoolirante_Open_Minded.Models;
using Zoolirante_Open_Minded.ViewModels;

namespace Zoolirante_Open_Minded.Controllers
{
    public class MerchandisesController : Controller
    {
        private readonly ZooliranteDatabaseContext _context;

        public MerchandisesController(ZooliranteDatabaseContext context)
        {
            _context = context;
        }

        // GET: Merchandises
        public async Task<IActionResult> Index(string searchText)
        {
			ViewData["BannerText"] = "View our merchandises";
			var q = _context.Merchandises.AsQueryable();

			if (!string.IsNullOrWhiteSpace(searchText))
				q = q.Where(i => i.Name.StartsWith(searchText));
			return View(await q.ToListAsync());
        }

        // GET: Merchandises/Details/5
        public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
            {
                return NotFound();
            }

            var merchandise = await _context.Merchandises
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (merchandise == null)
            {
                return NotFound();
            }

            return View(merchandise);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int id, int qty = 1)
        {
            var p = await _context.Merchandises.FirstOrDefaultAsync(x => x.ProductId == id);
            if (p == null) return NotFound();

            const string CartKey = "CART_V1";
            var cart = HttpContext.Session.GetObject<CartVM>(CartKey) ?? new CartVM();

            var line = cart.Items.FirstOrDefault(i => i.ProductId == id);
            if (line == null)
            {
                cart.Items.Add(new CartItemVM
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    Qty = Math.Max(1, Math.Min(qty, p.Stock)),
                    ImageUrl = p.ImageUrl
                });
            }
            else
            {
                var max = Math.Max(1, p.Stock);
                line.Qty = Math.Min(max, line.Qty + Math.Max(1, qty));
            }

            HttpContext.Session.SetObject(CartKey, cart);
            TempData["CartMessage"] = $"Added {p.Name} (x{qty})";

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrWhiteSpace(referer)) return Redirect(referer);
            return RedirectToAction(nameof(Index));
        }
                                     
            

        // GET: Merchandises/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Merchandises/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Name,Description,Price,Stock,ImageUrl,Category")] Merchandise merchandise)
        {
            if (ModelState.IsValid)
            {
                _context.Add(merchandise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(merchandise);
        }

        // GET: Merchandises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var merchandise = await _context.Merchandises.FindAsync(id);
            if (merchandise == null)
            {
                return NotFound();
            }
            return View(merchandise);
        }

        // POST: Merchandises/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Description,Price,Stock,ImageUrl,Category")] Merchandise merchandise)
        {
            if (id != merchandise.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(merchandise);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MerchandiseExists(merchandise.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(merchandise);
        }

        // GET: Merchandises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var merchandise = await _context.Merchandises
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (merchandise == null)
            {
                return NotFound();
            }

            return View(merchandise);
        }

        // POST: Merchandises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var merchandise = await _context.Merchandises.FindAsync(id);
            if (merchandise != null)
            {
                _context.Merchandises.Remove(merchandise);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MerchandiseExists(int id)
        {
            return _context.Merchandises.Any(e => e.ProductId == id);
        }
    }
}


