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
    public class ControlPreciosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ControlPreciosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ControlPrecios
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ControlPrecios
                .Include(c => c.Producto)
                .Include(c => c.Producto.Categoria);
                
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ControlPrecios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var controlPrecio = await _context.ControlPrecios
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(m => m.ControlPrecioId == id);
            if (controlPrecio == null)
            {
                return NotFound();
            }

            return View(controlPrecio);
        }

        // GET: ControlPrecios/Create
        public IActionResult Create()
        {
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "ProductoId");
            return View();
        }

        // POST: ControlPrecios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ControlPrecioId,ProductoId,Costo,PorcentajeGanancia,PrecioVenta,CodigoBarra")] ControlPrecio controlPrecio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(controlPrecio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "ProductoId", controlPrecio.ProductoId);
            return View(controlPrecio);
        }

        // GET: ControlPrecios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var controlPrecio = await _context.ControlPrecios
                .Include(c => c.Producto)
                .Include(c  => c.Producto.Categoria)
                .FirstOrDefaultAsync(c => c.ControlPrecioId == id);
            if (controlPrecio == null)
            {
                return NotFound();
            }
            //ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "ProductoId", controlPrecio.ProductoId);
            return View(controlPrecio);
        }

        // POST: ControlPrecios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ControlPrecioId,ProductoId,Costo,PorcentajeGanancia,CodigoBarra")] ControlPrecio controlPrecio)
        {
            if (id != controlPrecio.ControlPrecioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    controlPrecio.PrecioVenta = (decimal)(controlPrecio.Costo+(controlPrecio.Costo*controlPrecio.PorcentajeGanancia)/100);
                    _context.Update(controlPrecio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ControlPrecioExists(controlPrecio.ControlPrecioId))
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
           // ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "ProductoId", controlPrecio.ProductoId);
            return View(controlPrecio);
        }

        // GET: ControlPrecios/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var controlPrecio = await _context.ControlPrecios
        //        .Include(c => c.Producto)
        //        .FirstOrDefaultAsync(m => m.ControlPrecioId == id);
        //    if (controlPrecio == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(controlPrecio);
        //}

        //// POST: ControlPrecios/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var controlPrecio = await _context.ControlPrecios.FindAsync(id);
        //    if (controlPrecio != null)
        //    {
        //        _context.ControlPrecios.Remove(controlPrecio);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool ControlPrecioExists(int id)
        {
            return _context.ControlPrecios.Any(e => e.ControlPrecioId == id);
        }
    }
}
