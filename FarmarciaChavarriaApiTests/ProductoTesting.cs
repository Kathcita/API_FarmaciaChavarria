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
    public class ProductoTesting
    {
        private AppDbContext GetDbContextConDatosPrueba()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            // Datos de prueba
            context.Productos.AddRange(
                new Producto { id_producto = 1, nombre = "Acetaminofen", precio = 3, stock = 10, stock_minimo = 5, id_categoria = 1, id_laboratorio = 1, fecha_vencimiento = new DateOnly(2026, 12, 31), como_usar = "", efectos_secundarios = "" },
                new Producto { id_producto = 2, nombre = "Ibuprofeno", precio = 3, stock = 10, stock_minimo = 5, id_categoria = 1, id_laboratorio = 1, fecha_vencimiento = new DateOnly(2026, 12, 31), como_usar = "", efectos_secundarios = "" },
                new Producto {id_producto = 3, nombre = "Loratadina", precio = 3, stock = 10, stock_minimo = 5, id_categoria = 1, id_laboratorio = 1, fecha_vencimiento = new DateOnly(2026, 12, 31), como_usar = "", efectos_secundarios = "" }
            );

            context.Categorias.Add(new Categoria { id_categoria = 1, nombre = "Analgésicos" });
            context.Laboratorios.Add(new Laboratorio { id_laboratorio = 1, nombre = "LabFarm" });

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
        public async Task GetProductos_DeberiaRetornarOkConResultadosPaginados()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new ProductosController(context);

            // Act
            var result = await controller.GetProductos(pageNumber: 1, pageSize: 2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pageResult = Assert.IsType<ProductoPagedResult>(okResult.Value);
            Assert.Equal(2, pageResult.Productos.Count);
            Assert.Equal(3, pageResult.TotalItems);
            Assert.Equal(2, pageResult.TotalPages);
            Assert.Equal(1, pageResult.CurrentPage);
            Assert.Equal(2, pageResult.PageSize);
        }

        [Fact]
        public async Task GetProductos_DeberiaRetornarListaVaciaCuandoNoHayProductos()
        {
            // Arrange
            var context = GetDbContextSinDatos(); // DB sin datos
            var controller = new ProductosController(context);

            // Act
            var result = await controller.GetProductos(pageNumber: 1, pageSize: 5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pageResult = Assert.IsType<ProductoPagedResult>(okResult.Value);

            Assert.Empty(pageResult.Productos);
            Assert.Equal(0, pageResult.TotalItems);
            Assert.Equal(0, pageResult.TotalPages);
            Assert.Equal(1, pageResult.CurrentPage);
            Assert.Equal(5, pageResult.PageSize);
        }

        [Fact]
        public async Task GetProducto_DeberiaRetornarUnProductoFiltradoPorId()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new ProductosController(context);

            // Act
            var result = await controller.GetProducto(1);

            // Assert
            var producto = Assert.IsType<ProductoDetailedDTO>(result.Value);
            Assert.Equal(1, producto.IdProducto);

        }

        [Fact]
        public async Task GetProducto_DeberiaRetornarUnNotFound()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new ProductosController(context);

            // Act
            var result = await controller.GetProducto(10);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);

        }

        [Fact]
        public async Task GetProductoByName_DeberiaRetornarProductosFiltradosPorNombre()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new ProductosController(context);

            // Act
            var busqueda = "Aceta";
            var result = await controller.GetProductoByName(busqueda, 1, 8);

            //Assert
            var resultObject = Assert.IsType<OkObjectResult>(result.Result);
            var producto = Assert.IsType<ProductoPagedResult>(resultObject.Value);
            Assert.Contains(busqueda, producto.Productos.First().Nombre);
        }

        [Fact]
        public async Task GetProductoByName_NoDeberiaRetornarProductosFiltradasPorNombre()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            var controller = new ProductosController(context);

            // Act
            var busqueda = "Lobs";
            var result = await controller.GetProductoByName(busqueda, 1, 8);

            //Assert
            var resultObject = Assert.IsType<OkObjectResult>(result.Result);
            var producto = Assert.IsType<ProductoPagedResult>(resultObject.Value);
            Assert.True(producto.Productos.Count == 0);
        }

        [Fact]
        public async Task PostProducto_DeberiaCrearProductoYRetornarCreatedAtAction()
        {
            // Arrange
            var context = GetDbContextSinDatos(); // DB vacía
            var controller = new ProductosController(context);
            var nuevoProducto = new ProductoDTO { id_producto = 1, nombre = "Acetaminofen", precio = 3, stock = 10, stock_minimo = 5, id_categoria = 1, id_laboratorio = 1, fecha_vencimiento = new DateOnly(2026, 12, 31), como_usar = "", efectos_secundarios = "" };

            // Act
            var result = await controller.PostProducto(nuevoProducto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var productoDevuelto = Assert.IsType<Producto>(createdResult.Value);
            Assert.Equal(nuevoProducto.id_producto, productoDevuelto.id_producto);

            var productoEnDb = await context.Productos.FindAsync(nuevoProducto.id_producto);
            Assert.NotNull(productoEnDb);
            Assert.Equal("Acetaminofen", productoEnDb.nombre);
        }

        [Fact]
        public async Task PutProducto_DeberiaActualizarProductoYRetornarNoContent()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();

            // Desanclar manualmente la entidad que se insertó en GetDbContextConDatosPrueba
            var local = context.Productos.Local.FirstOrDefault(c => c.id_producto == 1);
            if (local != null)
                context.Entry(local).State = EntityState.Detached;

            var controller = new ProductosController(context);
            var productoActualizado = new ProductoDTO { id_producto = 1, nombre = "Acetaminofen", precio = 3, stock = 10, stock_minimo = 5, id_categoria = 1, id_laboratorio = 1, fecha_vencimiento = new DateOnly(2026, 12, 31), como_usar = "", efectos_secundarios = "" };

            // Act
            var result = await controller.PutProducto(1, productoActualizado);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var productoEnDb = await context.Productos.FindAsync(1);
            Assert.Equal("Acetaminofen", productoEnDb.nombre);
        }

        [Fact]
        public async Task PutProducto_DeberiaRetornarBadRequestSiIdNoCoincide()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new ProductosController(context);
            var productoConOtroId = new ProductoDTO { id_producto = 99, nombre = "Acetaminofen", precio = 3, stock = 10, stock_minimo = 5, id_categoria = 1, id_laboratorio = 1, fecha_vencimiento = new DateOnly(2026, 12, 31), como_usar = "", efectos_secundarios = "" };

            // Act
            var result = await controller.PutProducto(1, productoConOtroId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteProducto_DeberiaEliminarProductoYRetornarNoContent()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new ProductosController(context);

            // Act
            var result = await controller.DeleteProducto(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await context.Productos.FindAsync(1));
        }

        [Fact]
        public async Task DeleteProducto_DeberiaRetornarNotFoundSiNoExiste()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            var controller = new ProductosController(context);

            // Act
            var result = await controller.DeleteProducto(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
