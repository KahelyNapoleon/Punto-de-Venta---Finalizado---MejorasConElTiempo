using System.ComponentModel.DataAnnotations;

namespace POS.Models
{
    public class ProductoViewModel
    {
        //Producto
        public int ProductoId { get; set; }
        [Required(ErrorMessage = "El código de barra es Requerido.")]
        [StringLength(13, ErrorMessage = "El código de barra debe tener exactamente 13 caracteres.", MinimumLength = 13)]
        [RegularExpression("^[0-9]+$", ErrorMessage = "El código de barra debe contener solo números.")]
        [Display(Name ="Codigo de Barra")]
        public string CodigoBarra { get; set; } = null!;
        [Required(ErrorMessage ="El Nombre es Requerido")]
        public string Nombre { get; set; } = null!;

        [Display(Name = "Peso Neto")]
        [Required(ErrorMessage ="Peso Neto es Requerido")]
        public int? PesoNeto { get; set; }
        [Required(ErrorMessage ="Campo Requerido")]
        [Display(Name ="Categoria")]
        public int CategoriaId { get; set; }

    
        //ControlPrecio
        public int ControlPrecioId { get; set; }

        [Required]
       // [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        [Range(0.01, double.MaxValue, ErrorMessage ="Ingrese un Valor")]
        public decimal Costo { get; set; }

        [Required]
        [Display(Name = "Porcentaje de Ganancia")]
        //[DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        [Range(0.01, double.MaxValue, ErrorMessage ="Ingrese un Valor")]
        public decimal PorcentajeGanancia { get; set; }

        [Display(Name ="Precio de Venta")]
        public decimal PrecioVenta { get; set; }

        //Stock
        public int StockId { get; set; }
        [Required(ErrorMessage = "Ingrese una Cantidad")]
        [Range(1, int.MaxValue, ErrorMessage = "Ingrese un Valor Entero")]
        public int Cantidad { get; set; }



    }
}
