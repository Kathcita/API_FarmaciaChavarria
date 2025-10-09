using API_FarmaciaChavarria.Context;
using API_FarmaciaChavarria.Models;
using API_FarmaciaChavarria.Models.PaginationModels;
using API_FarmaciaChavarria.ModelsDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        [EnableRateLimiting("globalLimiter")]
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ProveedorPagedResult>> GetProveedores(int page = 1, int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var totalItems = await _context.Proveedores.CountAsync();
            var proveedores = await _context.Proveedores
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new ProveedorPagedResult
            {
                Proveedores = proveedores,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
            };

            return Ok(result);
        }

        // GET: api/Proveedors/buscar?nombre=Farmacia&page=1&pageSize=10
        [EnableRateLimiting("globalLimiter")]
        [Authorize]
        [HttpGet("buscar")]
        public async Task<ActionResult<ProveedorPagedResult>> BuscarProveedoresPorNombre(string nombre, int page = 1, int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Proveedores
                .Where(p => p.nombre.ToLower().Contains(nombre.ToLower()));

            var totalItems = await query.CountAsync();

            var proveedores = await query
                .OrderBy(p => p.nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new ProveedorPagedResult
            {
                Proveedores = proveedores,
                TotalItems = totalItems,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
            };

            return Ok(result);
        }

        // GET: api/Proveedors/5
        [EnableRateLimiting("globalLimiter")]
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
        [EnableRateLimiting("globalLimiter")]
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProveedor(int id, ProveedorDTO proveedorDTO)
        {
            if (proveedorDTO.nombre == "")
            {
                return BadRequest("El campo nombre de proveedor no puede estar vacío");
            }

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
        [EnableRateLimiting("globalLimiter")]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Proveedor>> PostProveedor(ProveedorDTO proveedorDTO)
        {
            if (proveedorDTO.nombre == "")
            {
                return BadRequest("El campo nombre de proveedor no puede estar vacío");
            }

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
        [EnableRateLimiting("globalLimiter")]
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
