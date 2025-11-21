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
    // Vehículos: todos los roles logueados
    [Authorize(Roles = "Empleado,Jefe,AdminApp")]
    public class TVehiculoesController : Controller
    {
        private readonly DbAlquilerVehiculosContext _context;

        public TVehiculoesController(DbAlquilerVehiculosContext context)
        {
            _context = context;
        }

        // GET: TVehiculoes
        public async Task<IActionResult> Index()
        {
            var dbAlquilerVehiculosContext = _context.TVehiculos.Include(t => t.IdSucursalNavigation).Include(t => t.IdTipoNavigation);
            return View(await dbAlquilerVehiculosContext.ToListAsync());
        }

        // GET: TVehiculoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tVehiculo = await _context.TVehiculos
                .Include(t => t.IdSucursalNavigation)
                .Include(t => t.IdTipoNavigation)
                .FirstOrDefaultAsync(m => m.IdVehiculo == id);
            if (tVehiculo == null)
            {
                return NotFound();
            }

            return View(tVehiculo);
        }

        // GET: TVehiculoes/Create
        public IActionResult Create()
        {
            ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal");
            ViewData["IdTipo"] = new SelectList(_context.TVehiculosTipos, "IdTipo", "IdTipo");
            return View();
        }

        // POST: TVehiculoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Placa,Marca,Modelo,Anio,Estado,IdTipo,IdSucursal")] TVehiculo tVehiculo)
        {
            // 🔹 Validación de rango para el año (antes de tocar BD)
            int anioActual = DateTime.Now.Year;

            if (tVehiculo.Anio < 1900 || tVehiculo.Anio > anioActual)
            {
                ModelState.AddModelError("Anio", $"El año debe estar entre 1900 y {anioActual}.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 🔹 Llamada al Stored Procedure en lugar de _context.Add()
                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC SC_AlquilerVehiculos.SP_VehiculoInsert " +
                        "@Placa, @Marca, @Modelo, @Anio, @Estado, @IdTipo, @IdSucursal",
                        new SqlParameter("@Placa", tVehiculo.Placa),
                        new SqlParameter("@Marca", tVehiculo.Marca),
                        new SqlParameter("@Modelo", tVehiculo.Modelo),
                        new SqlParameter("@Anio", tVehiculo.Anio),
                        new SqlParameter("@Estado", tVehiculo.Estado),
                        new SqlParameter("@IdTipo", tVehiculo.IdTipo),
                        new SqlParameter("@IdSucursal", tVehiculo.IdSucursal)
                    );

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Opcional: puedes loguear ex
                    ModelState.AddModelError(string.Empty,
                        "Ocurrió un error al guardar el vehículo. Verifique los datos e intente nuevamente.");
                }
            }

            // 🔹 Si hay errores, volvemos a cargar los combos y regresamos a la vista
            ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal", tVehiculo.IdSucursal);
            ViewData["IdTipo"] = new SelectList(_context.TVehiculosTipos, "IdTipo", "IdTipo", tVehiculo.IdTipo);

            return View(tVehiculo);
        }

        // GET: TVehiculoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tVehiculo = await _context.TVehiculos.FindAsync(id);
            if (tVehiculo == null)
            {
                return NotFound();
            }
            ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal", tVehiculo.IdSucursal);
            ViewData["IdTipo"] = new SelectList(_context.TVehiculosTipos, "IdTipo", "IdTipo", tVehiculo.IdTipo);
            return View(tVehiculo);
        }

        // POST: TVehiculoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdVehiculo,Placa,Marca,Modelo,Anio,Estado,IdTipo,IdSucursal")] TVehiculo tVehiculo)
        {
            if (id != tVehiculo.IdVehiculo)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal", tVehiculo.IdSucursal);
                ViewData["IdTipo"] = new SelectList(_context.TVehiculosTipos, "IdTipo", "IdTipo", tVehiculo.IdTipo);
                return View(tVehiculo);
            }

            try
            {
                var sql = "EXEC SC_AlquilerVehiculos.SP_VehiculoUpdate " +
                          "@id_vehiculo, @placa, @marca, @modelo, @anio, @estado, @id_tipo, @id_sucursal";

                await _context.Database.ExecuteSqlRawAsync(
                    sql,
                    new SqlParameter("@id_vehiculo", tVehiculo.IdVehiculo),
                    new SqlParameter("@placa", tVehiculo.Placa),
                    new SqlParameter("@marca", tVehiculo.Marca),
                    new SqlParameter("@modelo", tVehiculo.Modelo),
                    new SqlParameter("@anio", tVehiculo.Anio),
                    new SqlParameter("@estado", tVehiculo.Estado),
                    new SqlParameter("@id_tipo", tVehiculo.IdTipo),
                    new SqlParameter("@id_sucursal", tVehiculo.IdSucursal)
                );

                // Si el SP falla, cae al catch. Si no, asumimos que actualizó bien.
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al ejecutar SP_VehiculoUpdate: {ex.Message}");
                ViewData["IdSucursal"] = new SelectList(_context.TSucursales, "IdSucursal", "IdSucursal", tVehiculo.IdSucursal);
                ViewData["IdTipo"] = new SelectList(_context.TVehiculosTipos, "IdTipo", "IdTipo", tVehiculo.IdTipo);
                return View(tVehiculo);
            }
        }


        // GET: TVehiculoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tVehiculo = await _context.TVehiculos
                .Include(t => t.IdSucursalNavigation)
                .Include(t => t.IdTipoNavigation)
                .FirstOrDefaultAsync(m => m.IdVehiculo == id);
            if (tVehiculo == null)
            {
                return NotFound();
            }

            return View(tVehiculo);
        }

        // POST: TVehiculoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var sql = "EXEC SC_AlquilerVehiculos.SP_VehiculoDelete @id_vehiculo";

                await _context.Database.ExecuteSqlRawAsync(
                    sql,
                    new SqlParameter("@id_vehiculo", id)
                );

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al ejecutar SP_VehiculoDelete: {ex.Message}");
                
                return RedirectToAction(nameof(Index));
            }
        }

        private bool TVehiculoExists(int id)
        {
            return _context.TVehiculos.Any(e => e.IdVehiculo == id);
        }
    }
}
