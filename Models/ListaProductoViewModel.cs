using System.ComponentModel.DataAnnotations;

namespace POS.Models
{
    public class ListaProductoViewModel
    {
        public string CodigoBarra { get; set; }
        public string Nombre { get; set; }
        public int PesoNeto { get; set; }
        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal { get; set; }

       
    }
}
