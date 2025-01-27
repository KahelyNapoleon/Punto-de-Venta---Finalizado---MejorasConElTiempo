using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace POS.Models;
[BindProperties]
public partial class Producto
{
    public int ProductoId { get; set; }
   
    [Required(ErrorMessage = "El código de barra es obligatorio.")]
    [StringLength(13, ErrorMessage = "El código de barra debe tener exactamente 13 caracteres.", MinimumLength = 13)]
    [RegularExpression("^[0-9]+$", ErrorMessage = "El código de barra debe contener solo números.")]
    public string CodigoBarra { get; set; } = null!;
    [Required(ErrorMessage = "Este Campo es Obligatorio")]
    public string Nombre { get; set; } = null!;
    [Display(Name ="Peso Neto")]
    public int PesoNeto { get; set; }
    [Display(Name = "Categoria")]
    [Required(ErrorMessage = "La categoría es obligatoria.")]
    public int? CategoriaId { get; set; }

    //HAY QUE REALIZAR EL CAMBIO EN EL CONTROLADOR DE EDICION DE PRODUCTO
    //SE DEBE REMOVER LA PROPEIDAD DE MODELO "CATEGORIA" DE LA VALIDACION DE MODELOS QUE SE GENERA EN EL CONTROLADOR 
    //(!MODELSTATE.ISVALID) => REMOVE(CATEGORIA) --> RESUELTO
    [Display(Name ="Categoria")]
    public virtual Categoria? Categoria { get; set; }

    public virtual ControlPrecio? ControlPrecio { get; set; }

    public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();

    public  virtual Stock? Stock { get; set; }
}
