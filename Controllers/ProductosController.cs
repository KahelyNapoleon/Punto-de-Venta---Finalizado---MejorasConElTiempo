using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POS.Data;
using POS.Models;

namespace POS.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        //// GET: Productos
        //public async Task<IActionResult> Index()
        //{
        //    var applicationDbContext = _context.Productos.Include(p => p.Categoria);
        //    return View(await applicationDbContext.ToListAsync());
        //}

        //Filtro de BUSQUEDAS PARA LOS PRODUCTOS POR CONSULTAS DE CODIGOBARRA, CATEGORIA Y NOMBRE
        public async Task<IActionResult> Index(string BuscarNombre, string BuscarCategoria, string BuscarCodigoBarra)
        {
            if (_context.Productos == null)
            {
                return Problem("Entity Set 'ApplicationDbContext.Productos' is null");
            }

            var productos = (from p in _context.Productos.Include(p => p.Categoria)
                             where (String.IsNullOrEmpty(BuscarCategoria) || p.Categoria.Descripcion.ToUpper().Contains(BuscarCategoria.ToUpper()))
                             && (String.IsNullOrEmpty(BuscarCodigoBarra) || p.CodigoBarra.ToUpper().Contains(BuscarCodigoBarra.ToUpper()))
                             && (String.IsNullOrEmpty(BuscarNombre) || p.Nombre.ToUpper().Contains(BuscarNombre.ToUpper()))
                             select p).ToListAsync();

            return View(await productos);
        }


        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.ProductoId == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }



        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ProductoId,CodigoBarra,Nombre,PesoNeto,CategoriaId")] Producto producto)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(producto);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CategoriaId"] = new SelectList(_context.Categoria, "CategoriaId", "Descripcion", producto.CategoriaId);
        //    return View(producto);
        //}

        // GET: Productos/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "CategoriaId", "Descripcion");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        //Agrega datos de producto a la BD
                        var producto = new Producto
                        {
                            CodigoBarra = model.CodigoBarra,
                            Nombre = model.Nombre,
                            PesoNeto = (int)model.PesoNeto,
                            CategoriaId = model.CategoriaId

                        };
                        _context.Add(producto);
                        await _context.SaveChangesAsync();

                        //Id de producto
                        model.ProductoId = producto.ProductoId;

                        //Agrega datos de ControlPrecio al DB
                        var controlPrecio = new ControlPrecio
                        {
                            ProductoId = model.ProductoId,
                            Costo = model.Costo,
                            PorcentajeGanancia = model.PorcentajeGanancia,
                            PrecioVenta = (decimal)(model.Costo + ((model.Costo * model.PorcentajeGanancia) / 100)),
                            CodigoBarra = model.CodigoBarra
                        };
                        _context.Add(controlPrecio);
                        await _context.SaveChangesAsync();

                        //Agrega datos de Stock al DB
                        var stock = new Stock
                        {
                            ProductoId = model.ProductoId,
                            Cantidad = model.Cantidad,
                            CodigoBarra = model.CodigoBarra
                        };
                        _context.Add(stock);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return RedirectToAction("Index");

                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        ModelState.AddModelError("", "Ocurrio un Error");
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            //foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
            //{
            //    Console.WriteLine(modelError.ErrorMessage);
            //}
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "CategoriaId", "Descripcion");
            return View(model);
        }



        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.ProductoId == id);


            if (producto == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "CategoriaId", "Descripcion", producto.CategoriaId);
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductoId,CodigoBarra,Nombre,PesoNeto,CategoriaId")] Producto producto)
        {
            if (id != producto.ProductoId)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                //ModelState.Remove("CategoriaId");
                var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                // Inspecciona 'errors' para ver qué campos están fallando
            }
            //Hubo un problema en la validacion de las propiedades Bind con las propiedades de la clase Producto, el controlador intentaba validar 
            //dos veces categorias, CategoriaId y Categoria, esta ultima ASP.NET MVC la trataba como una mas
            //de las propiedades a validar por ser Virutal

            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.ProductoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            }


            //foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
            //{
            //    Console.WriteLine(modelError.ErrorMessage);
            //}


            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "CategoriaId", "Descripcion", producto.CategoriaId);
            return View(producto);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.ProductoId == id);
            if (producto == null)
            {
                return NotFound(); //NOTFOUND
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // SE ESTABAN GENERANDO LOS CAMBIOS PARA ELIMINAR LAS ENTIDADES RELACIONADAS AL PRODUCTO

            //ACA SE INCLUYEN LAS ENTIDADES Y LUEGO SE DEBEN ELIMINAR LOS REGISTROS RELACIONADOS 
            var producto = await _context.Productos //Con metodos de seleccion se busca en la base de datos(_context) la relacion del producto con stock y controlPrecio
                .Include(p => p.Stock)
                .Include(p => p.ControlPrecio)
                .FirstOrDefaultAsync(p => p.ProductoId == id);

            if (producto != null)
            {
                //ACA SE ELIMINAN LOS REGISTROS RELACIONADOS 
                _context.Stocks.RemoveRange(producto.Stock);
                _context.ControlPrecios.RemoveRange(producto.ControlPrecio);
                //Corregir, ala hora de eliminar un producto y estar relacionado con la tabla DetalleVenta 
                //La accion de eliminar lanza un error 
                //POSIBLE SOLUCION: Eliminar la clave foranea de la tabla DetalleVenta con la tabla Producto
                _context.Productos.Remove(producto);
            }


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.ProductoId == id);
        }
    }
}
