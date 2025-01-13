namespace POS.Models
{
    public class ProductoViewModel
    {
        //Producto
        public int ProductoId { get; set; }
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
