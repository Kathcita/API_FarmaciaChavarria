using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_FarmaciaChavarria.Context;
using API_FarmaciaChavarria.Models;
using API_FarmaciaChavarria.Models.Reporte_Models;
using System.Globalization;

namespace API_FarmaciaChavarria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FacturasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Facturas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Factura>>> GetFacturas()
        {
            return await _context.Facturas.ToListAsync();
        }

        [HttpGet("facturas-año")]
        public async Task<ActionResult<IEnumerable<Factura>>> GetFacturasPorAño([FromQuery] int year, [FromQuery] int userId = 0)
        {
            var query = _context.Facturas
         .Where(f => f.fecha_venta.Year == year);

            // Solo filtrar por usuario si userId es diferente de 0
            if (userId != 0)
            {
                query = query.Where(f => f.id_usuario == userId);
            }

            return await query.ToListAsync();

        }

        [HttpGet("ventas-por-mes-año")]
        public async Task<ActionResult<IEnumerable<RevenueDataItem>>> GetVentasPorMesAño([FromQuery] int year, int userId = 0)
        {
            // Obtener todas las facturas del año seleccionado
            var query = _context.Facturas
                .Where(f => f.fecha_venta.Year == year);

            if (userId != 0)
            {
                query = query.Where(u => u.id_usuario == userId);
            }

            var facturas = await query.ToListAsync();

            // Agrupar por mes y sumar los totales
            var ventasPorMes = facturas
                .GroupBy(f => f.fecha_venta.Month) // Agrupa por mes (1-12)
                .Select(g => new RevenueDataItem
                {
                    Date = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key), // "Ene", "Feb", etc.
                    Revenue = g.Sum(f => f.total) // Suma de ventas del mes
                })
                .OrderBy(item => DateTime.ParseExact(item.Date, "MMM", CultureInfo.CurrentCulture).Month) // Ordenar cronológicamente
                .ToList();

            return Ok(ventasPorMes);
        }

        // GET: api/Facturas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Factura>> GetFactura(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);

            if (factura == null)
            {
                return NotFound();
            }

            return factura;
        }

        // PUT: api/Facturas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFactura(int id, Factura factura)
        {
            if (id != factura.id_factura)
            {
                return BadRequest();
            }

            _context.Entry(factura).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FacturaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Facturas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Factura>> PostFactura(Factura factura)
        {
            _context.Facturas.Add(factura);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFactura", new { id = factura.id_factura }, factura);
        }

        // DELETE: api/Facturas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFactura(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);
            if (factura == null)
            {
                return NotFound();
            }

            _context.Facturas.Remove(factura);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FacturaExists(int id)
        {
            return _context.Facturas.Any(e => e.id_factura == id);
        }
    }
}
