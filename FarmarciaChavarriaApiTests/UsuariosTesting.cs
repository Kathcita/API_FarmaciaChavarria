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
    public class UsuariosTesting
    {
        private AppDbContext GetDbContextConDatosPrueba()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            // Datos de prueba
            context.Usuarios.AddRange(
                new Usuario { id_usuario = 1, nombre = "JomarDiaz", pin = 1009, rol = "Administrador" },
                new Usuario { id_usuario = 2, nombre = "KatherineMartinez", pin = 1010, rol = "Administrador" },
                new Usuario { id_usuario = 3, nombre = "HorellAltamirano", pin = 2405, rol = "Administrador" }
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
        public async Task GetUsuarios_DeberiaRetornarUnaListaDeUsuarios()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new UsuariosController(context);

            // Act
            var result = await controller.GetUsuarios();

            // Assert
            var usuarios = Assert.IsType<List<Usuario>>(result.Value);
            Assert.NotEmpty(usuarios);
        }

        [Fact]
        public async Task GetUsuarios_DeberiaRetornarListaVaciaCuandoNoHayUsuarios()
        {
            // Arrange
            var context = GetDbContextSinDatos(); // DB sin datos
            var controller = new UsuariosController(context);

            // Act
            var result = await controller.GetUsuarios();

            // Assert
            var usuarios = Assert.IsType<List<Usuario>>(result.Value);

            Assert.Empty(usuarios);
        }

        [Fact]
        public async Task GetUsuario_DeberiaRetornarUnUsuarioFiltradoPorId()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new UsuariosController(context);

            // Act
            var result = await controller.GetUsuario(1);

            // Assert
            var usuario = Assert.IsType<UsuarioDTO>(result.Value);
            Assert.Equal(1, usuario.id_usuario);

        }

        [Fact]
        public async Task GetUsuario_DeberiaRetornarUnNotFound()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new UsuariosController(context);

            // Act
            var result = await controller.GetUsuario(10);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);

        }

        [Fact]
        public async Task PostUsuario_DeberiaCrearUsuarioYRetornarCreatedAtAction()
        {
            // Arrange
            var context = GetDbContextSinDatos(); // DB vacía
            var controller = new UsuariosController(context);
            var nuevoUsuario = new UsuarioDTO { id_usuario = 10, nombre = "Katerina", rol = "Administrador", pin = 1144 };

            // Act
            var result = await controller.PostUsuario(nuevoUsuario);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var usuarioDevuelto = Assert.IsType<Usuario>(createdResult.Value);
            Assert.Equal(nuevoUsuario.id_usuario, usuarioDevuelto.id_usuario);

            var usuarioEnDb = await context.Usuarios.FindAsync(nuevoUsuario.id_usuario);
            Assert.NotNull(usuarioEnDb);
            Assert.Equal("Katerina", usuarioEnDb.nombre);
        }

        [Fact]
        public async Task PutUsuario_DeberiaActualizarProveedorYRetornarNoContent()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();

            // Desanclar manualmente la entidad que se insertó en GetDbContextConDatosPrueba
            var local = context.Usuarios.Local.FirstOrDefault(c => c.id_usuario == 1);
            if (local != null)
                context.Entry(local).State = EntityState.Detached;

            var controller = new UsuariosController(context);
            var usuarioActualizado = new UsuarioDTO { id_usuario = 1, nombre = "YuanGarcia", pin = 2507, rol = "Administrador" };

            // Act
            var result = await controller.PutUsuario(1, usuarioActualizado);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var proveedorEnDb = await context.Usuarios.FindAsync(1);
            Assert.Equal("YuanGarcia", proveedorEnDb.nombre);
        }

        [Fact]
        public async Task PutUsuario_DeberiaRetornarBadRequestSiIdNoCoincide()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new UsuariosController(context);
            var usuarioConOtroId = new UsuarioDTO { id_usuario = 99, nombre = "Yuanes", pin = 1234, rol = "Empleado" };

            // Act
            var result = await controller.PutUsuario(1, usuarioConOtroId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteUsuario_DeberiaEliminarUsuarioYRetornarNoContent()
        {
            // Arrange
            var context = GetDbContextConDatosPrueba();
            var controller = new UsuariosController(context);

            // Act
            var result = await controller.DeleteUsuario(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await context.Usuarios.FindAsync(1));
        }

        [Fact]
        public async Task DeleteUsuario_DeberiaRetornarNotFoundSiNoExiste()
        {
            // Arrange
            var context = GetDbContextSinDatos();
            var controller = new UsuariosController(context);

            // Act
            var result = await controller.DeleteUsuario(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
