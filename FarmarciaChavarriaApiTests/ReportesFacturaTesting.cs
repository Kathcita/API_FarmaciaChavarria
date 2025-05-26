using API_FarmaciaChavarria.Context;
using API_FarmaciaChavarria.Controllers;
using API_FarmaciaChavarria.Models.PaginationModels;
using API_FarmaciaChavarria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API_FarmaciaChavarria.Models.Reporte_Models;

namespace FarmarciaChavarriaApiTests
{
    public class ReportesFacturaTesting
    {
        private AppDbContext GetDbContextSinDatos()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Para que cada test tenga su propia DB
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetFacturasPorAño_DeberiaRetornarFacturasEnElRangoDeFechas()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            context.Facturas.AddRange(new List<Factura>
    {
        new Factura { id_factura = 1, id_usuario = 1, fecha_venta = new DateTime(2023, 01, 15) },
        new Factura { id_factura = 2, id_usuario = 1, fecha_venta = new DateTime(2023, 05, 10) },
        new Factura { id_factura = 3, id_usuario = 2, fecha_venta = new DateTime(2024, 02, 01) }
    });
            await context.SaveChangesAsync();

            var controller = new FacturasController(context);
            var fechaInicio = new DateTime(2023, 01, 01);
            var fechaFin = new DateTime(2023, 12, 31);

            // Act
            var result = await controller.GetFacturasPorAño(fechaInicio, fechaFin);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<FacturaPagedResult>(okResult.Value);

            Assert.Equal(2, response.TotalItems);
            Assert.All(response.Facturas, f => Assert.True(f.fecha_venta.Year == 2023));
        }


        [Fact]
        public async Task GetFacturasPorAño_DeberiaFiltrarPorUsuarioSiSeIndica()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            context.Facturas.AddRange(new List<Factura>
    {
        new Factura { id_factura = 1, id_usuario = 1, fecha_venta = new DateTime(2023, 01, 15) },
        new Factura { id_factura = 2, id_usuario = 2, fecha_venta = new DateTime(2023, 02, 10) }
    });
            await context.SaveChangesAsync();

            var controller = new FacturasController(context);
            var fechaInicio = new DateTime(2023, 01, 01);
            var fechaFin = new DateTime(2023, 12, 31);

            // Act
            var result = await controller.GetFacturasPorAño(fechaInicio, fechaFin, userId: 2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<FacturaPagedResult>(okResult.Value);

            Assert.Single(response.Facturas);
            Assert.Equal(2, response.Facturas[0].id_factura);
        }

        [Fact]
        public async Task GetFacturasPorAño_DeberiaRetornarListaVaciaSiNoHayFacturas()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            context.Facturas.Add(new Factura
            {
                id_factura = 1,
                id_usuario = 1,
                fecha_venta = new DateTime(2021, 01, 01)
            });
            await context.SaveChangesAsync();

            var controller = new FacturasController(context);
            var fechaInicio = new DateTime(2023, 01, 01);
            var fechaFin = new DateTime(2023, 12, 31);

            // Act
            var result = await controller.GetFacturasPorAño(fechaInicio, fechaFin);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<FacturaPagedResult>(okResult.Value);

