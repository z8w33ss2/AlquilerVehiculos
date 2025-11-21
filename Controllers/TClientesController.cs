using AlquilerVehiculos.Data;
using AlquilerVehiculos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlquilerVehiculos.Controllers
{
    // Clientes: cualquier usuario logueado (Empleado, Jefe, AdminApp)
    [Authorize(Roles = "Empleado,Jefe,AdminApp")]
    public class TClientesController : Controller
    {
        private readonly DbAlquilerVehiculosContext _context;

        public TClientesController(DbAlquilerVehiculosContext context)
        {
            _context = context;
        }

        // GET: TClientes
        public async Task<IActionResult> Index()
        {
            return View(await _context.TClientes.ToListAsync());
        }

        // GET: TClientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tCliente = await _context.TClientes
                .FirstOrDefaultAsync(m => m.IdCliente == id);
            if (tCliente == null)
            {
                return NotFound();
            }

            return View(tCliente);
        }

        // GET: TClientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TClientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCliente,Nombre,Cedula,Telefono,Email,Direccion")] TCliente tCliente)
        {
            if (!ModelState.IsValid)
            {
                return View(tCliente);
            }

            try
            {
                var sql = "EXEC SC_AlquilerVehiculos.SP_ClienteInsert " +
                          "@nombre, @cedula, @telefono, @email, @direccion";

                await _context.Database.ExecuteSqlRawAsync(
                    sql,
                    new SqlParameter("@nombre", tCliente.Nombre),
                    new SqlParameter("@cedula", tCliente.Cedula),
                    new SqlParameter("@telefono", (object?)tCliente.Telefono ?? DBNull.Value),
                    new SqlParameter("@email", (object?)tCliente.Email ?? DBNull.Value),
                    new SqlParameter("@direccion", (object?)tCliente.Direccion ?? DBNull.Value)
                );

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al ejecutar SP_ClienteInsert: {ex.Message}");
                return View(tCliente);
            }
        }

        // GET: TClientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tCliente = await _context.TClientes.FindAsync(id);
            if (tCliente == null)
            {
                return NotFound();
            }
            return View(tCliente);
        }

        // POST: TClientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCliente,Nombre,Cedula,Telefono,Email,Direccion")] TCliente tCliente)
        {
            if (id != tCliente.IdCliente)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(tCliente);
            }

            try
            {
                var sql = "EXEC SC_AlquilerVehiculos.SP_ClienteUpdate " +
                          "@id_cliente, @nombre, @cedula, @telefono, @email, @direccion";

                await _context.Database.ExecuteSqlRawAsync(
                    sql,
                    new SqlParameter("@id_cliente", tCliente.IdCliente),
                    new SqlParameter("@nombre", tCliente.Nombre),
                    new SqlParameter("@cedula", tCliente.Cedula),
                    new SqlParameter("@telefono", (object?)tCliente.Telefono ?? DBNull.Value),
                    new SqlParameter("@email", (object?)tCliente.Email ?? DBNull.Value),
                    new SqlParameter("@direccion", (object?)tCliente.Direccion ?? DBNull.Value)
                );

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al ejecutar SP_ClienteUpdate: {ex.Message}");
                return View(tCliente);
            }
        }

        // GET: TClientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tCliente = await _context.TClientes
                .FirstOrDefaultAsync(m => m.IdCliente == id);
            if (tCliente == null)
            {
                return NotFound();
            }

            return View(tCliente);
        }

        // POST: TClientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // 1. Verifica si el cliente tiene alquileres asociados
            var tieneAlquileres = await _context.TAlquileres
                .AnyAsync(a => a.IdCliente == id);

            if (tieneAlquileres)
            {
                TempData["ErrorMensaje"] = "No se puede eliminar este cliente porque tiene alquileres asociados.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            try
            {
                // 2. Ejecuta el SP de eliminación
                var sql = "EXEC SC_AlquilerVehiculos.SP_ClienteDelete @id_cliente";

                await _context.Database.ExecuteSqlRawAsync(
                    sql,
                    new SqlParameter("@id_cliente", id)
                );

                // 3. Si todo salió bien
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // 4. Manejo de errores genérico (puedes afinarlo si tienes FKs específicas)
                TempData["ErrorMensaje"] =
                    $"Error al ejecutar SP_ClienteDelete: {ex.Message}";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }



        private bool TClienteExists(int id)
        {
            return _context.TClientes.Any(e => e.IdCliente == id);
        }
    }
}
