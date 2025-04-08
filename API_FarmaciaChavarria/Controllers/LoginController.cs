using API_FarmaciaChavarria.Context;
using API_FarmaciaChavarria.Models;
using API_FarmaciaChavarria.ModelsDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_FarmaciaChavarria.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;
        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            var usuario = await _context.Usuarios
        .FirstOrDefaultAsync(u => u.nombre == userLogin.nombre);

            if (usuario == null)
            {
                return Unauthorized("Usuario no encontrado");
            }

            if (usuario.pin != userLogin.pin)
            {
                return Unauthorized("Contraseña incorrecta");
            }

            return Ok(usuario);
        }
    }
}
