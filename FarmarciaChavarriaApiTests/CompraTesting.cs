using API_FarmaciaChavarria.Context;
using API_FarmaciaChavarria.Controllers;
using API_FarmaciaChavarria.Models;
using API_FarmaciaChavarria.ModelsDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmarciaChavarriaApiTests
{
    public class CompraTesting
    {
        private AppDbContext GetDbContextConDatosPrueba()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            // Datos de prueba
            context.Compras.AddRange(
                new Compra { id_compra = 1, id_proveedor = 1, fecha_compra = DateTime.Today, total = 1000 },
                new Compra { id_compra = 2, id_proveedor = 2, fecha_compra = DateTime.Today, total = 1000 },
                new Compra { id_compra = 3, id_proveedor = 2, fecha_compra = DateTime.Today, total = 1000 }
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
        public async Task GetCompras_DeberiaRetornarUnaListaDeCompras()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new ComprasController(context);

            // Act
            var result = await controller.GetCompras();

            // Assert
            var compras = Assert.IsType<List<Compra>>(result.Value);
            Assert.NotEmpty(compras);
        }

        [Fact]
        public async Task GetCompras_DeberiaRetornarListaVaciaCuandoNoHayCompras()
        {
            // Arrange
            var context = GetDbContextSinDatos(); // DB sin datos
            var controller = new ComprasController(context);

            // Act
            var result = await controller.GetCompras();

            // Assert
            var compras = Assert.IsType<List<Compra>>(result.Value);

            Assert.Empty(compras);
        }

        [Fact]
        public async Task GetCompra_DeberiaRetornarUnaCompraFiltradoPorId()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new ComprasController(context);

            // Act
            var result = await controller.GetCompra(1);

            // Assert
            var compra = Assert.IsType<Compra>(result.Value);
            Assert.Equal(1, compra.id_compra);

        }

        [Fact]
        public async Task GetCompra_DeberiaRetornarUnNotFound()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new ComprasController(context);

            // Act
            var result = await controller.GetCompra(10);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);

        }

        [Fact]
        public async Task PostCompra_DeberiaCrearCompraYRetornarCreatedAtAction()
        {
            // Arrange
            var context = GetDbContextSinDatos(); // DB vacía
            var controller = new ComprasController(context);
            var nuevaCompra = new Compra { id_compra = 10, id_proveedor = 1, fecha_compra = DateTime.Today, total = 1000 };

            // Act
            var result = await controller.PostCompra(nuevaCompra);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var compraDevuelta = Assert.IsType<Compra>(createdResult.Value);
            Assert.Equal(nuevaCompra.id_compra, compraDevuelta.id_compra);

            var compraEnDb = await context.Compras.FindAsync(compraDevuelta.id_compra);
            Assert.NotNull(compraEnDb);
        }

        [Fact]
        public async Task PutCompra_DeberiaActualizarCompraYRetornarNoContent()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();

            // Desanclar manualmente la entidad que se insertó en GetDbContextConDatosPrueba
            var local = context.Compras.Local.FirstOrDefault(c => c.id_compra == 1);
            if (local != null)
                context.Entry(local).State = EntityState.Detached;

            var controller = new ComprasController(context);
            var compraActualizada = new Compra { id_compra = 1, id_proveedor = 1, fecha_compra = DateTime.Today, total = 2000};

            // Act
            var result = await controller.PutCompra(1, compraActualizada);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var compraEnDb = await context.Compras.FindAsync(1);
            Assert.NotNull(compraEnDb);
        }

        [Fact]
        public async Task PutCompra_DeberiaRetornarBadRequestSiIdNoCoincide()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();

            var local = context.Compras.Local.FirstOrDefault(c => c.id_compra == 1);

            var controller = new ComprasController(context);
            var compraConOtroId = new Compra { id_compra = 99, id_proveedor = 1, fecha_compra = DateTime.Today, total = 2000 };

            // Act
            var result = await controller.PutCompra(1, compraConOtroId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteCompra_DeberiaEliminarCompraYRetornarNoContent()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new ComprasController(context);

            // Act
            var result = await controller.DeleteCompra(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await context.Compras.FindAsync(1));
        }

        [Fact]
        public async Task DeleteCompra_DeberiaRetornarNotFoundSiNoExiste()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            var controller = new ComprasController(context);

            // Act
            var result = await controller.DeleteCompra(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

    }
}
