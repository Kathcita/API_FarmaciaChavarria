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
    public class ProductosCaducarController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosCaducarController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Productos_Caducar
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoCaducar>>> GetProductos_Caducar()
        {
            return await _context.Productos_Caducar.ToListAsync();
        }

        // GET: api/Productos_Caducar/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoCaducarDTO>> GetProductoCaducar(int id)
        {
            var productoCaducar = await _context.Productos_Caducar.FindAsync(id);

            if (productoCaducar == null)
            {
                return NotFound();
            }

            var prodCad = new ProductoCaducarDTO
            {
                id_producto = productoCaducar.id_producto,
                nombre = productoCaducar.nombre,
                fecha_vencimiento = productoCaducar.fecha_vencimiento
            };

            return prodCad;
        }

        // PUT: api/Productos_Caducar/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductoCaducar(int id, ProductoCaducarDTO productoCaducarDTO)
        {

            var productoCaducar = new ProductoCaducarDTO
            {
                id_producto = productoCaducarDTO.id_producto,
                fecha_vencimiento = productoCaducarDTO.fecha_vencimiento,
                nombre = productoCaducarDTO.nombre
            };

            if (id != productoCaducar.id_producto)
            {
                return BadRequest();
            }

            _context.Entry(productoCaducar).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoCaducarExists(id))
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

        // POST: api/Productos_Caducar
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ProductoCaducar>> PostProductoCaducar(ProductoCaducarDTO productoCaducarDTO)
        {
            var productoCaducar = new ProductoCaducar
            {
                id_producto = productoCaducarDTO.id_producto,
                fecha_vencimiento = productoCaducarDTO.fecha_vencimiento,
                nombre = productoCaducarDTO.nombre
            };

            _context.Productos_Caducar.Add(productoCaducar);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProductoCaducarExists(productoCaducar.id_producto))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetProductoCaducar", new { id = productoCaducar.id_producto }, productoCaducar);
        }

        // DELETE: api/Productos_Caducar/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductoCaducar(int id)
        {
            var productoCaducar = await _context.Productos_Caducar.FindAsync(id);
            if (productoCaducar == null)
            {
                return NotFound();
            }

            _context.Productos_Caducar.Remove(productoCaducar);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductoCaducarExists(int id)
        {
            return _context.Productos_Caducar.Any(e => e.id_producto == id);
        }
    }
}
