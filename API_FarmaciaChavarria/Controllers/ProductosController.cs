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
        public async Task<ActionResult<IEnumerable<ProductoDetailedDTO>>> GetProductos()
        {
            var productos = await (from p in _context.Productos
                                   join c in _context.Categorias on p.id_categoria equals c.id_categoria
                                   join l in _context.Laboratorios on p.id_laboratorio equals l.id_laboratorio
                                   select new ProductoDetailedDTO
                                   {
                                       IdProducto = p.id_producto,
                                       Nombre = p.nombre,
                                       id_categoria = p.id_categoria,
                                       id_laboratorio = p.id_laboratorio,
                                       CategoriaNombre = c.nombre,
                                       LaboratorioNombre = l.nombre,
                                       Precio = p.precio,
                                       Stock = p.stock,
                                       FechaVencimiento = p.fecha_vencimiento
                                   }).ToListAsync();

            return productos;
        }


        // GET: api/Productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDetailedDTO>> GetProducto(int id)
        {
            var producto = await (from p in _context.Productos
                                  join c in _context.Categorias on p.id_categoria equals c.id_categoria
                                  join l in _context.Laboratorios on p.id_laboratorio equals l.id_laboratorio
                                  where p.id_producto == id
                                  select new ProductoDetailedDTO
                                  {
                                      IdProducto = p.id_producto,
                                      Nombre = p.nombre,
                                      id_categoria = p.id_categoria,
                                      id_laboratorio = p.id_laboratorio,
                                      CategoriaNombre = c.nombre,
                                      LaboratorioNombre = l.nombre,
                                      Precio = p.precio,
                                      Stock = p.stock,
                                      FechaVencimiento = p.fecha_vencimiento
                                  }).FirstOrDefaultAsync();

            if (producto == null)
            {
                return NotFound();
            }

            return producto;
        }

        // GET: api/Productos/nombre/ibuprofeno
        [HttpGet("nombre/{nombre}")]
        public async Task<ActionResult<IEnumerable<ProductoDetailedDTO>>> GetProductoByName(string nombre)
        {
            var producto = await (from p in _context.Productos
                                  join c in _context.Categorias on p.id_categoria equals c.id_categoria
                                  join l in _context.Laboratorios on p.id_laboratorio equals l.id_laboratorio
                                  where p.nombre == nombre
                                  select new ProductoDetailedDTO
                                  {
                                      IdProducto = p.id_producto,
                                      Nombre = p.nombre,
                                      id_categoria = p.id_categoria,
                                      id_laboratorio = p.id_laboratorio,
                                      CategoriaNombre = c.nombre,
                                      LaboratorioNombre = l.nombre,
                                      Precio = p.precio,
                                      Stock = p.stock,
                                      FechaVencimiento = p.fecha_vencimiento
                                  }).ToListAsync();

            if (producto == null)
            {
                return NotFound();
            }

            return producto;
        }

        // GET: api/Productos/nombre/ibuprofeno
        [HttpGet("categoria/{id}")]
        public async Task<ActionResult<IEnumerable<ProductoDetailedDTO>>> GetProductoByCategory(int id)
        {
            var producto = await (from p in _context.Productos
                                  join c in _context.Categorias on p.id_categoria equals c.id_categoria
                                  join l in _context.Laboratorios on p.id_laboratorio equals l.id_laboratorio
                                  where p.id_categoria == id
                                  select new ProductoDetailedDTO
                                  {
                                      IdProducto = p.id_producto,
                                      Nombre = p.nombre,
                                      id_categoria = p.id_categoria,
                                      id_laboratorio = p.id_laboratorio,
                                      CategoriaNombre = c.nombre,
                                      LaboratorioNombre = l.nombre,
                                      Precio = p.precio,
                                      Stock = p.stock,
                                      FechaVencimiento = p.fecha_vencimiento
                                  }).ToListAsync();

            if (producto == null)
            {
                return NotFound();
            }

            return producto;
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
