using AlquilerVehiculos.Data;
using AlquilerVehiculos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AlquilerVehiculos.Controllers
{
    // Empleados: solo Jefe y Admin
    [Authorize(Roles = "Jefe,AdminApp")]
    public class TEmpleadoesController : Controller
    {
        private readonly DbAlquilerVehiculosContext _context;

        public TEmpleadoesController(DbAlquilerVehiculosContext context)
        {
            _context = context;
        }

        // GET: TEmpleadoes
        public async Task<IActionResult> Index()
        {
            var dbAlquilerVehiculosContext = _context.TEmpleados
                .Include(t => t.IdSucursalNavigation);
            return View(await dbAlquilerVehiculosContext.ToListAsync());
        }

        // GET: TEmpleadoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var tEmpleado = await _context.TEmpleados
                .Include(t => t.IdSucursalNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleado == id);

            if (tEmpleado == null)
                return NotFound();

            return View(tEmpleado);
        }

        // GET: TEmpleadoes/Create
        public IActionResult Create()
        {
            ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "Nombre");
            return View();
        }

        // POST: TEmpleadoes/Create  (usa SP_EmpleadoInsert)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Correo,Telefono,Puesto,IdSucursal")] TEmpleado tEmpleado)
        {
            if (!ModelState.IsValid)
            {
                ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "Nombre", tEmpleado.IdSucursal);
                return View(tEmpleado);
            }

            try
            {
                var sql = "EXEC SC_AlquilerVehiculos.SP_EmpleadoInsert " +
                          "@nombre, @correo, @telefono, @puesto, @id_sucursal";

                await _context.Database.ExecuteSqlRawAsync(
                    sql,
                    new SqlParameter("@nombre", tEmpleado.Nombre),
                    new SqlParameter("@correo", tEmpleado.Correo),
                    new SqlParameter("@telefono", (object?)tEmpleado.Telefono ?? DBNull.Value),
                    new SqlParameter("@puesto", tEmpleado.Puesto),
                    new SqlParameter("@id_sucursal", tEmpleado.IdSucursal)
                );

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al ejecutar SP_EmpleadoInsert: {ex.Message}");
                ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "Nombre", tEmpleado.IdSucursal);
                return View(tEmpleado);
            }
        }


        // GET: TEmpleadoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var tEmpleado = await _context.TEmpleados.FindAsync(id);
            if (tEmpleado == null)
                return NotFound();

            ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "Nombre", tEmpleado.IdSucursal);
            return View(tEmpleado);
        }

        // POST: TEmpleadoes/Edit/5  (usa SP_EmpleadoUpdate)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmpleado,Nombre,Correo,Telefono,Puesto,IdSucursal")] TEmpleado tEmpleado)
        {
            if (id != tEmpleado.IdEmpleado)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "Nombre", tEmpleado.IdSucursal);
                return View(tEmpleado);
            }

            try
            {
                var sql = "EXEC SC_AlquilerVehiculos.SP_EmpleadoUpdate " +
                          "@id_empleado, @nombre, @correo, @telefono, @puesto, @id_sucursal";

                await _context.Database.ExecuteSqlRawAsync(
                    sql,
                    new SqlParameter("@id_empleado", tEmpleado.IdEmpleado),
                    new SqlParameter("@nombre", tEmpleado.Nombre),
                    new SqlParameter("@correo", tEmpleado.Correo),
                    new SqlParameter("@telefono", (object?)tEmpleado.Telefono ?? DBNull.Value),
                    new SqlParameter("@puesto", tEmpleado.Puesto),
                    new SqlParameter("@id_sucursal", tEmpleado.IdSucursal)
                );

                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al ejecutar SP_EmpleadoUpdate: {ex.Message}");
                ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "Nombre", tEmpleado.IdSucursal);
                return View(tEmpleado);
            }
        }

        // GET: TEmpleadoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var tEmpleado = await _context.TEmpleados
                .Include(t => t.IdSucursalNavigation)
                .FirstOrDefaultAsync(m => m.IdEmpleado == id);

            if (tEmpleado == null)
                return NotFound();

            return View(tEmpleado);
        }

        // POST: TEmpleadoes/Delete/5  (usa SP_EmpleadoDelete)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var sql = "EXEC SC_AlquilerVehiculos.SP_EmpleadoDelete @id_empleado";

                await _context.Database.ExecuteSqlRawAsync(
                    sql,
                    new SqlParameter("@id_empleado", id)
                );

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al ejecutar SP_EmpleadoDelete: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        private bool TEmpleadoExists(int id)
        {
            return _context.TEmpleados.Any(e => e.IdEmpleado == id);
        }
    }
}
