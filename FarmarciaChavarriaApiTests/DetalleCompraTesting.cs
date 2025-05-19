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
    public class DetalleCompraTesting
    {
        private AppDbContext GetDbContextConDatosPrueba()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            // Datos de prueba
            context.Detalle_Compras.AddRange(
                new DetalleCompra { id_detalle = 1,id_compra = 1, id_producto = 1, cantidad = 2, precio_unitario = 150 },
                new DetalleCompra { id_detalle = 2, id_compra = 1, id_producto = 2, cantidad = 1, precio_unitario = 200 },
                new DetalleCompra { id_detalle = 3, id_compra = 3, id_producto = 1, cantidad = 2, precio_unitario = 150 }
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
        public async Task GetDetalleCompras_DeberiaRetornarUnaListaDeDetalleCompras()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new DetalleComprasController(context);

            // Act
            var result = await controller.GetDetalle_Compras();

            // Assert
            var detalleCompras = Assert.IsType<List<DetalleCompra>>(result.Value);
            Assert.NotEmpty(detalleCompras);
        }

        [Fact]
        public async Task GetDetalleCompras_DeberiaRetornarListaVaciaCuandoNoHayDetallesDeCompras()
        {
            // Arrange
            var context = GetDbContextSinDatos(); // DB sin datos
            var controller = new DetalleComprasController(context);

            // Act
            var result = await controller.GetDetalle_Compras();

            // Assert
            var detalleCompras = Assert.IsType<List<DetalleCompra>>(result.Value);

            Assert.Empty(detalleCompras);
        }

        [Fact]
        public async Task GetDetalleCompras_DeberiaRetornarUnDetalleCompraFiltradoPorId()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new DetalleComprasController(context);

            // Act
            var result = await controller.GetDetalleCompra(1);

            // Assert
            var detalleCompra = Assert.IsType<DetalleCompra>(result.Value);
            Assert.Equal(1, detalleCompra.id_detalle);

        }

        [Fact]
        public async Task GetDetalleCompra_DeberiaRetornarUnNotFound()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new DetalleComprasController(context);

            // Act
            var result = await controller.GetDetalleCompra(10);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);

        }

        [Fact]
        public async Task PostDetalleCompra_DeberiaCrearDetalleCompraYRetornarCreatedAtAction()
        {
            // Arrange
            var context = GetDbContextSinDatos(); // DB vacía
            var controller = new DetalleComprasController(context);
            var nuevoDetalleCompra = new DetalleCompra { id_detalle = 5, id_compra = 2, id_producto = 1, cantidad = 2, precio_unitario = 150 };

            // Act
            var result = await controller.PostDetalleCompra(nuevoDetalleCompra);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var detalleCompraDevuelto = Assert.IsType<DetalleCompra>(createdResult.Value);
            Assert.Equal(nuevoDetalleCompra.id_detalle, detalleCompraDevuelto.id_detalle);

            var detalleCompraEnDb = await context.Detalle_Compras.FindAsync(detalleCompraDevuelto.id_detalle);
            Assert.NotNull(detalleCompraEnDb);
        }

        [Fact]
        public async Task PutDetalleCompra_DeberiaActualizarDetalleCompraYRetornarNoContent()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();

            // Desanclar manualmente la entidad que se insertó en GetDbContextConDatosPrueba
            var local = context.Detalle_Compras.Local.FirstOrDefault(c => c.id_detalle == 1);
            if (local != null)
                context.Entry(local).State = EntityState.Detached;

            var controller = new DetalleComprasController(context);
            var detalleCompraActualizada = new DetalleCompra { id_detalle = 1, id_compra = 2, id_producto = 1, cantidad = 2, precio_unitario = 150 };

            // Act
            var result = await controller.PutDetalleCompra(1, detalleCompraActualizada);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var detalleCompraEnDb = await context.Detalle_Compras.FindAsync(1);
            Assert.NotNull(detalleCompraEnDb);
        }


        [Fact]
        public async Task PutDetalleCompra_DeberiaRetornarBadRequestSiIdNoCoincide()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();

            var local = context.Detalle_Compras.Local.FirstOrDefault(c => c.id_detalle == 1);

            var controller = new DetalleComprasController(context);
            var detalleCompraConOtroId = new DetalleCompra { id_detalle = 99, id_compra = 2, id_producto = 1, cantidad = 2, precio_unitario = 150 };

            // Act
            var result = await controller.PutDetalleCompra(1, detalleCompraConOtroId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteDetalleCompra_DeberiaEliminarDetalleCompraYRetornarNoContent()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new DetalleComprasController(context);

            // Act
            var result = await controller.DeleteDetalleCompra(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await context.Detalle_Compras.FindAsync(1));
        }

        [Fact]
        public async Task DeleteDetalleCompra_DeberiaRetornarNotFoundSiNoExiste()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            var controller = new DetalleComprasController(context);

            // Act
            var result = await controller.DeleteDetalleCompra(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}