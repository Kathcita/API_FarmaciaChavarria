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

namespace API_FarmaciaChavarria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaboratoriosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LaboratoriosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Laboratorios
        [HttpGet]
        public async Task<ActionResult<LaboratorioPagedResult>> GetLaboratorio([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 2)
        {
            var query = _context.Laboratorios.AsQueryable();

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var laboratorios = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new LaboratorioPagedResult
            {
                Laboratorios = laboratorios,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };

            return Ok(result);
        }

        // GET: api/Laboratorios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LaboratorioDTO>> GetLaboratorio(int id)
        {
            var laboratorio = await _context.Laboratorios.FindAsync(id);

            if (laboratorio == null)
            {
                return NotFound();
            }

            var laboratorioDTO = new LaboratorioDTO
            {
                id_laboratorio = laboratorio.id_laboratorio,
                nombre = laboratorio.nombre
            };

            return laboratorioDTO;
        }

        // GET: api/Laboratorios/nombre/galo
        [HttpGet("nombre/{nombre}")]
        public async Task<ActionResult<LaboratorioPagedResult>> GetLaboratorioByNombre(string nombre, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 2)
        {
            var query = from c in _context.Laboratorios
                                   where c.nombre.ToLower().Contains(nombre.ToLower())
                                   select new Laboratorio
                                   {
                                       id_laboratorio = c.id_laboratorio,
                                       nombre = c.nombre
                                   };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var laboratorios = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new LaboratorioPagedResult
            {
                Laboratorios = laboratorios,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };

            return Ok(result);
        }

        // PUT: api/Laboratorios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLaboratorio(int id, LaboratorioDTO laboratorioDTO)
        {
            var laboratorio = new Laboratorio
            {
                id_laboratorio = laboratorioDTO.id_laboratorio,
                nombre = laboratorioDTO.nombre
            };


            if (id != laboratorio.id_laboratorio)
            {
                return BadRequest();
            }

            _context.Entry(laboratorio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LaboratorioExists(id))
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

        // POST: api/Laboratorios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Laboratorio>> PostLaboratorio(LaboratorioDTO LaboratorioDTO)
        {

            var laboratorio = new Laboratorio
            {
                id_laboratorio = LaboratorioDTO.id_laboratorio,
                nombre = LaboratorioDTO.nombre
            };

            _context.Laboratorios.Add(laboratorio);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLaboratorio", new { id = laboratorio.id_laboratorio }, laboratorio);
        }

        // DELETE: api/Laboratorios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLaboratorio(int id)
        {
            var laboratorio = await _context.Laboratorios.FindAsync(id);
            if (laboratorio == null)
            {
                return NotFound();
            }

            _context.Laboratorios.Remove(laboratorio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LaboratorioExists(int id)
        {
            return _context.Laboratorios.Any(e => e.id_laboratorio == id);
        }
    }
}
