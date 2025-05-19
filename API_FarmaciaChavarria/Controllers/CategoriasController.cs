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
using Microsoft.AspNetCore.Authorization;

namespace API_FarmaciaChavarria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Categorias
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaPageResult>>> GetCategoria([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {


            var query =  _context.Categorias.AsQueryable();

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var categorias = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new CategoriaPageResult
            {
                Categorias = categorias,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };

            return Ok(result);
        }

        // GET: api/Categorias/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDTO>> GetCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
            {
                return NotFound();
            }

            var categ = new CategoriaDTO
            {
                id_categoria = categoria.id_categoria,
                nombre = categoria.nombre
            };

            return categ;
        }

        // GET: api/Categorias/nombre/jarabe
        [Authorize]
        [HttpGet("nombre/{nombre}")]
        public async Task<ActionResult<CategoriaPageResult>> GetCategoriaByNombre(string nombre, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {
            var query = from c in _context.Categorias
                                   where c.nombre.ToLower().Contains(nombre.ToLower()) 
                                   select new Categoria
                                   {
                                       id_categoria = c.id_categoria,
                                       nombre = c.nombre
                                   };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var categorias = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new CategoriaPageResult
            {
                Categorias = categorias,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };

            return Ok(result);
        }

        // PUT: api/Categorias/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(int id, CategoriaDTO categoria)
        {
            if (id != categoria.id_categoria)
            {
                return BadRequest();
            }

            if (categoria.nombre == "")
            {
                return BadRequest("El campo nombre de categoría no puede estar vacío");
            }

            var categ = new Categoria
            {
                id_categoria = categoria.id_categoria,
                nombre = categoria.nombre
            };

            _context.Entry(categ).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaExists(id))
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

        // POST: api/Categorias
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Categoria>> PostCategoria(CategoriaDTO categoria)
        {
            if (categoria.nombre == "")
            {
                return BadRequest("El campo nombre de categoría no puede estar vacío");
            }

            var categ = new Categoria
            {
                id_categoria = categoria.id_categoria,
                nombre = categoria.nombre
            };

            _context.Categorias.Add(categ);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategoria", new { id = categoria.id_categoria }, categoria);
        }

        // DELETE: api/Categorias/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoriaExists(int id)
        {
            return _context.Categorias.Any(e => e.id_categoria == id);
        }
    }
}
