using AlquilerVehiculos.Data;
using AlquilerVehiculos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlquilerVehiculos.Controllers
{
    [Authorize(Roles = "Empleado,Jefe,AdminApp")]
    public class TAlquileresController : Controller
    {
        private readonly DbAlquilerVehiculosContext _context;

        public TAlquileresController(DbAlquilerVehiculosContext context)
        {
            _context = context;
        }

        // GET: TAlquileres
        public async Task<IActionResult> Index()
        {
            var dbAlquilerVehiculosContext = _context.TAlquileres.Include(t => t.IdClienteNavigation).Include(t => t.IdEmpleadoNavigation).Include(t => t.IdSucursalNavigation);
            return View(await dbAlquilerVehiculosContext.ToListAsync());
        }

        // GET: TAlquileres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tAlquilere = await _context.TAlquileres
                .Include(t => t.IdClienteNavigation)
                .Include(t => t.IdEmpleadoNavigation)
                .Include(t => t.IdSucursalNavigation)
                .FirstOrDefaultAsync(m => m.IdAlquiler == id);
            if (tAlquilere == null)
            {
                return NotFound();
            }

            return View(tAlquilere);
        }

        // GET: TAlquileres/Create
        public IActionResult Create()
        {
            ViewData["IdCliente"] = new SelectList(_context.TClientes, "IdCliente", "IdCliente");
            ViewData["IdEmpleado"] = new SelectList(_context.TEmpleados, "IdEmpleado", "IdEmpleado");
            ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal");
            return View();
        }

        // POST: TAlquileres/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAlquiler,FechaInicio,FechaFin,Iva,IdCliente,IdEmpleado,IdSucursal,Estado")] TAlquilere tAlquilere)
        {
            if (!ModelState.IsValid)
            {
                ViewData["IdCliente"] = new SelectList(_context.TClientes, "IdCliente", "IdCliente", tAlquilere.IdCliente);
                ViewData["IdEmpleado"] = new SelectList(_context.TEmpleados, "IdEmpleado", "IdEmpleado", tAlquilere.IdEmpleado);
                ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal", tAlquilere.IdSucursal);
                return View(tAlquilere);
            }

            try
            {
                var sql = "EXEC SC_AlquilerVehiculos.SP_AlquilerInsert " +
                          "@fecha_inicio, @fecha_fin, @iva, @id_cliente, @id_empleado, @id_sucursal, @estado";

                await _context.Database.ExecuteSqlRawAsync(
                    sql,
                    new SqlParameter("@fecha_inicio", tAlquilere.FechaInicio),
                    new SqlParameter("@fecha_fin", (object?)tAlquilere.FechaFin ?? DBNull.Value),
                    new SqlParameter("@iva", tAlquilere.Iva),
                    new SqlParameter("@id_cliente", tAlquilere.IdCliente),
                    new SqlParameter("@id_empleado", tAlquilere.IdEmpleado),
                    new SqlParameter("@id_sucursal", tAlquilere.IdSucursal),
                    new SqlParameter("@estado", tAlquilere.Estado)
                );

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Manejo elegante de errores
                ModelState.AddModelError(string.Empty, $"Error al ejecutar SP_AlquilerInsert: {ex.Message}");

                ViewData["IdCliente"] = new SelectList(_context.TClientes, "IdCliente", "IdCliente", tAlquilere.IdCliente);
                ViewData["IdEmpleado"] = new SelectList(_context.TEmpleados, "IdEmpleado", "IdEmpleado", tAlquilere.IdEmpleado);
                ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal", tAlquilere.IdSucursal);

                return View(tAlquilere);
            }
        }


        // GET: TAlquileres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tAlquilere = await _context.TAlquileres.FindAsync(id);
            if (tAlquilere == null)
            {
                return NotFound();
            }
            ViewData["IdCliente"] = new SelectList(_context.TClientes, "IdCliente", "IdCliente", tAlquilere.IdCliente);
            ViewData["IdEmpleado"] = new SelectList(_context.TEmpleados, "IdEmpleado", "IdEmpleado", tAlquilere.IdEmpleado);
            ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal", tAlquilere.IdSucursal);
            return View(tAlquilere);
        }

        // POST: TAlquileres/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAlquiler,FechaInicio,FechaFin,Iva,IdCliente,IdEmpleado,IdSucursal,Estado")] TAlquilere tAlquilere)
        {
            if (id != tAlquilere.IdAlquiler)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["IdCliente"] = new SelectList(_context.TClientes, "IdCliente", "IdCliente", tAlquilere.IdCliente);
                ViewData["IdEmpleado"] = new SelectList(_context.TEmpleados, "IdEmpleado", "IdEmpleado", tAlquilere.IdEmpleado);
                ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal", tAlquilere.IdSucursal);
                return View(tAlquilere);
            }

            try
            {
                var sql = "EXEC SC_AlquilerVehiculos.SP_AlquilerUpdate " +
                          "@id_alquiler, @fecha_inicio, @fecha_fin, @iva, @id_cliente, @id_empleado, @id_sucursal, @estado";

                await _context.Database.ExecuteSqlRawAsync(
                    sql,
                    new SqlParameter("@id_alquiler", tAlquilere.IdAlquiler),
                    new SqlParameter("@fecha_inicio", tAlquilere.FechaInicio),
                    new SqlParameter("@fecha_fin", (object?)tAlquilere.FechaFin ?? DBNull.Value),
                    new SqlParameter("@iva", tAlquilere.Iva),
                    new SqlParameter("@id_cliente", tAlquilere.IdCliente),
                    new SqlParameter("@id_empleado", tAlquilere.IdEmpleado),
                    new SqlParameter("@id_sucursal", tAlquilere.IdSucursal),
                    new SqlParameter("@estado", tAlquilere.Estado)
                );

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al ejecutar SP_AlquilerUpdate: {ex.Message}");
                ViewData["IdCliente"] = new SelectList(_context.TClientes, "IdCliente", "IdCliente", tAlquilere.IdCliente);
                ViewData["IdEmpleado"] = new SelectList(_context.TEmpleados, "IdEmpleado", "IdEmpleado", tAlquilere.IdEmpleado);
                ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal", tAlquilere.IdSucursal);
                return View(tAlquilere);
            }
        }


        // GET: TAlquileres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tAlquilere = await _context.TAlquileres
                .Include(t => t.IdClienteNavigation)
                .Include(t => t.IdEmpleadoNavigation)
                .Include(t => t.IdSucursalNavigation)
                .FirstOrDefaultAsync(m => m.IdAlquiler == id);
            if (tAlquilere == null)
            {
                return NotFound();
            }

            return View(tAlquilere);
        }

        // POST: TAlquileres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // 1. Verifica si el alquiler tiene DETALLES asociados
            var tieneDetalles = await _context.TAlquileresDetalles
                .AnyAsync(d => d.IdAlquiler == id);

            if (tieneDetalles)
            {
                TempData["ErrorMensaje"] =
                    "No se puede eliminar este alquiler porque posee detalles de alquiler asociados.";
                // Devolvemos a la misma pantalla de Delete con el id
                return RedirectToAction(nameof(Delete), new { id });
            }

            try
            {
                // 2. Ejecuta el SP de eliminación del alquiler
                var sql = "EXEC SC_AlquilerVehiculos.SP_AlquilerDelete @id_alquiler";

                await _context.Database.ExecuteSqlRawAsync(
                    sql,
                    new SqlParameter("@id_alquiler", id)
                );

                // 3. Si todo salió bien
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // 4. Si el error viene de la FK de recibos, mostramos mensaje amigable
                if (ex.InnerException?.Message.Contains("FK_AlqRec_Recibos") == true)
                {
                    TempData["ErrorMensaje"] =
                        "No se puede eliminar este alquiler porque posee recibos asociados.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                // 5. Otros errores: no los ocultamos
                throw;
            }
            catch (Exception ex)
            {
                // Por si el proveedor lanza otra excepción distinta a DbUpdateException
                TempData["ErrorMensaje"] =
                    $"Error al ejecutar SP_AlquilerDelete: {ex.Message}";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }



        private bool TAlquilereExists(int id)
        {
            return _context.TAlquileres.Any(e => e.IdAlquiler == id);
        }
    }
}
