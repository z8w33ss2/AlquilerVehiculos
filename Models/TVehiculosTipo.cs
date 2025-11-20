using System;
using System.Collections.Generic;

namespace AlquilerVehiculos.Models;

public partial class TVehiculosTipo
{
    public int IdTipo { get; set; }

    public string Descripcion { get; set; } = null!;

    public decimal TarifaDiaria { get; set; }

    public virtual ICollection<TVehiculo> TVehiculos { get; set; } = new List<TVehiculo>();
}
