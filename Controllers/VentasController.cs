using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POS.Data;
using POS.Extension;
using POS.Models;

namespace POS.Controllers
{
    public class VentasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VentasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ventas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Venta.ToListAsync());
        }


        // GET: AgregarProducto
        public ActionResult AgregarProducto()
        {
            var ListaProductos = HttpContext.Session.GetObjectFromJson<List<ListaProductoViewModel>>("ListaProductos") ?? new List<ListaProductoViewModel>();

            return View("Vender", ListaProductos);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarProducto(string codigoBarra)
        {
            // Se toman los registros de productos de la Session (un espacio de memoria temporal del servidor)
            // Este espacio de memoria temporal del servidor tiene como identificador de la sesion de usuario
            // un tipo [key,value] o tokens donde reconoce las solicitudes del cliente 
            var ListaProductos = HttpContext.Session.GetObjectFromJson<List<ListaProductoViewModel>>("ListaProductos") ?? new List<ListaProductoViewModel>();

            // Se busca un producto por el parametro codigoBarra
            var producto = await _context.Productos
                .Include(p => p.ControlPrecio)
                .Include(p => p.Stock)
                .FirstOrDefaultAsync(p => p.CodigoBarra == codigoBarra);

            
            //Se verifica que no sea nulo
            if (producto != null)
            {
                //ListaProductos es una variable que almacena el modelo de ProductoVenta(Un view model, NO es de La BD)
                //productoExiste busca el primer producto de la sesion temporal que concioda con el codigo de barra ingresado como 
                // parametro de valor de la funcion AgregarProducto

                //Se recupera un producto de una lista en caso de que haya sido ingresado con anterioridad
                var productoExiste = ListaProductos.FirstOrDefault(p => p.CodigoBarra == codigoBarra);

                //    CODIGO DE PRUEBA ELIMINAR!!
                //var stockCantidad = stockProducto.Cantidad;
                
                //Si se agrego con anterioridad se procede a lo siguiente
                if (productoExiste != null)
                {
                    // !!  CODIGO DE PRUEBA: VERIFICAR Y VALIDAR CANTIDAD EN STOCK!!
                    //if ((stockCantidad - productoExiste.Cantidad) > 0)
                    //{
                    //    //SI EL PRODUCTO YA SE ENCUENTRA EN LA LISTA TEMPORAL, SE LE SUMA LA CANTIDAD EN 1
                    //    productoExiste.Cantidad++;
                    //    productoExiste.Subtotal = productoExiste.Cantidad * productoExiste.PrecioUnitario;
                    //}
                    //else
                    //{
                    //    return BadRequest("La cantidad excede el stock disponible");
                    //}

                    //CODIGO DE PRUEBA 2 CON METODO QUE DEVUELVE TRUE O FALSE SI LA CANTIDAD SOLICITADA ES VALIDA

                        bool validarStock = ValidarStock(productoExiste.CodigoBarra, productoExiste.Cantidad + 1);
                        if (validarStock)
                        {
                            productoExiste.Cantidad++;
                            productoExiste.Subtotal = productoExiste.Cantidad * productoExiste.PrecioUnitario;
                        }
                        else
                        {
                            TempData["StockInsuficiente"] = true; //SE PUEDE USAR TEMPDATA PARA ENVIAR INFORMACION A LA VISTA
                                                                  //Y ASI PODER ACTIVAR ALGUN EVENTO EN LA VISTA EN ESTE CASP 
                        }                                         //SI ES TRUE MUESTRA UNA VENTANA EMERGENTE DE ERROR


                    //SI EL PRODUCTO YA SE ENCUENTRA EN LA LISTA TEMPORAL, SE LE SUMA LA CANTIDAD EN 1
                    //productoExiste.Cantidad++;
                    //productoExiste.Subtotal = productoExiste.Cantidad * productoExiste.PrecioUnitario;

                }
                //Si no se agrego con anterioridad se procede a ingresarlo.
                else
                {
                    bool validarStock = ValidarStock(codigoBarra, 1);
                    if (validarStock)
                    {
                        ListaProductos.Add(new ListaProductoViewModel
                        {
                            CodigoBarra = producto.CodigoBarra,
                            Nombre = producto.Nombre,
                            PesoNeto = producto.PesoNeto,
                            PrecioUnitario = producto.ControlPrecio.PrecioVenta,
                            Cantidad = 1,
                            Subtotal = producto.ControlPrecio.PrecioVenta
                        });
                    }
                    else
                    {
                        TempData["StockInsuficiente"] = true;
                    }
                    
                  

                 

                }

                HttpContext.Session.SetObjectAsJson("ListaProductos", ListaProductos);
            }


            return View("Vender", ListaProductos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //FUNCIONES AL FINALIZAR COMPRA SON: 
        //1) RESTAR LA CANTIDAD DE LOS PRODUCTOS DE LA TABLA STOCK
        //2) EN CADA FINALIZACION DE COMPRA SE DEBEN AGREGAR LOS DETALLES DE LA VENTA
        public async Task<IActionResult> FinalizarCompra()
        {
            //Variables
            var ListaProductos = HttpContext.Session.GetObjectFromJson<List<ListaProductoViewModel>>("ListaProductos") ?? new List<ListaProductoViewModel>();
            //Variable que recupera todos los codigos de barra de la Lista de la Session producida por la venta
            var codigosBarraVenta = ListaProductos.Select(l => l.CodigoBarra).ToList();
          
            //En Caso que no exista ningun producto en la Lista de Venta
            if (ListaProductos == null || !ListaProductos.Any())
            {
                return BadRequest("No hay productos en la lista de venta");
                //return View("error", new { message = "No hay productos para finalizar la compra" });

            }
            //Caso2: Existe al menos 1 producto se procede a Registrar el Total de la Compra y La Fecha y la Hora.
            else
            {
                //PROCEDIMIENTO PARA el Modelo de VENTA -------------------------1
                var totalCompra = ListaProductos.Sum(p => p.Subtotal);

                var registrarVenta = new Venta
                {
                    FechaVenta = DateTime.Now,
                    Total = (decimal)totalCompra
                    
                };
                _context.Venta.Add(registrarVenta);
                await _context.SaveChangesAsync();

                //PROCEDIMIENTO PARA el Modelo de DetalleVenta -------------------------2

                //Condiciona, mapeando la tabla productos de la BD, cada articulo que coincida con el codigo de barra de la venta,
                //devolviendo una lista.
                var productos = await _context.Productos
                    .Where(p => codigosBarraVenta.Contains(p.CodigoBarra))
                    .ToListAsync();
               

                
                //Se crea una lista de DetalleVentas mapeando ListaProductos que es de tipo Session.
                var detalleVenta = ListaProductos
                    .Select(l =>  //select mapea cada producto de session y devuelve un objeto de 
                    {             //detellaVenta para el registro de ventas.

                        //lo que guarda 
                        var producto = productos.FirstOrDefault(p => p.CodigoBarra == l.CodigoBarra);

                        return new DetalleVenta
                        {
                            VentaId = registrarVenta.VentaId,
                            ProductoId = producto.ProductoId,
                            Cantidad = l.Cantidad,
                            PrecioUnitario = l.PrecioUnitario,
                            Subtotal = l.Subtotal,
                            CodigoBarra = l.CodigoBarra
                        };
                    }).ToList();
                    
                //SE GUARDA LA LISTA DE PRODUCTOS GENERADOS EN LA VENTA, UNA VEZ FINALIZADA, EN LA BASE DE DATOS
                //COMO DETALLEVENTA
                _context.DetalleVenta.AddRange(detalleVenta);
                await _context.SaveChangesAsync();

                //------------------CONTROLAR QUE A LA HORA DE INGRESAR NUEVA CANTIDAD EN STOCK DEL PRODUCTO----------------------------
                //-----DIGAMOS QUE SE VENDIERON 12 Y HABIA 12 EN STOCK, LUEGO SE AGREGO NUEVA CANTIDAD DE STOCK EN EL PRODUCTO VENDIDO--
                //-----PERO LA SESSION PARA EJECUTAR LA VENTA NO LO IDENTIFICA----------------------------------------------------------

                //3ER PROCEDIMIENTO OBLIGATORIO - RESTAR LA CANTIDAD VENDIDA DE CADA ARTICULO DE LA VENTA
                //                                DEL REGISTRO DEL STOCK.

                //LISTA DE CODIGOS DE BARRAS VENDIDOS
                var codigosVendidos = ListaProductos.Select(l=>l.CodigoBarra).ToList();

                //STOCKS DE LOS CODIGOS DE BARRAS VENDIDOS
                var productosStock = await _context.Stocks
                    .Where(s => codigosVendidos.Contains(s.CodigoBarra)).ToListAsync();

                foreach (var l in ListaProductos)
                {
                    var producto = productosStock.FirstOrDefault(s => s.CodigoBarra == l.CodigoBarra);

                    if (producto != null)
                    {
                        producto.Cantidad -= l.Cantidad;

                        _context.Stocks.Update(producto);
                    }
                    else
                    {
                        return BadRequest("El producto No Existe");
                    }
                }

                await _context.SaveChangesAsync();


                //-------------------------------------------------------------------------------------------

               
                HttpContext.Session.Remove("ListaProductos");

                return RedirectToAction("AgregarProducto");
            }
            // return View("Vender", ListaProductos);
        }


        public ActionResult EditarCantidad(string? codigoBarra)
        {

            if (codigoBarra == null)
            {
                return NotFound();
            }

            var listaProductos = HttpContext.Session.GetObjectFromJson<List<ListaProductoViewModel>>("ListaProductos") ?? new List<ListaProductoViewModel>();
            var productoEditar = listaProductos.FirstOrDefault(p => p.CodigoBarra == codigoBarra);

            if (productoEditar == null)
            {
                return NotFound();
            }

            return View("_listaProductos", productoEditar);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarCantidad(string codigoBarra, int cantidad)
        {
            var ListaProductos = HttpContext.Session.GetObjectFromJson<List<ListaProductoViewModel>>("ListaProductos") ?? new List<ListaProductoViewModel>();
            var productoModificar = ListaProductos.FirstOrDefault(p=> p.CodigoBarra == codigoBarra);

            if (productoModificar != null)
            {
                if (ValidarStock(codigoBarra, cantidad))
                {
                    productoModificar.Cantidad = cantidad;
                    productoModificar.Subtotal = productoModificar.PrecioUnitario * cantidad;
                    HttpContext.Session.SetObjectAsJson("ListaProductos", ListaProductos);
                }
                else
                {
                    TempData["StockInsuficiente"] = true;
                }
              
            }
            else
            {
                return NotFound();
            }
            return RedirectToAction(nameof(AgregarProducto));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarProductoDeLista(string codigoBarra)
        {
            var ListaProductos = HttpContext.Session.GetObjectFromJson<List<ListaProductoViewModel>>("ListaProductos") ?? new List<ListaProductoViewModel>();
            var productoEliminar = ListaProductos.FirstOrDefault(p => p.CodigoBarra == codigoBarra);

            if (productoEliminar != null)
            {
                ListaProductos.Remove(productoEliminar);

                HttpContext.Session.SetObjectAsJson("ListaProductos", ListaProductos);
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction(nameof(AgregarProducto));
        }















        // GET: Ventas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Venta
                .Include(v=>v.DetalleVenta)
                .ThenInclude(d=>d.Producto.Categoria)
                .FirstOrDefaultAsync(m => m.VentaId == id);

            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

       

        // GET: Ventas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Venta
                .Include(v => v.DetalleVenta)
                .FirstOrDefaultAsync(m => m.VentaId == id);
            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // POST: Ventas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venta = await _context.Venta
                .Include(v => v.DetalleVenta)
                .FirstOrDefaultAsync(v=>v.VentaId == id);
           
            if (venta != null)
            {
                _context.DetalleVenta.RemoveRange(venta.DetalleVenta);
                _context.Venta.Remove(venta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //------------------------------METODOS AUXILIARES--------------------------------------------------------
        
        private bool VentaExists(int id)
        {
            return _context.Venta.Any(e => e.VentaId == id);
        }


        public bool ValidarStock(string codigoBarra, int cantidadSolicitada)
        {
            //El metodo debe retornar false en caso que la cantidad de stock requerida no se asuficiente
            //De lo contrario retorna true

            var producto = _context.Stocks.FirstOrDefault(s => s.CodigoBarra == codigoBarra);

            if (producto == null)
            {
                return false;
            }

            if(producto.Cantidad < cantidadSolicitada)
            {   
                return false;
            }
     
                return true;            

        }


        public static void RestarStockVendido(List<ProductoViewModel> listaVendida)
        {
           
        }




    }
}
