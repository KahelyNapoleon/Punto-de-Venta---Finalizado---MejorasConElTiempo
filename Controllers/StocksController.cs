using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POS.Data;
using POS.Models;

namespace POS.Controllers
{
    public class StocksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StocksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Stocks
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Stocks
                .Include(s => s.Producto)
                .ToListAsync();
            return View(await applicationDbContext);
        }

        // GET: Stocks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stock = await _context.Stocks
                .Include(s => s.Producto)
                .FirstOrDefaultAsync(m => m.StockId == id);

            if (stock == null)
            {
                return NotFound();
            }

            return View(stock);
        }

        // GET: Stocks/Create
        //public IActionResult Create()
        //{
        //    ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "ProductoId");
        //    return View();
        //}

        // POST: Stocks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("StockId,ProductoId,Cantidad,CodigoBarra")] Stock stock)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(stock);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "ProductoId", stock.ProductoId);
        //    return View(stock);
        //}

        // GET: Stocks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stock = await _context.Stocks
                .Include(s=>s.Producto)
                .FirstOrDefaultAsync(s=>s.StockId == id);


            if (stock == null)
            {
                return NotFound();
            }
            //ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "ProductoId", stock.ProductoId);
            return View(stock);
        }

        // POST: Stocks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StockId,ProductoId,Cantidad,CodigoBarra")] Stock stock)
        {
            if (id != stock.StockId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockExists(stock.StockId))
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
           // ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "ProductoId", stock.ProductoId);
            return View(stock);
        }



        // GET: Stocks/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var stock = await _context.Stocks
        //        .Include(s => s.Producto)
        //        .FirstOrDefaultAsync(m => m.StockId == id);
        //    if (stock == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(stock);
        //}

        //// POST: Stocks/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var stock = await _context.Stocks.FindAsync(id);
        //    if (stock != null)
        //    {
        //        _context.Stocks.Remove(stock);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool StockExists(int id)
        {
            return _context.Stocks.Any(e => e.StockId == id);
        }
    }
}
