using CERVERICA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CERVERICA.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        // DbSet representa una colección de entidades en una base de datos
        // Proporciona métodos para consultar, agregar, modificar y eliminar entidades en la base de datos
        // Cada DbSet se mapea a una tabla en la base de datos y cada entidad se mapea a una fila en esa tabla
        // Los DbSet se utilizan para interactuar con las entidades en el contexto de la base de datos
        // En este caso, cada DbSet representa una tabla en la base de datos ApplicationDbContext
        // Los DbSet se utilizan para realizar operaciones CRUD (Crear, Leer, Actualizar, Eliminar) en las entidades correspondientes
        // Por ejemplo, el DbSet<Proveedor> representa la tabla "Proveedores" en la base de datos y se utiliza para realizar operaciones en la entidad Proveedor
        // Los DbSet se definen como propiedades en la clase ApplicationDbContext y se configuran en el método OnModelCreating
        // Estos DbSet se utilizan en otras partes del código para interactuar con la base de datos y realizar consultas y modificaciones en las entidades correspondientes
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Insumo> Insumos { get; set; }
        public DbSet<LoteInsumo> LotesInsumos { get; set; }
        public DbSet<Receta> Recetas { get; set; }
        public DbSet<IngredienteReceta> IngredientesReceta { get; set; }
        public DbSet<Produccion> Producciones { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }
        public DbSet<LogLogin> LogsLogin { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<PasosReceta> PasosRecetas { get; set; }
        public DbSet<ProduccionLoteInsumo> ProduccionLoteInsumos { get; set; }
        public DbSet<FavoritosComprador> FavoritosComprador { get; set; }
        public DbSet<ProductoCarrito> ProductosCarrito { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<PuntosFidelidad> PuntosFidelidad { get; set; }
        public DbSet<TransaccionPuntos> TransaccionesPuntos { get; set; }
        public DbSet<ReglaPuntos> ReglasPuntos { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<ClienteMayorista> ClientesMayoristas { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<CategoriaCerveza> CategoriasCerveza { get; set; }
        public DbSet<RecetaCategoriaCerveza> RecetasCategoriasCerveza { get; set; }
        public DbSet<Cupones> Cupones { get; set; }
        public DbSet<ConfiguracionesGenerales> ConfiguracionesGenerales { get; set; }
        public DbSet<ConfiguracionVentasMayoreo> ConfiguracionesVentasMayoreo { get; set; }
        public DbSet<PreferenciasUsuario> PreferenciasUsuarios { get; set; }
        public DbSet<EstadisticaComprador> EstadisticasCompradores { get; set; }
        public DbSet<SolicitudesCambioAgente> SolicitudesCambioAgente { get; set; }
        public DbSet<HistorialPrecios> HistorialPrecios { get; set; }
        public DbSet<PedidoMayoreo> PedidosMayoreo { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<EstadisticaAgenteVenta> EstadisticasAgentesVenta { get; set; }
        public DbSet<SolicitudAsistencia> SolicitudesAsistencia { get; set; }
        public DbSet<SeguimientoSolicitudAsistencia> SeguimientosSolicitudesAsistencia { get; set; }
        public DbSet<CategoriaAsistencia> CategoriasAsistencia { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<SolicitudAsistencia>()
               .HasOne(p => p.CategoriaAsistencia)
               .WithMany(pe => pe.SolicitudesAsistencia)
               .HasForeignKey(p => p.IdCategoriaAsistencia)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SeguimientoSolicitudAsistencia>()
               .HasOne(p => p.SolicitudAsistencia)
               .WithMany(pe => pe.SeguimientosSolicitudAsistencia)
               .HasForeignKey(p => p.IdSolicitudAsistencia)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SolicitudAsistencia>()
               .HasOne(p => p.Cliente)
               .WithMany(pe => pe.SolicitudesAsistencia)
               .HasForeignKey(p => p.IdCliente)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.EstadisticaAgenteVenta)
                .WithOne(e => e.AgenteVenta)
                .HasForeignKey<EstadisticaAgenteVenta>(e => e.IdAgenteVenta)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pago>()
               .HasOne(p => p.Mayorista)
               .WithMany(pe => pe.Pagos)
               .HasForeignKey(p => p.IdMayorista)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pago>()
               .HasOne(p => p.PedidoMayoreo)
               .WithMany(pe => pe.Pagos) 
               .HasForeignKey(p => p.IdPedidoMayoreo)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comentario>()
                .HasOne(c => c.DetalleVenta)
                .WithMany(dv => dv.Comentarios)
                .HasForeignKey(c => c.IdDetalleVenta)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Venta)
                .WithMany(v => v.DetallesVentas)
                .HasForeignKey(d => d.IdVenta)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PedidoMayoreo>()
                .HasOne(p => p.Venta)
                .WithOne(v => v.PedidoMayoreo)
                .HasForeignKey<PedidoMayoreo>(p => p.IdVenta)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PedidoMayoreo>()
                .HasOne(p => p.ClienteMayorista)
                .WithMany(v => v.PedidosMayoreo)
                .HasForeignKey(p => p.IdMayorista)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración para la relación con IdUsuarioSolicitud en Producciones
            modelBuilder.Entity<Produccion>()
                .HasOne(p => p.UsuarioSolicitud)
                .WithMany(u => u.ProduccionesSolicitadas)
                .HasForeignKey(p => p.IdUsuarioSolicitud)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración para otras relaciones de Producciones que tengan claves foráneas con ApplicationUser
            modelBuilder.Entity<Produccion>()
                .HasOne(p => p.UsuarioProduccion)
                .WithMany(u => u.ProduccionesAprobadas)
                .HasForeignKey(p => p.IdUsuarioProduccion)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de la relación para SolicitudesCambioAgenteActual
            modelBuilder.Entity<SolicitudesCambioAgente>()
                .HasOne(s => s.AgenteVentaActual)
                .WithMany(a => a.SolicitudesCambioAgenteActual)
                .HasForeignKey(s => s.IdAgenteVentaActual)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de la relación para SolicitudesCambioAgenteNuevo
            modelBuilder.Entity<SolicitudesCambioAgente>()
                .HasOne(s => s.AgenteVentaNuevo)
                .WithMany(a => a.SolicitudesCambioAgenteNuevo)
                .HasForeignKey(s => s.IdAgenteVentaNuevo)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación uno a muchos entre ApplicationUser (AgenteVenta) y ClienteMayorista
            modelBuilder.Entity<ClienteMayorista>()
                .HasOne(c => c.AgenteVenta)
                .WithMany(a => a.ClientesMayoristas)
                .HasForeignKey(c => c.IdAgenteVenta)
                .OnDelete(DeleteBehavior.Restrict);

            // Definición de tablas intermedias de muchos a muchos
            modelBuilder.Entity<IngredienteReceta>()
                .HasKey(ir => new { ir.IdReceta, ir.IdInsumo });

            modelBuilder.Entity<ProduccionLoteInsumo>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Produccion)
                    .WithMany(p => p.ProduccionLoteInsumos)
                    .HasForeignKey(e => e.IdProduccion)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.LoteInsumo)
                    .WithMany(l => l.ProduccionLoteInsumos)
                    .HasForeignKey(e => e.IdLoteInsumo)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.Cantidad).IsRequired();
            });

            //Restricciones de eliminación
            modelBuilder.Entity<Receta>()
                .HasMany(r => r.Stocks)
                .WithOne(s => s.Receta)
                .HasForeignKey(s => s.IdReceta)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Receta)
                .WithMany(r => r.Stocks)
                .HasForeignKey(s => s.IdReceta)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Produccion>()
                .HasOne(s => s.Receta)
                .WithMany(r => r.Producciones)
                .HasForeignKey(s => s.IdReceta)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IngredienteReceta>()
                .HasOne(s => s.Receta)
                .WithMany(r => r.IngredientesReceta)
                .HasForeignKey(s => s.IdReceta)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LoteInsumo>()
            .HasOne(o => o.Insumo)
            .WithMany(i => i.LotesInsumos)
            .HasForeignKey(o => o.IdInsumo)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IngredienteReceta>()
            .HasOne(o => o.Insumo)
            .WithMany(i => i.IngredientesReceta)
            .HasForeignKey(o => o.IdInsumo)
            .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);

            // Seed de datos
            
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Operador", NormalizedName = "OPERADOR" },
                new IdentityRole { Id = "2", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "3", Name = "Mayorista", NormalizedName = "MAYORISTA" },
                new IdentityRole { Id = "4", Name = "Cliente", NormalizedName = "CLIENTE" },
                new IdentityRole { Id = "5", Name = "Agente", NormalizedName = "AGENTE" },
                new IdentityRole { Id = "6", Name = "Cocinero", NormalizedName = "COCINERO" }
            );

            // Hasher para las contraseñas
            var hasher = new PasswordHasher<ApplicationUser>();

            // Usuarios de cada rol
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = "U1",
                    UserName = "operador precargado",
                    NormalizedUserName = "OPERADOR PRECARGADO",
                    Email = "operador@example.com",
                    NormalizedEmail = "OPERADOR@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Contra1234?"),
                    FullName = "Operador General",
                    Activo = true
                },
                new ApplicationUser
                {
                    Id = "U2",
                    UserName = "admin precargado",
                    NormalizedUserName = "ADMIN PRECARGADO",
                    Email = "admin@example.com",
                    NormalizedEmail = "ADMIN@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Contra1234?"),
                    FullName = "Administrador Principal",
                    Activo = true
                },
                new ApplicationUser
                {
                    Id = "U3",
                    UserName = "mayorista precargado",
                    NormalizedUserName = "MAYORISTA PRECARGADO",
                    Email = "mayorista@example.com",
                    NormalizedEmail = "MAYORISTA@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Contra1234?"),
                    FullName = "Mayorista Distribuidor",
                    Activo = true
                },
                new ApplicationUser
                {
                    Id = "U4",
                    UserName = "cliente precargado",
                    NormalizedUserName = "CLIENTE PRECARGADO",
                    Email = "cliente@example.com",
                    NormalizedEmail = "CLIENTE@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Contra1234?"),
                    FullName = "Cliente Regular",
                    Activo = true
                },
                new ApplicationUser
                {
                    Id = "U5",
                    UserName = "agente precargado",
                    NormalizedUserName = "AGENTE PRECARGADO",
                    Email = "agente@example.com",
                    NormalizedEmail = "AGENTE@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Contra1234?"),
                    FullName = "Agente de Ventas",
                    Activo = true
                },
                new ApplicationUser
                {
                    Id = "U6",
                    UserName = "cocinero precargado",
                    NormalizedUserName = "COCINERO PRECARGADO",
                    Email = "cocinero@example.com",
                    NormalizedEmail = "COCINERO@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Contra1234?"),
                    FullName = "Cocinero Principal",
                    Activo = true
                },
                new ApplicationUser
                {
                    Id = "U7",
                    UserName = "mayorista 2 precargado",
                    NormalizedUserName = "MAYORISTA 2 PRECARGADO",
                    Email = "mayorista2@example.com",
                    NormalizedEmail = "MAYORISTA2@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Contra1234?"),
                    FullName = "Mayorista 2 Distribuidor",
                    Activo = true
                },
                new ApplicationUser
                {
                    Id = "U8",
                    UserName = "cliente 2 precargado",
                    NormalizedUserName = "CLIENTE 2 PRECARGADO",
                    Email = "cliente 2 @example.com",
                    NormalizedEmail = "CLIENTE2@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Contra1234?"),
                    FullName = "Cliente 2 Regular",
                    Activo = true
                },
                new ApplicationUser
                {
                    Id = "U9",
                    UserName = "agente 2 precargado",
                    NormalizedUserName = "AGENTE 2 PRECARGADO",
                    Email = "agente2@example.com",
                    NormalizedEmail = "AGENTE2@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Contra1234?"),
                    FullName = "Agente 2 de Ventas",
                    Activo = true
                },
                new ApplicationUser
                {
                    Id = "U10",
                    UserName = "agente 3 precargado",
                    NormalizedUserName = "AGENTE 3 PRECARGADO",
                    Email = "agente3@example.com",
                    NormalizedEmail = "AGENTE3@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Contra1234?"),
                    FullName = "Agente 3 de Ventas",
                    Activo = true
                }
            );

            // Asignación de roles a los usuarios
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = "U1", RoleId = "1" }, // Operador
                new IdentityUserRole<string> { UserId = "U2", RoleId = "2" }, // Admin
                new IdentityUserRole<string> { UserId = "U3", RoleId = "3" }, // Mayorista
                new IdentityUserRole<string> { UserId = "U4", RoleId = "4" }, // Cliente
                new IdentityUserRole<string> { UserId = "U5", RoleId = "5" }, // Agente
                new IdentityUserRole<string> { UserId = "U6", RoleId = "6" }, // Cocinero
                new IdentityUserRole<string> { UserId = "U7", RoleId = "3" }, // Mayorista
                new IdentityUserRole<string> { UserId = "U8", RoleId = "4" }, // Cliente
                new IdentityUserRole<string> { UserId = "U9", RoleId = "5" }, // Agente
                new IdentityUserRole<string> { UserId = "U10", RoleId = "5" } // Agente
            );
        }
    }
}
