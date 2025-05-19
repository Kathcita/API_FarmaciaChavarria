using API_FarmaciaChavarria.Context;
using API_FarmaciaChavarria.Controllers;
using API_FarmaciaChavarria.Models;
using API_FarmaciaChavarria.Models.PaginationModels;
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
    public class ProveedorTesting
    {
        private AppDbContext GetDbContextConDatosPrueba()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            // Datos de prueba
            context.Proveedores.AddRange(
                new Proveedor { id_proveedor = 1, nombre = "JomWest", telefono = "99228799", direccion = "Parque" },
                new Proveedor { id_proveedor = 2, nombre = "Horell IsReal", telefono = "22334455", direccion = "Colina Sur" },
                new Proveedor {id_proveedor = 3, nombre = "Yared", telefono = "77889966", direccion = "Cotran Norte" }
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
        public async Task GetProveedores_DeberiaRetornarUnaListaDeProveedores()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new ProveedorsController(context);

            // Act
            var result = await controller.GetProveedores();

            // Assert
            var proveedores = Assert.IsType<List<Proveedor>>(result.Value);
            Assert.NotEmpty(proveedores);
        }

        [Fact]
        public async Task GetProveedores_DeberiaRetornarListaVaciaCuandoNoHayProveedores()
        {
            // Arrange
            var context = GetDbContextSinDatos(); // DB sin datos
            var controller = new ProveedorsController(context);

            // Act
            var result = await controller.GetProveedores();

            // Assert
            var proveedores = Assert.IsType<List<Proveedor>>(result.Value);

            Assert.Empty(proveedores);
        }

        [Fact]
        public async Task GetProveedor_DeberiaRetornarUnProveedorFiltradoPorId()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new ProveedorsController(context);

            // Act
            var result = await controller.GetProveedor(1);

            // Assert
            var proveedor = Assert.IsType<ProveedorDTO>(result.Value);
            Assert.Equal(1, proveedor.id_proveedor);

        }

        [Fact]
        public async Task GetProveedor_DeberiaRetornarUnNotFound()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new ProveedorsController(context);

            // Act
            var result = await controller.GetProveedor(10);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);

        }

        [Fact]
        public async Task PostProveedor_DeberiaCrearProveedorYRetornarCreatedAtAction()
        {
            // Arrange
            var context = GetDbContextSinDatos(); // DB vacía
            var controller = new ProveedorsController(context);
            var nuevoProveedor = new ProveedorDTO { id_proveedor = 10, nombre = "Katerina", telefono = "99880055", direccion = "La Cuesta" };

            // Act
            var result = await controller.PostProveedor(nuevoProveedor);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var proveedorDevuelto = Assert.IsType<Proveedor>(createdResult.Value);
            Assert.Equal(nuevoProveedor.id_proveedor, proveedorDevuelto.id_proveedor);

            var proveedorEnDb = await context.Proveedores.FindAsync(nuevoProveedor.id_proveedor);
            Assert.NotNull(proveedorEnDb);
            Assert.Equal("Katerina", proveedorEnDb.nombre);
        }

        [Fact]
        public async Task PutProveedor_DeberiaActualizarProveedorYRetornarNoContent()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();

            // Desanclar manualmente la entidad que se insertó en GetDbContextConDatosPrueba
            var local = context.Proveedores.Local.FirstOrDefault(c => c.id_proveedor == 1);
            if (local != null)
                context.Entry(local).State = EntityState.Detached;

            var controller = new ProveedorsController(context);
            var proveedorActualizado = new ProveedorDTO { id_proveedor = 1, nombre = "YuanGarcia", telefono = "11335577", direccion = "Por hay" };

            // Act
            var result = await controller.PutProveedor(1, proveedorActualizado);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var proveedorEnDb = await context.Proveedores.FindAsync(1);
            Assert.Equal("YuanGarcia", proveedorEnDb.nombre);
        }

        [Fact]
        public async Task PutProveedor_DeberiaRetornarBadRequestSiIdNoCoincide()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new ProveedorsController(context);
            var proveedorConOtroId = new ProveedorDTO { id_proveedor = 99, nombre = "Yuanes" };

            // Act
            var result = await controller.PutProveedor(1, proveedorConOtroId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteProveedor_DeberiaEliminarProveedorYRetornarNoContent()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new ProveedorsController(context);

            // Act
            var result = await controller.DeleteProveedor(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await context.Proveedores.FindAsync(1));
        }

        [Fact]
        public async Task DeleteProveedor_DeberiaRetornarNotFoundSiNoExiste()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            var controller = new ProveedorsController(context);

            // Act
            var result = await controller.DeleteProveedor(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
