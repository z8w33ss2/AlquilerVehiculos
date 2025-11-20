using AlquilerVehiculos.Data;
using AlquilerVehiculos.Models; 
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace AlquilerVehiculos.Controllers
{
    public class AccountController : Controller
    {
        private readonly DbAlquilerVehiculosContext _context;

        public AccountController(DbAlquilerVehiculosContext context)
        {
            _context = context;
        }

        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public async Task<IActionResult> Login(string usuario, string contrasena)
        {
            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasena))
            {
                ViewBag.Error = "Debe ingresar usuario y contraseña.";
                return View();
            }

            // Ejecutar el SP y obtener el resultado en memoria
            var usuarios = await _context.TUsuariosApps
                .FromSqlRaw(
                    "EXEC SC_AlquilerVehiculos.SP_UsuarioLogin @usuario, @contrasena",
                    new SqlParameter("@usuario", usuario),
                    new SqlParameter("@contrasena", contrasena)
                )
                .ToListAsync();   

            var user = usuarios.FirstOrDefault();   // LINQ en memoria

            if (user == null)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos.";
                return View();
            }

            // Crear Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Usuario),
                new Claim(ClaimTypes.Role, user.Rol) // Empleado / Jefe / AdminApp
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Iniciar sesión con cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,        // Para mantener sesión
                    ExpiresUtc = DateTime.UtcNow.AddHours(2)
                });

            return RedirectToAction("Index", "Home");
        }

        // Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        // AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
