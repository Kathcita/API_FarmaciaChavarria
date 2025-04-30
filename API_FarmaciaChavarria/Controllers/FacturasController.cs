using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_FarmaciaChavarria.Context;
using API_FarmaciaChavarria.Models;
using API_FarmaciaChavarria.Models.Reporte_Models;
using System.Globalization;

namespace API_FarmaciaChavarria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FacturasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Facturas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Factura>>> GetFacturas()
        {
            return await _context.Facturas.ToListAsync();
        }

        [HttpGet("facturas-año")]
        public async Task<ActionResult<IEnumerable<Factura>>> GetFacturasPorAño(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin,
            [FromQuery] int userId = 0)
        {
            var query = _context.Facturas
         .Where(f => f.fecha_venta >= fechaInicio && f.fecha_venta <= fechaFin);

            // Solo filtrar por usuario si userId es diferente de 0
            if (userId != 0)
            {
                query = query.Where(f => f.id_usuario == userId);
            }

            return await query.ToListAsync();

        }

        [HttpGet("ventas-por-mes-año")]
        public async Task<ActionResult<IEnumerable<RevenueDataItem>>> GetVentasPorMesAño(
            [FromQuery] DateTime fechaInicio,
    [FromQuery] DateTime fechaFin,
    int userId = 0)
        {
            // Obtener todas las facturas del año seleccionado
            var query = _context.Facturas
        .Where(f => f.fecha_venta >= fechaInicio && f.fecha_venta <= fechaFin);

            if (userId != 0)
            {
                query = query.Where(u => u.id_usuario == userId);
            }

            var facturas = await query.ToListAsync();

            // Agrupar por mes y sumar los totales
            var ventasPorMes = facturas
    .GroupBy(f => new { f.fecha_venta.Year, f.fecha_venta.Month }) // Agrupar por año y mes
    .OrderBy(g => new DateTime(g.Key.Year, g.Key.Month, 1))         // Ordenar antes de proyectar
    .Select(g => new RevenueDataItem
    {
        Date = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key.Month)} {g.Key.Year}",
        Revenue = g.Sum(f => f.total)
    })
    .ToList();


            return Ok(ventasPorMes);
        }

        [HttpGet("top-laboratorios")]
        public async Task<ActionResult<IEnumerable<LaboratorioVentasDTO>>> GetTopLaboratorios(
    [FromQuery] DateTime? fechaInicio = null,
    [FromQuery] DateTime? fechaFin = null,
    [FromQuery] int userId = 0)
        {
            // Consulta base con joins y filtros opcionales
            var query = _context.Facturas
                .Join(
                    _context.Detalle_Facturas,
                    f => f.id_factura,
                    df => df.id_factura,
                    (f, df) => new { Factura = f, Detalle = df }
                )
                .Join(
                    _context.Productos,
                    fd => fd.Detalle.id_producto,
                    p => p.id_producto,
                    (fd, p) => new { fd.Factura, fd.Detalle, Producto = p }
                )
                .Join(
                    _context.Laboratorios,
                    fdp => fdp.Producto.id_laboratorio,
                    l => l.id_laboratorio,
                    (fdp, l) => new { fdp.Factura, fdp.Detalle, fdp.Producto, Laboratorio = l }
                );

            // Filtros opcionales
            if (fechaInicio != null && fechaFin != null)
            {
                query = query.Where(x => x.Factura.fecha_venta >= fechaInicio && x.Factura.fecha_venta <= fechaFin);
            }

            if (userId != 0)
            {
                query = query.Where(x => x.Factura.id_usuario == userId);
            }

            // Agrupar por laboratorio y calcular total de ventas
            var resultado = await query
    .GroupBy(x => new { x.Laboratorio.id_laboratorio, x.Laboratorio.nombre })
    .Select(g => new LaboratorioVentasDTO
    {
        IdLaboratorio = g.Key.id_laboratorio,
        NombreLaboratorio = g.Key.nombre,
        TotalVentas = g.Sum(x => x.Detalle.cantidad * x.Detalle.precio_unitario) // ¡Calculado aquí!
    })
    .OrderByDescending(x => x.TotalVentas)
    .Take(10)
    .ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("top-categorias")]
        public async Task<ActionResult<IEnumerable<CategoriaVentasDTO>>> GetTopCategorias(
    [FromQuery] DateTime? fechaInicio = null,
    [FromQuery] DateTime? fechaFin = null,
    [FromQuery] int userId = 0)
        {
            // Consulta base con joins y filtros opcionales
            var query = _context.Facturas
                .Join(
                    _context.Detalle_Facturas,
                    f => f.id_factura,
                    df => df.id_factura,
                    (f, df) => new { Factura = f, Detalle = df }
                )
                .Join(
                    _context.Productos,
                    fd => fd.Detalle.id_producto,
                    p => p.id_producto,
                    (fd, p) => new { fd.Factura, fd.Detalle, Producto = p }
                )
                .Join(
                    _context.Categorias,
                    fdp => fdp.Producto.id_categoria,
                    c => c.id_categoria,
                    (fdp, c) => new { fdp.Factura, fdp.Detalle, fdp.Producto, Categoria = c }
                );

            // Filtros opcionales
            if (fechaInicio != null && fechaFin != null)
            {
                query = query.Where(x => x.Factura.fecha_venta >= fechaInicio && x.Factura.fecha_venta <= fechaFin);
            }

            if (userId != 0)
            {
                query = query.Where(x => x.Factura.id_usuario == userId);
            }

            // Agrupar por categoría y calcular total de ventas
            var resultado = await query
    .GroupBy(x => new { x.Categoria.id_categoria, x.Categoria.nombre })
    .Select(g => new CategoriaVentasDTO
    {
        IdCategoria = g.Key.id_categoria,
        NombreCategoria = g.Key.nombre,
        TotalVentas = g.Sum(x => x.Detalle.cantidad * x.Detalle.precio_unitario) // ¡Calculado aquí!
    })
    .OrderByDescending(x => x.TotalVentas)
    .Take(10)
    .ToListAsync();

            return Ok(resultado);
        }

        [HttpGet("top-productos")]
        public async Task<ActionResult<IEnumerable<ProductoVentasDTO>>> GetTopProductos(
    [FromQuery] DateTime? fechaInicio = null,
    [FromQuery] DateTime? fechaFin = null,
    [FromQuery] int userId = 0)
        {
            var query = _context.Facturas
                .Join(
                    _context.Detalle_Facturas,
                    f => f.id_factura,
                    df => df.id_factura,
                    (f, df) => new { Factura = f, Detalle = df }
                )
                .Join(
                    _context.Productos,
                    fd => fd.Detalle.id_producto,
                    p => p.id_producto,
                    (fd, p) => new { fd.Factura, fd.Detalle, Producto = p }
                );

            // Filtros opcionales
            if (fechaInicio != null && fechaFin != null)
            {
                query = query.Where(x => x.Factura.fecha_venta >= fechaInicio && x.Factura.fecha_venta <= fechaFin);
            }

            if (userId != 0)
            {
                query = query.Where(x => x.Factura.id_usuario == userId);
            }

            // Agrupar por categoría y calcular total de ventas
            var resultado = await query
    .GroupBy(x => new { x.Producto.id_producto, x.Producto.nombre })
    .Select(g => new ProductoVentasDTO
    {
        IdProducto = g.Key.id_producto,
        NombreProducto = g.Key.nombre,
        TotalVentas = g.Sum(x => x.Detalle.cantidad * x.Detalle.precio_unitario) // ¡Calculado aquí!
    })
    .OrderByDescending(x => x.TotalVentas)
    .Take(15)
    .ToListAsync();

            return Ok(resultado);
        }

            // GET: api/Facturas/5
            [HttpGet("{id}")]
        public async Task<ActionResult<Factura>> GetFactura(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);

            if (factura == null)
            {
                return NotFound();
            }

            return factura;
        }

        // PUT: api/Facturas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFactura(int id, Factura factura)
        {
            if (id != factura.id_factura)
            {
                return BadRequest();
            }

            _context.Entry(factura).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FacturaExists(id))
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

        // POST: api/Facturas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Factura>> PostFactura(Factura factura)
        {
            _context.Facturas.Add(factura);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFactura", new { id = factura.id_factura }, factura);
        }

        // DELETE: api/Facturas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFactura(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);
            if (factura == null)
            {
                return NotFound();
            }

            _context.Facturas.Remove(factura);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FacturaExists(int id)
        {
            return _context.Facturas.Any(e => e.id_factura == id);
        }
    }
}
