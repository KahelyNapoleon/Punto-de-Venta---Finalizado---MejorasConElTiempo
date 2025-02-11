using System.ComponentModel.DataAnnotations;

namespace POS.Models
{
    public class ListaProductoViewModel
    {
        [Required]
        [Display(Name = "Codigo de Barra")]
        public string? CodigoBarra { get; set; }
        [Required]
        public string? Nombre { get; set; }
        [Display(Name = "Peso Neto")]
        public int PesoNeto { get; set; }
        public int Cantidad { get; set; }
        [Display(Name = "Precio Unitario")]
        public decimal PrecioUnitario { get; set; }
        [Display(Name = "SubTotal")]
        public decimal Subtotal { get; set; }

       
    }
}
