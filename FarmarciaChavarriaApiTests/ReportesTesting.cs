using API_FarmaciaChavarria.Context;
using API_FarmaciaChavarria.Controllers;
using API_FarmaciaChavarria.Models;
using API_FarmaciaChavarria.Models.PaginationModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmarciaChavarriaApiTests
{
    public class ReportesTesting
    {
        private AppDbContext GetDbContextConDatosPrueba()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            // Datos de prueba
            context.Productos.AddRange(
                new Producto { id_producto = 1, nombre = "Acetaminofen", precio = 3, stock = 2, stock_minimo = 5, id_categoria = 1, id_laboratorio = 1, fecha_vencimiento = new DateOnly(2026, 12, 31), como_usar = "", efectos_secundarios = "" },
                new Producto { id_producto = 2, nombre = "Ibuprofeno", precio = 3, stock = 10, stock_minimo = 5, id_categoria = 1, id_laboratorio = 1, fecha_vencimiento = new DateOnly(2026, 12, 31), como_usar = "", efectos_secundarios = "" },
                new Producto { id_producto = 3, nombre = "Loratadina", precio = 3, stock = 10, stock_minimo = 5, id_categoria = 1, id_laboratorio = 1, fecha_vencimiento = new DateOnly(2026, 12, 31), como_usar = "", efectos_secundarios = "" },
                new Producto
                {   id_producto = 4, nombre = "Paracetamol", precio = 2, stock = 20, stock_minimo = 5, id_categoria = 1, id_laboratorio = 1,
                    fecha_vencimiento = DateOnly.FromDateTime(DateTime.Now.AddMonths(2)), // dentro de 3 meses
                    como_usar = "",
                    efectos_secundarios = ""
                });

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
        public async Task GetProductoByNameAndByCategory_DeberiaRetornarResultadosFiltrados()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new ProductosController(context);

            // Act
            var result = await controller.GetProductoByNameAndByCategory(id: 1, nombre: "aceta", pageNumber: 1, pageSize: 5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pageResult = Assert.IsType<ProductoPagedResult>(okResult.Value);

            Assert.Equal("Acetaminofen", pageResult.Productos[0].Nombre);
            Assert.Equal(2, pageResult.TotalItems);
            Assert.Equal(1, pageResult.TotalPages);
        }

        [Fact]
        public async Task GetProductoByNameAndByCategory_DeberiaRetornarListaVaciaSiNoHayCoincidencias()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new ProductosController(context);

            // Act
            var result = await controller.GetProductoByNameAndByCategory(id: 1, nombre: "xyz", pageNumber: 1, pageSize: 5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pageResult = Assert.IsType<ProductoPagedResult>(okResult.Value);

            Assert.Empty(pageResult.Productos);
            Assert.Equal(0, pageResult.TotalItems);
            Assert.Equal(0, pageResult.TotalPages);
        }

        [Fact]
        public async Task GetProductosPorCaducar_DeberiaRetornarProductosConFechaVencimientoProxima()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();

            context.SaveChanges();

            var controller = new ProductosController(context);

            // Act
            var result = await controller.GetProductosPorCadudar(pageNumber: 1, pageSize: 5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pageResult = Assert.IsType<ProductoPagedResult>(okResult.Value);

            Assert.Contains(pageResult.Productos, p => p.Nombre == "Paracetamol");
            Assert.True(pageResult.TotalItems >= 1);
        }

        [Fact]
        public async Task GetProductosPorCaducar_DeberiaRetornarListaVaciaSiNoHayProductos()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            var controller = new ProductosController(context);

            // Act
            var result = await controller.GetProductosPorCadudar(pageNumber: 1, pageSize: 5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pageResult = Assert.IsType<ProductoPagedResult>(okResult.Value);

            Assert.Empty(pageResult.Productos);
            Assert.Equal(0, pageResult.TotalItems);
        }

        [Fact]
        public async Task GetProductosStockEscaso_DeberiaRetornarOkConProductos()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba(); // Este incluye productos con stock escaso
            var controller = new ProductosController(context);

            // Act
            var result = await controller.GetProductosStockEscaso(pageNumber: 1, pageSize: 2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pageResult = Assert.IsType<ProductoPagedResult>(okResult.Value);

            Assert.NotEmpty(pageResult.Productos);
        }

        [Fact]
        public async Task GetProductosStockEscaso_DeberiaRetornarListaVaciaSiNoHayProductosConStockEscaso()
        {
            // Arrange
            var context = GetDbContextSinDatos();

            var controller = new ProductosController(context);

            // Act
            var result = await controller.GetProductosStockEscaso();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pageResult = Assert.IsType<ProductoPagedResult>(okResult.Value);
            Assert.Empty(pageResult.Productos);
            Assert.Equal(0, pageResult.TotalItems);
            Assert.Equal(0, pageResult.TotalPages);
        }


        [Fact]
        public async Task GetProductoByCategory_DeberiaRetornarProductosDeLaCategoria()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new ProductosController(context);

            int categoriaId = 1;

            // Act
            var result = await controller.GetProductoByCategory(categoriaId, pageNumber: 1, pageSize: 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pagedResult = Assert.IsType<ProductoPagedResult>(okResult.Value);

            Assert.NotEmpty(pagedResult.Productos);
            Assert.All(pagedResult.Productos, p => Assert.Equal(categoriaId, p.id_categoria));
            Assert.Equal(pagedResult.TotalItems, pagedResult.Productos.Count);
        }

        [Fact]
        public async Task GetProductoByCategory_DeberiaRetornarListaVaciaSiNoHayProductos()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new ProductosController(context);

            int categoriaIdSinProductos = 999; // ID que no existe en ningún producto

            // Act
            var result = await controller.GetProductoByCategory(categoriaIdSinProductos);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pagedResult = Assert.IsType<ProductoPagedResult>(okResult.Value);

            Assert.Empty(pagedResult.Productos);
            Assert.Equal(0, pagedResult.TotalItems);
            Assert.Equal(0, pagedResult.TotalPages);
        }

        [Fact]
        public async Task GetProductoByCategory_DeberiaRetornarPaginadoCorrectamente()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            int categoriaId = 1;

            // Agregar 10 productos a la misma categoría
            context.Categorias.Add(new Categoria { id_categoria = categoriaId, nombre = "Medicamentos" });
            context.Laboratorios.Add(new Laboratorio { id_laboratorio = 1, nombre = "LabTest" });

            for (int i = 1; i <= 10; i++)
            {
                context.Productos.Add(new Producto
                {
                    id_producto = i,
                    nombre = $"Producto {i}",
                    id_categoria = categoriaId,
                    id_laboratorio = 1,
                    precio = 10,
                    stock = 5,
                    stock_minimo = 2,
                    fecha_vencimiento = new DateOnly(2026, 12, 31),
                    efectos_secundarios = "",
                    como_usar = ""
                });
            }

            context.SaveChanges();

            var controller = new ProductosController(context);

            // Act
            var result = await controller.GetProductoByCategory(categoriaId, pageNumber: 2, pageSize: 4);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pagedResult = Assert.IsType<ProductoPagedResult>(okResult.Value);

            Assert.Equal(4, pagedResult.Productos.Count); // Página 2, tamaño 4 => productos 5 al 8
            Assert.Equal(10, pagedResult.TotalItems);
            Assert.Equal(3, pagedResult.TotalPages); // 10 / 4 = 2.5 => 3 páginas
            Assert.Equal(2, pagedResult.CurrentPage);
        }

    }
}
