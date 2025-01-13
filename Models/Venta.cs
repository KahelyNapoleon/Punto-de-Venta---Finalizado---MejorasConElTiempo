using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace POS.Models;

public partial class Venta
{
    public int VentaId { get; set; }
    [DisplayName("Fecha de Venta")]
    public DateTime FechaVenta { get; set; }

    public decimal Total { get; set; }

    public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();
}
