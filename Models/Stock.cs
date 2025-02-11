using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace POS.Models;

public partial class Stock
{

    public int StockId { get; set; }

    public int ProductoId { get; set; }
   
    //[Required(ErrorMessage = "Ingrese una Cantidad")]
    public int Cantidad { get; set; }
    [Required]
    [Display(Name ="Codigo de Barra")]
    public string? CodigoBarra { get; set; }

    public  Producto? Producto { get; set; }
}
