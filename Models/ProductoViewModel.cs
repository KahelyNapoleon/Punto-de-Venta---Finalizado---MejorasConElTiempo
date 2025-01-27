using System.ComponentModel.DataAnnotations;

namespace POS.Models
{
    public class ProductoViewModel
    {
        //Producto
        public int ProductoId { get; set; }
        [Required(ErrorMessage = "El código de barra es obligatorio.")]
        [StringLength(13, ErrorMessage = "El código de barra debe tener exactamente 13 caracteres.", MinimumLength = 13)]
        [RegularExpression("^[0-9]+$", ErrorMessage = "El código de barra debe contener solo números.")]
        public string CodigoBarra { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public int? PesoNeto { get; set; }
        public int CategoriaId { get; set; }

    

        //ControlPrecio
        public int ControlPrecioId { get; set; }
        public decimal Costo { get; set; }
        public decimal PorcentajeGanancia { get; set; }
        public decimal PrecioVenta { get; set; }

        //Stock
        public int StockId { get; set; }
        public int Cantidad { get; set; }



    }
}
