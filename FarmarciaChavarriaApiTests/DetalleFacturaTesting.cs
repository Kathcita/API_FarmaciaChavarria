using API_FarmaciaChavarria.Context;
using API_FarmaciaChavarria.Controllers;
using API_FarmaciaChavarria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmarciaChavarriaApiTests
{
    public class DetalleFacturaTesting
    {
        private AppDbContext GetDbContextConDatosPrueba()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            // Datos de prueba
            context.Detalle_Facturas.AddRange(
                new DetalleFactura { id_detalle = 1,id_factura = 1, id_producto = 1, cantidad = 1, precio_unitario = 100 },
                new DetalleFactura { id_detalle = 2,id_factura = 2, id_producto = 2, cantidad = 1, precio_unitario = 50 },
                new DetalleFactura { id_detalle = 3,id_factura = 3, id_producto = 2, cantidad = 1, precio_unitario = 50 }
            );

            context.SaveChanges();

            return context;
        }

        private AppDbContext GetDbContextSinDatos()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Para que cada test tenga su propia DB
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetDetalleFacturas_DeberiaRetornarUnaListaDeDetallesDeFacturas()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new DetalleFacturasController(context);

            // Act
            var result = await controller.GetDetalleFactura();

            // Assert
            var detalleFactura = Assert.IsType<List<DetalleFactura>>(result.Value);
            Assert.NotEmpty(detalleFactura);
        }

        [Fact]
        public async Task GetDetalleFacturas_DeberiaRetornarListaVaciaCuandoNoHayDetallesDeFacturas()
        {
            // Arrange
            var context = GetDbContextSinDatos(); // DB sin datos
            var controller = new DetalleFacturasController(context);

            // Act
            var result = await controller.GetDetalleFactura();

            // Assert
            var detallesFacturas = Assert.IsType<List<DetalleFactura>>(result.Value);

            Assert.Empty(detallesFacturas);
        }

        [Fact]
        public async Task GetDetalleFactura_DeberiaRetornarUnDetalleDeFacturaFiltradoPorId()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new DetalleFacturasController(context);

            // Act
            var result = await controller.GetDetalleFactura(1);

            // Assert
            var detalleFactura = Assert.IsType<DetalleFactura>(result.Value);
            Assert.Equal(1, detalleFactura.id_detalle);

        }

        [Fact]
        public async Task GetDetalleFactura_DeberiaRetornarUnNotFound()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new DetalleFacturasController(context);

            // Act
            var result = await controller.GetDetalleFactura(10);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostDetalleFactura_DeberiaCrearDetalleFacturaYRetornarCreatedAtAction()
        {
            // Arrange
            var context = GetDbContextSinDatos(); // DB vacía
            var controller = new DetalleFacturasController(context);
            var nuevoDetalleFactura = new DetalleFactura { id_detalle = 10, id_factura = 1, id_producto = 1, cantidad = 1, precio_unitario = 100 };

            // Act
            var result = await controller.PostDetalleFactura(nuevoDetalleFactura);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var detalleFacturaDevuelto = Assert.IsType<DetalleFactura>(createdResult.Value);
            Assert.Equal(nuevoDetalleFactura.id_detalle, detalleFacturaDevuelto.id_detalle);

            var detalleFacturaEnDb = await context.Detalle_Facturas.FindAsync(detalleFacturaDevuelto.id_detalle);
            Assert.NotNull(detalleFacturaEnDb);
        }

        [Fact]
        public async Task PutDetalleFactura_DeberiaActualizarDetalleFacturaYRetornarNoContent()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();

            // Desanclar manualmente la entidad que se insertó en GetDbContextConDatosPrueba
            var local = context.Detalle_Facturas.Local.FirstOrDefault(c => c.id_detalle == 1);
            if (local != null)
                context.Entry(local).State = EntityState.Detached;

            var controller = new DetalleFacturasController(context);
            var detalleFacturaActualizada = new DetalleFactura { id_detalle = 1, id_factura = 1, id_producto = 1, cantidad = 1, precio_unitario = 100 };

            // Act
            var result = await controller.PutDetalleFactura(1, detalleFacturaActualizada);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var detalleFacturaEnDb = await context.Detalle_Facturas.FindAsync(1);
            Assert.NotNull(detalleFacturaEnDb);
        }

        [Fact]
        public async Task PutDetalleFactura_DeberiaRetornarBadRequestSiIdNoCoincide()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();

            var local = context.Detalle_Facturas.Local.FirstOrDefault(c => c.id_detalle == 1);

            var controller = new DetalleFacturasController(context);
            var detalleFacturaConOtroId = new DetalleFactura { id_detalle = 99, id_factura = 1, id_producto = 1, cantidad = 1, precio_unitario = 100 };

            // Act
            var result = await controller.PutDetalleFactura(1, detalleFacturaConOtroId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteDetalleFactura_DeberiaEliminarDetalleFacturaYRetornarNoContent()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new DetalleFacturasController(context);

            // Act
            var result = await controller.DeleteDetalleFactura(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await context.Detalle_Facturas.FindAsync(1));
        }

        [Fact]
        public async Task DeleteDetalleFacturas_DeberiaRetornarNotFoundSiNoExiste()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            var controller = new DetalleFacturasController(context);

            // Act
            var result = await controller.DeleteDetalleFactura(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

    }
}
