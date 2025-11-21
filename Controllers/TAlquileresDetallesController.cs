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
    // Detalle de alquileres: igual que alquileres
    [Authorize(Roles = "Empleado,Jefe,AdminApp")]
    public class TAlquileresDetallesController : Controller
    {
        private readonly DbAlquilerVehiculosContext _context;

        public TAlquileresDetallesController(DbAlquilerVehiculosContext context)
        {
            _context = context;
        }

        // GET: TAlquileresDetalles
        public async Task<IActionResult> Index()
        {
            var dbAlquilerVehiculosContext = _context.TAlquileresDetalles.Include(t => t.IdAlquilerNavigation).Include(t => t.IdVehiculoNavigation);
            return View(await dbAlquilerVehiculosContext.ToListAsync());
        }

        // GET: TAlquileresDetalles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tAlquileresDetalle = await _context.TAlquileresDetalles
                .Include(t => t.IdAlquilerNavigation)
                .Include(t => t.IdVehiculoNavigation)
                .FirstOrDefaultAsync(m => m.IdDetalle == id);
            if (tAlquileresDetalle == null)
            {
                return NotFound();
            }

            return View(tAlquileresDetalle);
        }

        // GET: TAlquileresDetalles/Create
        public IActionResult Create()
        {
            ViewData["IdAlquiler"] = new SelectList(_context.TAlquileres, "IdAlquiler", "IdAlquiler");
            ViewData["IdVehiculo"] = new SelectList(_context.TVehiculos, "IdVehiculo", "IdVehiculo");
            return View();
        }

        // POST: TAlquileresDetalles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDetalle,IdAlquiler,IdVehiculo,TarifaDiaria,FechaInicio,FechaFin,Subtotal")] TAlquileresDetalle tAlquileresDetalle)
        {
            if (!ModelState.IsValid)
            {
                ViewData["IdAlquiler"] = new SelectList(_context.TAlquileres, "IdAlquiler", "IdAlquiler", tAlquileresDetalle.IdAlquiler);
                ViewData["IdVehiculo"] = new SelectList(_context.TVehiculos, "IdVehiculo", "IdVehiculo", tAlquileresDetalle.IdVehiculo);
                return View(tAlquileresDetalle);
            }

            try
            {
                var sql = "EXEC SC_AlquilerVehiculos.SP_AlquilerDetalleInsert " +
                          "@id_alquiler, @id_vehiculo, @tarifa_diaria, @fecha_inicio, @fecha_fin, @subtotal";

                await _context.Database.ExecuteSqlRawAsync(
                    sql,
                    new SqlParameter("@id_alquiler", tAlquileresDetalle.IdAlquiler),
                    new SqlParameter("@id_vehiculo", tAlquileresDetalle.IdVehiculo),
                    new SqlParameter("@tarifa_diaria", tAlquileresDetalle.TarifaDiaria),
                    new SqlParameter("@fecha_inicio", tAlquileresDetalle.FechaInicio),
                    new SqlParameter("@fecha_fin", (object?)tAlquileresDetalle.FechaFin ?? DBNull.Value),
                    new SqlParameter("@subtotal", tAlquileresDetalle.Subtotal)
                );

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al ejecutar SP_AlquilerDetalleInsert: {ex.Message}");

                ViewData["IdAlquiler"] = new SelectList(_context.TAlquileres, "IdAlquiler", "IdAlquiler", tAlquileresDetalle.IdAlquiler);
                ViewData["IdVehiculo"] = new SelectList(_context.TVehiculos, "IdVehiculo", "IdVehiculo", tAlquileresDetalle.IdVehiculo);

                return View(tAlquileresDetalle);
            }
        }

        // GET: TAlquileresDetalles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tAlquileresDetalle = await _context.TAlquileresDetalles.FindAsync(id);
            if (tAlquileresDetalle == null)
            {
                return NotFound();
            }
            ViewData["IdAlquiler"] = new SelectList(_context.TAlquileres, "IdAlquiler", "IdAlquiler", tAlquileresDetalle.IdAlquiler);
            ViewData["IdVehiculo"] = new SelectList(_context.TVehiculos, "IdVehiculo", "IdVehiculo", tAlquileresDetalle.IdVehiculo);
            return View(tAlquileresDetalle);
        }

        // POST: TAlquileresDetalles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDetalle,IdAlquiler,IdVehiculo,TarifaDiaria,FechaInicio,FechaFin,Subtotal")] TAlquileresDetalle tAlquileresDetalle)
        {
            if (id != tAlquileresDetalle.IdDetalle)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["IdAlquiler"] = new SelectList(_context.TAlquileres, "IdAlquiler", "IdAlquiler", tAlquileresDetalle.IdAlquiler);
                ViewData["IdVehiculo"] = new SelectList(_context.TVehiculos, "IdVehiculo", "IdVehiculo", tAlquileresDetalle.IdVehiculo);
                return View(tAlquileresDetalle);
            }

            try
            {
                var sql = "EXEC SC_AlquilerVehiculos.SP_AlquilerDetalleUpdate " +
                          "@id_detalle, @id_alquiler, @id_vehiculo, @tarifa_diaria, @fecha_inicio, @fecha_fin, @subtotal";

                await _context.Database.ExecuteSqlRawAsync(
                    sql,
                    new SqlParameter("@id_detalle", tAlquileresDetalle.IdDetalle),
                    new SqlParameter("@id_alquiler", tAlquileresDetalle.IdAlquiler),
                    new SqlParameter("@id_vehiculo", tAlquileresDetalle.IdVehiculo),
                    new SqlParameter("@tarifa_diaria", tAlquileresDetalle.TarifaDiaria),
                    new SqlParameter("@fecha_inicio", tAlquileresDetalle.FechaInicio),
                    new SqlParameter("@fecha_fin", (object?)tAlquileresDetalle.FechaFin ?? DBNull.Value),
                    new SqlParameter("@subtotal", tAlquileresDetalle.Subtotal)
                );

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al ejecutar SP_AlquilerDetalleUpdate: {ex.Message}");

                ViewData["IdAlquiler"] = new SelectList(_context.TAlquileres, "IdAlquiler", "IdAlquiler", tAlquileresDetalle.IdAlquiler);
                ViewData["IdVehiculo"] = new SelectList(_context.TVehiculos, "IdVehiculo", "IdVehiculo", tAlquileresDetalle.IdVehiculo);

                return View(tAlquileresDetalle);
            }
        }


        // GET: TAlquileresDetalles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tAlquileresDetalle = await _context.TAlquileresDetalles
                .Include(t => t.IdAlquilerNavigation)
                .Include(t => t.IdVehiculoNavigation)
                .FirstOrDefaultAsync(m => m.IdDetalle == id);
            if (tAlquileresDetalle == null)
            {
                return NotFound();
            }

            return View(tAlquileresDetalle);
        }

        // POST: TAlquileresDetalles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var sql = "EXEC SC_AlquilerVehiculos.SP_AlquilerDetalleDelete @id_detalle";

                await _context.Database.ExecuteSqlRawAsync(
                    sql,
                    new SqlParameter("@id_detalle", id)
                );

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMensaje"] =
                    $"Error al ejecutar SP_AlquilerDetalleDelete: {ex.Message}";

                return RedirectToAction(nameof(Delete), new { id });
            }
        }


        private bool TAlquileresDetalleExists(int id)
        {
            return _context.TAlquileresDetalles.Any(e => e.IdDetalle == id);
        }
    }
}
