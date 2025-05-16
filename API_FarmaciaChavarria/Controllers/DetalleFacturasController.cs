using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_FarmaciaChavarria.Context;
using API_FarmaciaChavarria.Models;
using Microsoft.AspNetCore.Authorization;

namespace API_FarmaciaChavarria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleFacturasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DetalleFacturasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/DetalleFacturas
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetalleFactura>>> GetDetalleFactura()
        {
            return await _context.Detalle_Facturas.ToListAsync();
        }

        // GET: api/DetalleFacturas/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<DetalleFactura>> GetDetalleFactura(int id)
        {
            var detalleFactura = await _context.Detalle_Facturas.FindAsync(id);

            if (detalleFactura == null)
            {
                return NotFound();
            }

            return detalleFactura;
        }

        // PUT: api/DetalleFacturas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDetalleFactura(int id, DetalleFactura detalleFactura)
        {
            if (id != detalleFactura.id_detalle)
            {
                return BadRequest();
            }

            _context.Entry(detalleFactura).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DetalleFacturaExists(id))
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

        // POST: api/DetalleFacturas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<DetalleFactura>> PostDetalleFactura(DetalleFactura detalleFactura)
        {
            _context.Detalle_Facturas.Add(detalleFactura);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDetalleFactura", new { id = detalleFactura.id_detalle }, detalleFactura);
        }

        // DELETE: api/DetalleFacturas/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetalleFactura(int id)
        {
            var detalleFactura = await _context.Detalle_Facturas.FindAsync(id);
            if (detalleFactura == null)
            {
                return NotFound();
            }

            _context.Detalle_Facturas.Remove(detalleFactura);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DetalleFacturaExists(int id)
        {
            return _context.Detalle_Facturas.Any(e => e.id_detalle == id);
        }
    }
}
