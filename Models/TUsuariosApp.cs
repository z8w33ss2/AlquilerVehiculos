using System;

namespace AlquilerVehiculos.Models
{
    public partial class TUsuariosApp
    {
        public int IdUsuario { get; set; }
        public string Usuario { get; set; } = null!;
        public string Contrasena { get; set; } = null!;
        public string Rol { get; set; } = null!;
    }
}
