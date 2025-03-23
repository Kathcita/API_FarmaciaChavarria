using API_FarmaciaChavarria.Models;
using Microsoft.EntityFrameworkCore;

namespace API_FarmaciaChavarria.Context
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; } = default!;
        public DbSet<Laboratorio> Laboratorios { get; set; } = default!;

        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<ProductoCaducar> Productos_Caducar { get; set; }

        public DbSet<Compra> Compras { get; set; }
        public DbSet<DetalleCompra> Detalle_Compras { get; set; }
    }
}
