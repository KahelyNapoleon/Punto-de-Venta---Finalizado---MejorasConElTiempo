using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace POS.Models;

public partial class DetalleVenta
{
    public int DetalleVentaId { get; set; }

    public int VentaId { get; set; }

    public int ProductoId { get; set; }

    public int Cantidad { get; set; }
    [DisplayName("Precio Unitario")]
    public decimal PrecioUnitario { get; set; }
    [DisplayNameAttribute("SubTotal")]
    public decimal? Subtotal { get; set; }
    [DisplayName("Codigo Barra")]
    public string? CodigoBarra { get; set; }

    public virtual Producto Producto { get; set; } = null!;

    public virtual Venta Venta { get; set; } = null!;
}