            Assert.Empty(response.Facturas);
            Assert.Equal(0, response.TotalItems);
        }

        [Fact]
        public async Task GetFacturasPorAño_DeberiaPaginarCorrectamente()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            for (int i = 1; i <= 10; i++)
            {
                context.Facturas.Add(new Factura
                {
                    id_factura = i,
                    id_usuario = 1,
                    fecha_venta = new DateTime(2023, 01, i)
                });
            }
            await context.SaveChangesAsync();

            var controller = new FacturasController(context);
            var fechaInicio = new DateTime(2023, 01, 01);
            var fechaFin = new DateTime(2023, 12, 31);

            // Act
            var result = await controller.GetFacturasPorAño(fechaInicio, fechaFin, pageNumber: 2, pageSize: 4);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<FacturaPagedResult>(okResult.Value);

            Assert.Equal(4, response.Facturas.Count); // Página 2, tamaño 4 => facturas 5 al 8
            Assert.Equal(10, response.TotalItems);
            Assert.Equal(3, response.TotalPages);
            Assert.Equal(2, response.CurrentPage);
        }

        [Fact]
        public async Task GetVentasPorMesAño_DeberiaAgruparVentasPorMesYAnio()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            context.Facturas.AddRange(new List<Factura>
    {
        new Factura { id_factura = 1, id_usuario = 1, fecha_venta = new DateTime(2023, 01, 10), total = 100 },
        new Factura { id_factura = 2, id_usuario = 1, fecha_venta = new DateTime(2023, 01, 15), total = 150 },
        new Factura { id_factura = 3, id_usuario = 1, fecha_venta = new DateTime(2023, 02, 05), total = 200 }
    });
            await context.SaveChangesAsync();

            var controller = new FacturasController(context);
            var fechaInicio = new DateTime(2023, 01, 01);
            var fechaFin = new DateTime(2023, 12, 31);

            // Act
            var result = await controller.GetVentasPorMesAño(fechaInicio, fechaFin);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<List<RevenueDataItem>>(okResult.Value);

            Assert.Equal(2, response.Count);
            Assert.Equal("ene 2023", response[0].Date.ToLower()); // Nombre abreviado depende de cultura
            Assert.Equal(250, response[0].Revenue); // enero
            Assert.Equal("feb 2023", response[1].Date.ToLower()); // febrero
            Assert.Equal(200, response[1].Revenue);
        }

        [Fact]
        public async Task GetVentasPorMesAño_DeberiaFiltrarPorUsuario()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            context.Facturas.AddRange(new List<Factura>
    {
        new Factura { id_factura = 1, id_usuario = 1, fecha_venta = new DateTime(2023, 03, 01), total = 100 },
        new Factura { id_factura = 2, id_usuario = 2, fecha_venta = new DateTime(2023, 03, 02), total = 200 }
    });
            await context.SaveChangesAsync();

            var controller = new FacturasController(context);
            var fechaInicio = new DateTime(2023, 01, 01);
            var fechaFin = new DateTime(2023, 12, 31);

            // Act
            var result = await controller.GetVentasPorMesAño(fechaInicio, fechaFin, userId: 2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<List<RevenueDataItem>>(okResult.Value);

            Assert.Single(response);
            Assert.Equal(200, response[0].Revenue);
        }

        [Fact]
        public async Task GetVentasPorMesAño_DeberiaRetornarListaVaciaSiNoHayDatos()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            context.Facturas.Add(new Factura
            {
                id_factura = 1,
                id_usuario = 1,
                fecha_venta = new DateTime(2022, 01, 01),
                total = 500
            });
            await context.SaveChangesAsync();

            var controller = new FacturasController(context);
            var fechaInicio = new DateTime(2023, 01, 01);
            var fechaFin = new DateTime(2023, 12, 31);

            // Act
            var result = await controller.GetVentasPorMesAño(fechaInicio, fechaFin);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<List<RevenueDataItem>>(okResult.Value);

            Assert.Empty(response);
        }

        [Fact]
        public async Task GetTopLaboratorios_DeberiaRetornarLaboratoriosOrdenadosPorTotalVentas()
        {
            // Arrange
            var context = GetDbContextSinDatos();

            var lab1 = new Laboratorio { id_laboratorio = 1, nombre = "Lab Uno" };
            var lab2 = new Laboratorio { id_laboratorio = 2, nombre = "Lab Dos" };

            var prod1 = new Producto { id_producto = 1, nombre = "Producto A", id_laboratorio = 1, como_usar = "", efectos_secundarios = "" };
            var prod2 = new Producto { id_producto = 2, nombre = "Producto B", id_laboratorio = 2, como_usar = "", efectos_secundarios = "" };

            var factura = new Factura { id_factura = 1, id_usuario = 1, fecha_venta = DateTime.Now };
            var detalles = new List<DetalleFactura>
    {
        new DetalleFactura { id_factura = 1, id_producto = 1, cantidad = 2, precio_unitario = 50 },  // total 100
        new DetalleFactura { id_factura = 1, id_producto = 2, cantidad = 3, precio_unitario = 30 }   // total 90
    };

            context.Laboratorios.AddRange(lab1, lab2);
            context.Productos.AddRange(prod1, prod2);
            context.Facturas.Add(factura);
            context.Detalle_Facturas.AddRange(detalles);
            await context.SaveChangesAsync();

            var controller = new FacturasController(context);

            // Act
            var result = await controller.GetTopLaboratorios();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var top = Assert.IsType<List<LaboratorioVentasDTO>>(okResult.Value);

            Assert.Equal(2, top.Count);
            Assert.Equal("Lab Uno", top[0].NombreLaboratorio); // 100 > 90
            Assert.Equal(100, top[0].TotalVentas);
            Assert.Equal("Lab Dos", top[1].NombreLaboratorio);
            Assert.Equal(90, top[1].TotalVentas);
        }

        [Fact]
        public async Task GetTopLaboratorios_DeberiaFiltrarPorUsuario()
        {
            // Arrange
            var context = GetDbContextSinDatos();

            var lab = new Laboratorio { id_laboratorio = 1, nombre = "Lab X" };
            var prod = new Producto {id_producto = 1, nombre = "Prod X", id_laboratorio = 1, como_usar = "", efectos_secundarios = "" };

            var factura1 = new Factura { id_factura = 1, id_usuario = 1, fecha_venta = DateTime.Now };
            var factura2 = new Factura { id_factura = 2, id_usuario = 2, fecha_venta = DateTime.Now };

            context.Laboratorios.Add(lab);
            context.Productos.Add(prod);
            context.Facturas.AddRange(factura1, factura2);
            context.Detalle_Facturas.AddRange(
                new DetalleFactura { id_factura = 1, id_producto = 1, cantidad = 1, precio_unitario = 100 },
                new DetalleFactura { id_factura = 2, id_producto = 1, cantidad = 10, precio_unitario = 100 }
            );

            await context.SaveChangesAsync();

            var controller = new FacturasController(context);

            // Act
            var result = await controller.GetTopLaboratorios(userId: 1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var top = Assert.IsType<List<LaboratorioVentasDTO>>(okResult.Value);

            Assert.Single(top);
            Assert.Equal(100, top[0].TotalVentas);
        }

        [Fact]
        public async Task GetTopLaboratorios_DeberiaFiltrarPorFecha()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            var lab = new Laboratorio { id_laboratorio = 1, nombre = "TemporalLab" };
            var prod = new Producto {id_producto = 1, nombre = "Prod Temporal", id_laboratorio = 1, como_usar = "", efectos_secundarios = "" };

            var facturaAntigua = new Factura { id_factura = 1, id_usuario = 1, fecha_venta = new DateTime(2020, 1, 1) };
            var facturaActual = new Factura { id_factura = 2, id_usuario = 1, fecha_venta = DateTime.Now };

            context.Laboratorios.Add(lab);
            context.Productos.Add(prod);
            context.Facturas.AddRange(facturaAntigua, facturaActual);
            context.Detalle_Facturas.AddRange(
                new DetalleFactura { id_factura = 1, id_producto = 1, cantidad = 1, precio_unitario = 500 },
                new DetalleFactura { id_factura = 2, id_producto = 1, cantidad = 1, precio_unitario = 200 }
            );

            await context.SaveChangesAsync();

            var controller = new FacturasController(context);
            var desde = new DateTime(2021, 1, 1);
            var hasta = DateTime.Now.AddDays(1);

            // Act
            var result = await controller.GetTopLaboratorios(desde, hasta);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var top = Assert.IsType<List<LaboratorioVentasDTO>>(okResult.Value);

            Assert.Single(top);
            Assert.Equal(200, top[0].TotalVentas);
        }

        [Fact]
        public async Task GetTopLaboratorios_DeberiaRetornarListaVaciaSiNoHayDatos()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            var controller = new FacturasController(context);
            var fechaInicio = DateTime.Now.AddYears(-1);
            var fechaFin = DateTime.Now;

            // Act
            var result = await controller.GetTopLaboratorios(fechaInicio, fechaFin);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var top = Assert.IsType<List<LaboratorioVentasDTO>>(okResult.Value);

            Assert.Empty(top);
        }

        [Fact]
        public async Task GetTopCategorias_ReturnsTopCategoriasOrderedByVentas()
        {
            // Arrange
            var context = GetDbContextSinDatos();

            // Agregar categorías
            var categoria1 = new Categoria { id_categoria = 1, nombre = "Pastillas" };
            var categoria2 = new Categoria { id_categoria = 2, nombre = "Jarabes" };
            context.Categorias.AddRange(categoria1, categoria2);

            // Agregar productos
            var producto1 = new Producto { id_producto = 1, nombre = "Acetaminofen", id_categoria = 1 };
            var producto2 = new Producto { id_producto = 2, nombre = "Broncodil", id_categoria = 2 };
            context.Productos.AddRange(producto1, producto2);

            // Agregar facturas
            var factura1 = new Factura { id_factura = 1, id_usuario = 1, fecha_venta = new DateTime(2024, 1, 15), total = 30 };
            var factura2 = new Factura { id_factura = 2, id_usuario = 1, fecha_venta = new DateTime(2024, 1, 20), total = 40 };
            context.Facturas.AddRange(factura1, factura2);

            // Agregar detalles
            var detalle1 = new DetalleFactura { id_factura = 1, id_producto = 1, cantidad = 3, precio_unitario = 10 }; // Total: 30
            var detalle2 = new DetalleFactura { id_factura = 2, id_producto = 2, cantidad = 5, precio_unitario = 8 };  // Total: 40
            context.Detalle_Facturas.AddRange(detalle1, detalle2);

            await context.SaveChangesAsync();

            // Act
            var controller = new FacturasController(context);
            var resultado = await controller.GetTopCategorias(
                fechaInicio: new DateTime(2024, 1, 1),
                fechaFin: new DateTime(2024, 12, 31),
                userId: 1
            );

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var lista = Assert.IsAssignableFrom<IEnumerable<CategoriaVentasDTO>>(okResult.Value).ToList();

            Assert.Equal(2, lista.Count);
            Assert.Equal("Jarabes", lista[0].NombreCategoria); // Jarabes (40) debe ser primero
            Assert.Equal(40, lista[0].TotalVentas);
            Assert.Equal("Pastillas", lista[1].NombreCategoria); // Pastillas (30)
            Assert.Equal(30, lista[1].TotalVentas);
        }

        [Fact]
        public async Task GetTopCategorias_DeberiaFiltrarPorUsuario()
        {
            // Arrange
            var context = GetDbContextSinDatos();

            var cat = new Categoria { id_categoria = 1, nombre = "Vitaminas" };
            var prod = new Producto { id_producto = 1, nombre = "Vitamina C", id_categoria = 1 };

            var factura1 = new Factura { id_factura = 1, id_usuario = 1, fecha_venta = DateTime.Now };
            var factura2 = new Factura { id_factura = 2, id_usuario = 2, fecha_venta = DateTime.Now };

            context.Categorias.Add(cat);
            context.Productos.Add(prod);
            context.Facturas.AddRange(factura1, factura2);
            context.Detalle_Facturas.AddRange(
                new DetalleFactura { id_factura = 1, id_producto = 1, cantidad = 1, precio_unitario = 50 },
                new DetalleFactura { id_factura = 2, id_producto = 1, cantidad = 10, precio_unitario = 50 }
            );

            await context.SaveChangesAsync();

            var controller = new FacturasController(context);

            // Act
            var result = await controller.GetTopCategorias(userId: 1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var top = Assert.IsType<List<CategoriaVentasDTO>>(okResult.Value);

            Assert.Single(top);
            Assert.Equal(50, top[0].TotalVentas);
        }

        [Fact]
        public async Task GetTopCategorias_DeberiaFiltrarPorFechas()
        {
            // Arrange
            var context = GetDbContextSinDatos();

            var cat = new Categoria { id_categoria = 1, nombre = "Analgésicos" };
            var prod = new Producto { id_producto = 1, nombre = "Paracetamol", id_categoria = 1 };

            var facturaAntigua = new Factura { id_factura = 1, id_usuario = 1, fecha_venta = new DateTime(2020, 1, 1) };
            var facturaReciente = new Factura { id_factura = 2, id_usuario = 1, fecha_venta = DateTime.Now };

            context.Categorias.Add(cat);
            context.Productos.Add(prod);
            context.Facturas.AddRange(facturaAntigua, facturaReciente);
            context.Detalle_Facturas.AddRange(
                new DetalleFactura { id_factura = 1, id_producto = 1, cantidad = 1, precio_unitario = 100 },
                new DetalleFactura { id_factura = 2, id_producto = 1, cantidad = 2, precio_unitario = 50 }
            );

            await context.SaveChangesAsync();

            var controller = new FacturasController(context);
            var desde = new DateTime(2021, 1, 1);
            var hasta = DateTime.Now.AddDays(1);

            // Act
            var result = await controller.GetTopCategorias(desde, hasta);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var top = Assert.IsType<List<CategoriaVentasDTO>>(okResult.Value);

            Assert.Single(top);
            Assert.Equal(100, top[0].TotalVentas); // Solo la factura reciente entra
        }

        [Fact]
        public async Task GetTopCategorias_DeberiaRetornarListaVaciaSiNoCoincide()
        {
            // Arrange
            var context = GetDbContextSinDatos();

            // Cargar datos fuera de rango de fecha
            var cat = new Categoria { id_categoria = 1, nombre = "Antibióticos" };
            var prod = new Producto { id_producto = 1, nombre = "Amoxicilina", id_categoria = 1 };
            var factura = new Factura { id_factura = 1, id_usuario = 1, fecha_venta = new DateTime(2000, 1, 1) };

            context.Categorias.Add(cat);
            context.Productos.Add(prod);
            context.Facturas.Add(factura);
            context.Detalle_Facturas.Add(
                new DetalleFactura { id_factura = 1, id_producto = 1, cantidad = 1, precio_unitario = 100 }
            );

            await context.SaveChangesAsync();

            var controller = new FacturasController(context);
            var desde = new DateTime(2022, 1, 1);
            var hasta = DateTime.Now;

            // Act
            var result = await controller.GetTopCategorias(desde, hasta, userId: 99);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var top = Assert.IsType<List<CategoriaVentasDTO>>(okResult.Value);

            Assert.Empty(top);
        }

        [Fact]
        public async Task GetTopProductos_DeberiaFiltrarPorUsuario()
        {
            // Arrange
            var context = GetDbContextSinDatos();

            var prod = new Producto { id_producto = 1, nombre = "Ibuprofeno" };
            var factura1 = new Factura { id_factura = 1, id_usuario = 1, fecha_venta = DateTime.Now };
            var factura2 = new Factura { id_factura = 2, id_usuario = 2, fecha_venta = DateTime.Now };

            context.Productos.Add(prod);
            context.Facturas.AddRange(factura1, factura2);
            context.Detalle_Facturas.AddRange(
                new DetalleFactura { id_factura = 1, id_producto = 1, cantidad = 1, precio_unitario = 10 },
                new DetalleFactura { id_factura = 2, id_producto = 1, cantidad = 10, precio_unitario = 10 }
            );

            await context.SaveChangesAsync();
            var controller = new FacturasController(context);

            // Act
            var result = await controller.GetTopProductos(userId: 1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var top = Assert.IsType<List<ProductoVentasDTO>>(okResult.Value);

            Assert.Single(top);
            Assert.Equal(10, top[0].TotalVentas);
            Assert.NotEmpty(top);
        }

        [Fact]
        public async Task GetTopProductos_DeberiaFiltrarPorFechas()
        {
            // Arrange
            var context = GetDbContextSinDatos();

            var prod = new Producto { id_producto = 1, nombre = "Aspirina" };
            var facturaAntigua = new Factura { id_factura = 1, id_usuario = 1, fecha_venta = new DateTime(2020, 1, 1) };
            var facturaReciente = new Factura { id_factura = 2, id_usuario = 1, fecha_venta = DateTime.Now };

            context.Productos.Add(prod);
            context.Facturas.AddRange(facturaAntigua, facturaReciente);
            context.Detalle_Facturas.AddRange(
                new DetalleFactura { id_factura = 1, id_producto = 1, cantidad = 1, precio_unitario = 100 },
                new DetalleFactura { id_factura = 2, id_producto = 1, cantidad = 2, precio_unitario = 50 }
            );

            await context.SaveChangesAsync();
            var controller = new FacturasController(context);
            var desde = new DateTime(2022, 1, 1);
            var hasta = DateTime.Now.AddDays(1);

            // Act
            var result = await controller.GetTopProductos(desde, hasta);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var top = Assert.IsType<List<ProductoVentasDTO>>(okResult.Value);

            Assert.Single(top);
            Assert.Equal(100, top[0].TotalVentas); // Solo la venta reciente entra
            Assert.NotEmpty(top);
        }

        [Fact]
        public async Task GetTopProductos_DeberiaDevolverResultadosSinFiltros()
        {
            // Arrange
            var context = GetDbContextSinDatos();

            var prod = new Producto { id_producto = 1, nombre = "Paracetamol" };
            var factura = new Factura { id_factura = 1, id_usuario = 1, fecha_venta = DateTime.Now };

            context.Productos.Add(prod);
            context.Facturas.Add(factura);
            context.Detalle_Facturas.Add(new DetalleFactura
            {
                id_factura = 1,
                id_producto = 1,
                cantidad = 2,
                precio_unitario = 25
            });

            await context.SaveChangesAsync();
            var controller = new FacturasController(context);

            // Act
            var result = await controller.GetTopProductos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var top = Assert.IsType<List<ProductoVentasDTO>>(okResult.Value);

            Assert.Single(top);
            Assert.Equal("Paracetamol", top[0].NombreProducto);
            Assert.Equal(50, top[0].TotalVentas);
        }


        [Fact]
        public async Task GetTopProductos_DeberiaRetornarListaVaciaSiNoCoincide()
        {
            // Arrange
            var context = GetDbContextSinDatos();

            var prod = new Producto { id_producto = 1, nombre = "Clonazepam" };
            var factura = new Factura { id_factura = 1, id_usuario = 1, fecha_venta = new DateTime(2000, 1, 1) };

            context.Productos.Add(prod);
            context.Facturas.Add(factura);
            context.Detalle_Facturas.Add(new DetalleFactura
            {
                id_factura = 1,
                id_producto = 1,
                cantidad = 1,
                precio_unitario = 50
            });

            await context.SaveChangesAsync();
            var controller = new FacturasController(context);

            var desde = new DateTime(2022, 1, 1);
            var hasta = DateTime.Now;
            var userId = 99;

            // Act
            var result = await controller.GetTopProductos(desde, hasta, userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var top = Assert.IsType<List<ProductoVentasDTO>>(okResult.Value);

            Assert.Empty(top);
        }

    }
}
