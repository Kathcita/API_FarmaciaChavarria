using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_FarmaciaChavarria.Context;
using API_FarmaciaChavarria.Models;
using API_FarmaciaChavarria.Controllers;
using System.Diagnostics;


[ApiController]
[Route("api/[controller]")]
public class RecuperarPinController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly CorreoService _correo;

    public RecuperarPinController(AppDbContext context, CorreoService correo)
    {
        _context = context;
        _correo = correo;
    }

    [HttpPost]
    public IActionResult RecuperarPin([FromBody] RecuperarPinRequest request)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.nombre == request.Usuario);

        if (usuario == null)
        {
            return NotFound(new { mensaje = "El usuario no existe." });
        }

        int nuevoPin = new Random().Next(1000, 9999);
        usuario.pin = nuevoPin;
        _context.SaveChanges();

        _correo.EnviarPin(request.Email, nuevoPin);

        return Ok(new { mensaje = "El PIN ha sido enviado al correo proporcionado." });
    }

    public class RecuperarPinRequest
    {
        public string Usuario { get; set; }
        public string Email { get; set; }
    }
}