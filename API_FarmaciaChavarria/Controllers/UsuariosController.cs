using API_FarmaciaChavarria.Context;
using API_FarmaciaChavarria.Models;
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
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [EnableRateLimiting("globalLimiter")]
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/5
        [EnableRateLimiting("globalLimiter")]
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDTO>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            var user = new UsuarioDTO
            {
                id_usuario = usuario.id_usuario,
                nombre = usuario.nombre,
                rol = usuario.rol,
                pin = usuario.pin
            };

            return user;
        }

        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [EnableRateLimiting("globalLimiter")]
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, UsuarioDTO usuarioDTO)
        {

            if (usuarioDTO.nombre == "")
            {
                return BadRequest("El campo nombre de usuario no puede estar vacío");
            }

            if (usuarioDTO.pin.ToString().Length != 4)
            {
                return BadRequest("El campo pin debe constar de 4 dígitos");
            }

            var usuario = new Usuario
            {
                id_usuario = usuarioDTO.id_usuario,
                nombre = usuarioDTO.nombre,
                rol = usuarioDTO.rol,
                pin = usuarioDTO.pin
            };

            if (id != usuario.id_usuario)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
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

        // POST: api/Usuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [EnableRateLimiting("globalLimiter")]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(UsuarioDTO usuarioDTO)
        {
            if (usuarioDTO.nombre == "")
            {
                return BadRequest("El campo nombre de usuario no puede estar vacío");
            }

            if (usuarioDTO.pin.ToString().Length != 4)
            {
                return BadRequest("El campo pin debe constar de 4 dígitos");
            }

            var usuario = new Usuario
            {
                id_usuario = usuarioDTO.id_usuario,
                nombre = usuarioDTO.nombre,
                rol = usuarioDTO.rol,
                pin = usuarioDTO.pin
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.id_usuario }, usuario);
        }

        // DELETE: api/Usuarios/5
        [EnableRateLimiting("globalLimiter")]
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.id_usuario == id);
        }
    }
}
