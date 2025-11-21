using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlquilerVehiculos.Models;


public partial class TVehiculo
{
    public int IdVehiculo { get; set; }

    public string Placa { get; set; } = null!;

    public string Marca { get; set; } = null!;

    public string Modelo { get; set; } = null!;

    [Required(ErrorMessage = "El año es obligatorio")]
    [Range(1900, 2100, ErrorMessage = "Debe ingresar un año válido")]
    public int Anio { get; set; }

    public string Estado { get; set; } = null!;

    public int IdTipo { get; set; }

    public int IdSucursal { get; set; }

    public virtual TSucursale? IdSucursalNavigation { get; set; }

    public virtual TVehiculosTipo? IdTipoNavigation { get; set; }

    public virtual ICollection<TAlquileresDetalle> TAlquileresDetalles { get; set; } = new List<TAlquileresDetalle>();
}
