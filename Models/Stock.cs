using System;
using System.Collections.Generic;

namespace POS.Models;

public partial class Stock
{
    public int StockId { get; set; }

    public int ProductoId { get; set; }

    public int Cantidad { get; set; }

    public string? CodigoBarra { get; set; }

    public  Producto? Producto { get; set; }
}
