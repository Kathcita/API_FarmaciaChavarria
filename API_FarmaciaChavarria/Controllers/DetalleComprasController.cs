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
    public class DetalleComprasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DetalleComprasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/DetalleCompras
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetalleCompra>>> GetDetalle_Compras()
        {
            return await _context.Detalle_Compras.ToListAsync();
        }

        // GET: api/DetalleCompras/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<DetalleCompra>> GetDetalleCompra(int id)
        {
            var detalleCompra = await _context.Detalle_Compras.FindAsync(id);

            if (detalleCompra == null)
            {
                return NotFound();
            }

            return detalleCompra;
        }

        // PUT: api/DetalleCompras/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDetalleCompra(int id, DetalleCompra detalleCompra)
        {
            if (id != detalleCompra.id_detalle)
            {
                return BadRequest();
            }

            _context.Entry(detalleCompra).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DetalleCompraExists(id))
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

        // POST: api/DetalleCompras
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<DetalleCompra>> PostDetalleCompra(DetalleCompra detalleCompra)
        {
            _context.Detalle_Compras.Add(detalleCompra);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDetalleCompra", new { id = detalleCompra.id_detalle }, detalleCompra);
        }

        // DELETE: api/DetalleCompras/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetalleCompra(int id)
        {
            var detalleCompra = await _context.Detalle_Compras.FindAsync(id);
            if (detalleCompra == null)
            {
                return NotFound();
            }

            _context.Detalle_Compras.Remove(detalleCompra);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DetalleCompraExists(int id)
        {
            return _context.Detalle_Compras.Any(e => e.id_detalle == id);
        }
    }
}
