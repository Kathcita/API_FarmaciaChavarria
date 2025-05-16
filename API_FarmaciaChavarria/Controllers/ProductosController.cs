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
using API_FarmaciaChavarria.Models.PaginationModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Drawing.Printing;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ProductoPagedResult>> GetProductos([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {
            var query = from p in _context.Productos
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
                            Stock_Minimo = p.stock_minimo,
                            Efectos_secundarios = p.efectos_secundarios,
                            Como_usar = p.como_usar,
                            FechaVencimiento = p.fecha_vencimiento
                        };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var productos = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new ProductoPagedResult
            {
                Productos = productos,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };

            return Ok(result);
        }

        // GET: api/Productos
        [Authorize]
        [HttpGet("medicamentos-escasos")]
        public async Task<ActionResult<ProductoPagedResult>> GetProductosStockEscaso([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {
            var query = from p in _context.Productos
                        
                        join c in _context.Categorias on p.id_categoria equals c.id_categoria
                        join l in _context.Laboratorios on p.id_laboratorio equals l.id_laboratorio
                        where p.stock_minimo >= p.stock
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
                            Stock_Minimo = p.stock_minimo,
                            Efectos_secundarios = p.efectos_secundarios,
                            Como_usar = p.como_usar,
                            FechaVencimiento = p.fecha_vencimiento
                        };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var productos = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new ProductoPagedResult
            {
                Productos = productos,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };

            return Ok(result);
        }


        // GET: api/Productos/5
        [Authorize]
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
                                      Stock_Minimo = p.stock_minimo,
                                      Efectos_secundarios = p.efectos_secundarios,
                                      Como_usar = p.como_usar,
                                      FechaVencimiento = p.fecha_vencimiento
                                  }).FirstOrDefaultAsync();

            if (producto == null)
            {
                return NotFound();
            }

            return producto;
        }

        // GET: api/Productos/nombre/ibuprofeno
        [Authorize]
        [HttpGet("nombre/{nombre}")]
        public async Task<ActionResult<ProductoPagedResult>> GetProductoByName(string nombre,[FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {
            var query = from p in _context.Productos
                                  join c in _context.Categorias on p.id_categoria equals c.id_categoria
                                  join l in _context.Laboratorios on p.id_laboratorio equals l.id_laboratorio
                                  where p.nombre.ToLower().Contains(nombre.ToLower())
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
                                      Stock_Minimo = p.stock_minimo,
                                      Efectos_secundarios = p.efectos_secundarios,
                                      Como_usar = p.como_usar,
                                      FechaVencimiento = p.fecha_vencimiento
                                  };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var productos = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new ProductoPagedResult
            {
                Productos = productos,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };

            return Ok(result);
        }

        // GET: api/Productos/categoria/1
        [Authorize]
        [HttpGet("categoria/{id}")]
        public async Task<ActionResult<ProductoPagedResult>> GetProductoByCategory(int id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {

            var query = from p in _context.Productos
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
                                      Stock_Minimo = p.stock_minimo,
                                      Efectos_secundarios = p.efectos_secundarios,
                                      Como_usar = p.como_usar,
                                      FechaVencimiento = p.fecha_vencimiento
                                  };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var productos = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new ProductoPagedResult
            {
                Productos = productos,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };

            return Ok(result);
        }

        // GET: api/Productos/categoría/1/nombre/ibuprofeno
        [Authorize]
        [HttpGet("categoria/{id}/nombre/{nombre}")]
        public async Task<ActionResult<ProductoPagedResult>> GetProductoByNameAndByCategory(int id, string nombre, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {

            var query = from p in _context.Productos
                        join c in _context.Categorias on p.id_categoria equals c.id_categoria
                        join l in _context.Laboratorios on p.id_laboratorio equals l.id_laboratorio
                        where p.id_categoria == id && p.nombre.ToLower().Contains(nombre.ToLower())
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
                            Stock_Minimo = p.stock_minimo,
                            Efectos_secundarios = p.efectos_secundarios,
                            Como_usar = p.como_usar,
                            FechaVencimiento = p.fecha_vencimiento
                        };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var productos = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new ProductoPagedResult
            {
                Productos = productos,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };

            return Ok(result);
        }

        [Authorize]
        [HttpGet("productosPorCadudar")]
        public async Task<ActionResult<ProductoPagedResult>> GetProductosPorCadudar([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Now);
            var dentroDeTresMeses = hoy.AddMonths(3);

            var query = from p in _context.Productos
                        join c in _context.Categorias on p.id_categoria equals c.id_categoria
                        join l in _context.Laboratorios on p.id_laboratorio equals l.id_laboratorio
                        where p.fecha_vencimiento >= hoy && p.fecha_vencimiento <= dentroDeTresMeses
                        orderby p.fecha_vencimiento ascending
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
                            Stock_Minimo = p.stock_minimo,
                            Efectos_secundarios = p.efectos_secundarios,
                            Como_usar = p.como_usar,
                            FechaVencimiento = p.fecha_vencimiento
                        };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var productos = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new ProductoPagedResult
            {
                Productos = productos,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };

            return Ok(result);
        }

        // PUT: api/Productos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
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
                stock_minimo = productoDTO.stock_minimo,
                efectos_secundarios = productoDTO.efectos_secundarios,
                como_usar = productoDTO.como_usar,
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
        [Authorize]
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
                stock_minimo = productoDTO.stock_minimo,
                fecha_vencimiento = productoDTO.fecha_vencimiento,
                efectos_secundarios = productoDTO.efectos_secundarios,
                como_usar = productoDTO.como_usar,
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProducto", new { id = producto.id_producto }, producto);
        }

        // DELETE: api/Productos/5
        [Authorize]
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
