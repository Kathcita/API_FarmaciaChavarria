using API_FarmaciaChavarria.Context;
using API_FarmaciaChavarria.Controllers;
using API_FarmaciaChavarria.Models;
using API_FarmaciaChavarria.Models.PaginationModels;
using API_FarmaciaChavarria.ModelsDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FarmarciaChavarriaApiTests
{
    public class CategoriaTesting
    {
        private AppDbContext GetDbContextConDatosPrueba()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            // Datos de prueba
            context.Categorias.AddRange(
                new Categoria { id_categoria = 1, nombre = "Pastillas" },
                new Categoria { id_categoria = 2, nombre = "Jarabes" },
                new Categoria { id_categoria = 3, nombre = "Vendajes" }
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
        public async Task GetCategoria_DeberiaRetornarOkConResultadosPaginados()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new CategoriasController(context);

            // Act
            var result = await controller.GetCategoria(pageNumber: 1, pageSize: 2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pageResult = Assert.IsType<CategoriaPageResult>(okResult.Value);
            Assert.Equal(2, pageResult.Categorias.Count);
            Assert.Equal(3, pageResult.TotalItems);
            Assert.Equal(2, pageResult.TotalPages);
            Assert.Equal(1, pageResult.CurrentPage);
            Assert.Equal(2, pageResult.PageSize);
        }

        [Fact]
        public async Task GetCategoria_DeberiaRetornarListaVaciaCuandoNoHayCategorias()
        {
            // Arrange
            var context = GetDbContextSinDatos(); // DB sin datos
            var controller = new CategoriasController(context);

            // Act
            var result = await controller.GetCategoria(pageNumber: 1, pageSize: 5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pageResult = Assert.IsType<CategoriaPageResult>(okResult.Value);

            Assert.Empty(pageResult.Categorias);
            Assert.Equal(0, pageResult.TotalItems);
            Assert.Equal(0, pageResult.TotalPages);
            Assert.Equal(1, pageResult.CurrentPage);
            Assert.Equal(5, pageResult.PageSize);
        }

        [Fact]
        public async Task GetCategoria_DeberiaRetornarUnaCategoriaFiltradaPorId()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new CategoriasController(context);

            // Act
            var result = await controller.GetCategoria(1);

            // Assert
            var categoriaDto = Assert.IsType<CategoriaDTO>(result.Value);
            Assert.Equal(1, categoriaDto.id_categoria);

        }

        [Fact]
        public async Task GetCategoria_DeberiaRetornarUnNotFound()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new CategoriasController(context);

            // Act
            var result = await controller.GetCategoria(10);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);

        }

        [Fact]
        public async Task GetCategoria_DeberiaRetornarCategoriasFiltradasPorNombre()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new CategoriasController(context);

            // Act
            var busqueda = "Pas";
            var result = await controller.GetCategoriaByNombre(busqueda,1 , 8);

            //Assert
            var resultObject = Assert.IsType<OkObjectResult>(result.Result);
            var categoriaDto = Assert.IsType<CategoriaPageResult>(resultObject.Value);
            Assert.Contains(busqueda, categoriaDto.Categorias.First().nombre);
        }

        [Fact]
        public async Task GetCategoria_NoDeberiaRetornarCategoriasFiltradasPorNombre()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            var controller = new CategoriasController(context);

            // Act
            var busqueda = "Pos";
            var result = await controller.GetCategoriaByNombre(busqueda, 1, 8);

            //Assert
            var resultObject = Assert.IsType<OkObjectResult>(result.Result);
            var categoriaDto = Assert.IsType<CategoriaPageResult>(resultObject.Value);
            Assert.True(categoriaDto.Categorias.Count == 0);
        }

        [Fact]
        public async Task PostCategoria_DeberiaCrearCategoriaYRetornarCreatedAtAction()
        {
            // Arrange
            var context = GetDbContextSinDatos(); // DB vacía
            var controller = new CategoriasController(context);
            var nuevaCategoria = new CategoriaDTO { id_categoria = 10, nombre = "Vitaminas" };

            // Act
            var result = await controller.PostCategoria(nuevaCategoria);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var categoriaDevuelta = Assert.IsType<CategoriaDTO>(createdResult.Value);
            Assert.Equal(nuevaCategoria.id_categoria, categoriaDevuelta.id_categoria);
        }

        [Fact]
        public async Task PutCategoria_DeberiaActualizarCategoriaYRetornarNoContent()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();

            // Desanclar manualmente la entidad que se insertó en GetDbContextConDatosPrueba
            var local = context.Categorias.Local.FirstOrDefault(c => c.id_categoria == 1);
            if (local != null)
                context.Entry(local).State = EntityState.Detached;

            var controller = new CategoriasController(context);
            var categoriaActualizada = new CategoriaDTO { id_categoria = 1, nombre = "Medicinas" };

            // Act
            var result = await controller.PutCategoria(1, categoriaActualizada);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var categoriaEnDb = await context.Categorias.FindAsync(1);
            Assert.Equal("Medicinas", categoriaEnDb.nombre);
        }


        [Fact]
        public async Task PutCategoria_DeberiaRetornarBadRequestSiIdNoCoincide()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new CategoriasController(context);
            var categoriaConOtroId = new CategoriaDTO { id_categoria = 99, nombre = "Cremas" };

            // Act
            var result = await controller.PutCategoria(1, categoriaConOtroId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteCategoria_DeberiaEliminarCategoriaYRetornarNoContent()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new CategoriasController(context);

            // Act
            var result = await controller.DeleteCategoria(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await context.Categorias.FindAsync(1));
        }

        [Fact]
        public async Task DeleteCategoria_DeberiaRetornarNotFoundSiNoExiste()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            var controller = new CategoriasController(context);

            // Act
            var result = await controller.DeleteCategoria(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }



    }
}