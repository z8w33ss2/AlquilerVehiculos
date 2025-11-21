using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;

namespace AlquilerVehiculos.Models;


public partial class TEmpleado
{
    public int IdEmpleado { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string? Telefono { get; set; }

    public string Puesto { get; set; } = null!;

    public int IdSucursal { get; set; }

    // Ahora es nullable -> ya no se valida como requerido
    public virtual TSucursale? IdSucursalNavigation { get; set; }

    public virtual ICollection<TAlquilere> TAlquileres { get; set; } = new List<TAlquilere>();
}
