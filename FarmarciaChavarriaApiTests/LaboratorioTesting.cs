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
    public class LaboratorioTesting
    {
        private AppDbContext GetDbContextConDatosPrueba()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            // Datos de prueba
            context.Laboratorios.AddRange(
                new Laboratorio { id_laboratorio = 1, nombre = "labWest" },
                new Laboratorio { id_laboratorio = 2, nombre = "Galo" },
                new Laboratorio { id_laboratorio = 3, nombre = "labColombia" }
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
        public async Task GetLaboratorio_DeberiaRetornarOkConResultadosPaginados()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new LaboratoriosController(context);

            // Act
            var result = await controller.GetLaboratorio(pageNumber: 1, pageSize: 2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pageResult = Assert.IsType<LaboratorioPagedResult>(okResult.Value);
            Assert.Equal(2, pageResult.Laboratorios.Count);
            Assert.Equal(3, pageResult.TotalItems);
            Assert.Equal(2, pageResult.TotalPages);
            Assert.Equal(1, pageResult.CurrentPage);
            Assert.Equal(2, pageResult.PageSize);
        }

        [Fact]
        public async Task GetLaboratorio_DeberiaRetornarListaVaciaCuandoNoHayLaboratorios()
        {
            // Arrange
            var context = GetDbContextSinDatos(); // DB sin datos
            var controller = new LaboratoriosController(context);

            // Act
            var result = await controller.GetLaboratorio(pageNumber: 1, pageSize: 5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pageResult = Assert.IsType<LaboratorioPagedResult>(okResult.Value);

            Assert.Empty(pageResult.Laboratorios);
            Assert.Equal(0, pageResult.TotalItems);
            Assert.Equal(0, pageResult.TotalPages);
            Assert.Equal(1, pageResult.CurrentPage);
            Assert.Equal(5, pageResult.PageSize);
        }

        [Fact]
        public async Task GetLaboratorio_DeberiaRetornarUnLaboratorioFiltradoPorId()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new LaboratoriosController(context);

            // Act
            var result = await controller.GetLaboratorio(1);

            // Assert
            var laboratorioDTO = Assert.IsType<LaboratorioDTO>(result.Value);
            Assert.Equal(1, laboratorioDTO.id_laboratorio);

        }

        [Fact]
        public async Task GetLaboratorio_DeberiaRetornarUnNotFound()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new LaboratoriosController(context);

            // Act
            var result = await controller.GetLaboratorio(10);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);

        }

        [Fact]
        public async Task GetLaboratorio_DeberiaRetornarLaboratoriosFiltradosPorNombre()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new LaboratoriosController(context);

            // Act
            var busqueda = "West";
            var result = await controller.GetLaboratorioByNombre(busqueda, 1, 8);

            //Assert
            var resultObject = Assert.IsType<OkObjectResult>(result.Result);
            var laboratorioDTO = Assert.IsType<LaboratorioPagedResult>(resultObject.Value);
            Assert.Contains(busqueda, laboratorioDTO.Laboratorios.First().nombre);
        }

        [Fact]
        public async Task GetCategoria_NoDeberiaRetornarLaboratoriosFiltradosPorNombre()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            var controller = new LaboratoriosController(context);

            // Act
            var busqueda = "Lobs";
            var result = await controller.GetLaboratorioByNombre(busqueda, 1, 8);

            //Assert
            var resultObject = Assert.IsType<OkObjectResult>(result.Result);
            var laboratorioDTO = Assert.IsType<LaboratorioPagedResult>(resultObject.Value);
            Assert.True(laboratorioDTO.Laboratorios.Count == 0);
        }


        [Fact]
        public async Task PostLaboratorio_DeberiaCrearLaboratorioYRetornarCreatedAtAction()
        {
            // Arrange
            var context = GetDbContextSinDatos(); // DB vacía
            var controller = new LaboratoriosController(context);
            var nuevoLaboratorio = new LaboratorioDTO { id_laboratorio = 10, nombre = "labJuan" };

            // Act
            var result = await controller.PostLaboratorio(nuevoLaboratorio);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var laboratorioDevuelto = Assert.IsType<Laboratorio>(createdResult.Value);
            Assert.Equal(nuevoLaboratorio.id_laboratorio, laboratorioDevuelto.id_laboratorio);

            var laboratorioEnDb = await context.Laboratorios.FindAsync(nuevoLaboratorio.id_laboratorio);
            Assert.NotNull(laboratorioEnDb);
            Assert.Equal("labJuan", laboratorioEnDb.nombre);
        }

        [Fact]
        public async Task PutLaboratorio_DeberiaActualizarLaboratorioYRetornarNoContent()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();

            // Desanclar manualmente la entidad que se insertó en GetDbContextConDatosPrueba
            var local = context.Laboratorios.Local.FirstOrDefault(c => c.id_laboratorio == 1);
            if (local != null)
                context.Entry(local).State = EntityState.Detached;

            var controller = new LaboratoriosController(context);
            var laboratorioActualizada = new LaboratorioDTO { id_laboratorio = 1, nombre = "labJuan" };

            // Act
            var result = await controller.PutLaboratorio(1, laboratorioActualizada);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var laboratorioEnDb = await context.Laboratorios.FindAsync(1);
            Assert.Equal("labJuan", laboratorioEnDb.nombre);
        }

        [Fact]
        public async Task PutLaboratorio_DeberiaRetornarBadRequestSiIdNoCoincide()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new LaboratoriosController(context);
            var laboratorioConOtroId = new LaboratorioDTO { id_laboratorio = 99, nombre = "GemmaLab" };

            // Act
            var result = await controller.PutLaboratorio(1, laboratorioConOtroId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteLaboratorio_DeberiaEliminarLaboratorioYRetornarNoContent()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new LaboratoriosController(context);

            // Act
            var result = await controller.DeleteLaboratorio(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await context.Laboratorios.FindAsync(1));
        }

        [Fact]
        public async Task DeleteLaboratorio_DeberiaRetornarNotFoundSiNoExiste()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            var controller = new LaboratoriosController(context);

            // Act
            var result = await controller.DeleteLaboratorio(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
