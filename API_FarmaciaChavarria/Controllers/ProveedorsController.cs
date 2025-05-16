using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_FarmaciaChavarria.Context;
using API_FarmaciaChavarria.Models;
using API_FarmaciaChavarria.ModelsDto;
using Microsoft.AspNetCore.Authorization;

namespace API_FarmaciaChavarria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProveedorsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Proveedors
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proveedor>>> GetProveedores()
        {
            return await _context.Proveedores.ToListAsync();
        }

        // GET: api/Proveedors/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProveedorDTO>> GetProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
            {
                return NotFound();
            }

            var prov = new ProveedorDTO
            {
                id_proveedor = proveedor.id_proveedor,
                nombre = proveedor.nombre,
                telefono = proveedor.telefono,
                direccion = proveedor.direccion
            };

            return prov;
        }

        // PUT: api/Proveedors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProveedor(int id, ProveedorDTO proveedorDTO)
        {

            var proveedor = new Proveedor
            {
                id_proveedor = proveedorDTO.id_proveedor,
                nombre = proveedorDTO.nombre,
                direccion = proveedorDTO.direccion,
                telefono = proveedorDTO.telefono
            };

            if (id != proveedor.id_proveedor)
            {
                return BadRequest();
            }

            _context.Entry(proveedor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProveedorExists(id))
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

        // POST: api/Proveedors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Proveedor>> PostProveedor(ProveedorDTO proveedorDTO)
        {
            var proveedor = new Proveedor
            {
                id_proveedor = proveedorDTO.id_proveedor,
                nombre = proveedorDTO.nombre,
                direccion = proveedorDTO.direccion,
                telefono = proveedorDTO.telefono
            };

            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProveedor", new { id = proveedor.id_proveedor }, proveedor);
        }

        // DELETE: api/Proveedors/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }

            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProveedorExists(int id)
        {
            return _context.Proveedores.Any(e => e.id_proveedor == id);
        }
    }
}
