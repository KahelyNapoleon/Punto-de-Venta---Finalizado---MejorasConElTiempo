using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace POS.Models;

public partial class Venta
{
    public int VentaId { get; set; }
    [Display(Name ="Fecha de Venta")]
    [DataType(DataType.Date)]
    public DateTime FechaVenta { get; set; }

    public decimal Total { get; set; }

    public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();
}
