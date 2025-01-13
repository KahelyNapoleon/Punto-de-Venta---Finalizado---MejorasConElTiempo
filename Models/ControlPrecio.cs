using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace POS.Models;

public partial class ControlPrecio
{
    public int ControlPrecioId { get; set; }

    public int ProductoId { get; set; }

    [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
    [Range(0.01, double.MaxValue)]
    public decimal Costo { get; set; }

    [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
    [Range(0.01, double.MaxValue)]
    [Display(Name ="Porcentaje de Ganancia")]
    public decimal PorcentajeGanancia { get; set; }
    [DisplayName("Precio de Venta")]
    public decimal PrecioVenta { get; set; }
    [DisplayName("Codigo Barra")]
    public string? CodigoBarra { get; set; }

    public virtual Producto? Producto { get; set; }
}
