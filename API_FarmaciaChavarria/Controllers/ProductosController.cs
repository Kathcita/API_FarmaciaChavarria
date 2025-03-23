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

namespace API_FarmaciaChavarria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos.ToListAsync();
        }

        // GET: api/Productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDTO>> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            var prod = new ProductoDTO
            {
                id_producto = producto.id_producto,
                id_categoria = producto.id_categoria,
                id_laboratorio = producto.id_laboratorio,
                precio = producto.precio,
                fecha_vencimiento = producto.fecha_vencimiento,
                nombre = producto.nombre,
                stock = producto.stock
            };

            return prod;
        }

        // PUT: api/Productos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, ProductoDTO productoDTO)
        {
            var producto = new Producto
            {
                id_producto = productoDTO.id_producto,
                nombre = productoDTO.nombre,
                id_categoria = productoDTO.id_categoria,
                id_laboratorio = productoDTO.id_laboratorio,
                precio = productoDTO.precio,
                stock = productoDTO.stock,
                fecha_vencimiento = productoDTO.fecha_vencimiento
            };

            if (id != producto.id_producto)
            {
                return BadRequest();
            }

            _context.Entry(producto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
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

        // POST: api/Productos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(ProductoDTO productoDTO)
        {
            var producto = new Producto
            {
                nombre = productoDTO.nombre,
                id_categoria = productoDTO.id_categoria,
                id_laboratorio = productoDTO.id_laboratorio,
                precio = productoDTO.precio,
                stock = productoDTO.stock,
                fecha_vencimiento = productoDTO.fecha_vencimiento
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProducto", new { id = producto.id_producto }, producto);
        }

        // DELETE: api/Productos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.id_producto == id);
        }
    }
}
