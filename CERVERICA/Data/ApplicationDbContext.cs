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

        //Tablas añadidas para el CRM
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
        public DbSet<SolicitudMayorista> SolicitudesMayorista { get; set; }
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

            modelBuilder.Entity<Proveedor>().HasData(
                new Proveedor
                {
                    Id = 1,
                    Empresa = "Bonafont",
                    Telefono = "1234567890",
                    Direccion = "Bonafont Avenue 728",
                    Email = "bonafont@bonafont.com",
                    NombreContacto = "Bonafont",
                    Activo = true
                },
                new Proveedor
                {
                    Id = 2,
                    Empresa = "Maseca",
                    Telefono = "0987654321",
                    Direccion = "Maseca Boulevard 283",
                    Email = "maseca@maseca.com",
                    NombreContacto = "Maseca",
                    Activo = true
                },
                new Proveedor
                {
                    Id = 3,
                    Empresa = "LaCosteña",
                    Telefono = "7364093846",
                    Direccion = "LaCosteña street 632",
                    Email = "lacostena@lacostena.com",
                    NombreContacto = "LaCosteña",
                    Activo = true
                }
            );

            modelBuilder.Entity<CategoriaAsistencia>().HasData(
               new CategoriaAsistencia { Id = 1, Nombre = "Consulta", Estatus=true},
               new CategoriaAsistencia { Id = 2, Nombre = "Reclamación", Estatus = true },
               new CategoriaAsistencia { Id = 3, Nombre = "Soporte", Estatus = true },
               new CategoriaAsistencia { Id = 4, Nombre = "Revisión", Estatus = true }
           );

            modelBuilder.Entity<CategoriaCerveza>().HasData(
                    new CategoriaCerveza { Id = 1, Nombre = "Witbier", Estatus = true },
                    new CategoriaCerveza { Id = 2, Nombre = "Ale", Estatus = true },
                    new CategoriaCerveza { Id = 3, Nombre = "Stout", Estatus = true },
                    new CategoriaCerveza { Id = 4, Nombre = "Pilsner", Estatus = true },
                    new CategoriaCerveza { Id = 5, Nombre = "Lager", Estatus = true }
                );

            modelBuilder.Entity<Insumo>().HasData(
                new Insumo { Id = 1, UnidadMedida = "L", Activo = true, Nombre = "Agua", CantidadMinima = 100, CantidadMaxima = 10000, CostoUnitario = (float)0.15, Descripcion = "Ingrediente base que constituye la mayor parte de la cerveza y afecta su calidad y sabor." },
                new Insumo { Id = 2, UnidadMedida = "PZ", Activo = true, Nombre = "Botellas", CantidadMinima = 281, CantidadMaxima = 10000, CostoUnitario = (float)6, Descripcion = "Envases utilizados para almacenar y distribuir la cerveza terminada. Medida unica de 355 ml." },
                new Insumo { Id = 3, UnidadMedida = "KG", Activo = true, Nombre = "Levadura", CantidadMinima = 5, CantidadMaxima = 50, CostoUnitario = (float)500, Descripcion = "Microorganismo esencial para la fermentación, que convierte los azúcares en alcohol y dióxido de carbono." },
                new Insumo { Id = 4, UnidadMedida = "KG", Activo = true, Nombre = "Lúpulo", CantidadMinima = 1, CantidadMaxima = 50, CostoUnitario = (float)800, Descripcion = "Flor utilizada para aportar amargor, aroma y propiedades conservantes a la cerveza." },
                new Insumo { Id = 5, UnidadMedida = "KG", Activo = true, Nombre = "Trigo", CantidadMinima = 10, CantidadMaxima = 100, CostoUnitario = (float)20, Descripcion = "Cereal usado como complemento de la malta para dar cuerpo y un toque especial al sabor." },
                new Insumo { Id = 6, UnidadMedida = "KG", Activo = true, Nombre = "Maiz", CantidadMinima = 0, CantidadMaxima = 50, CostoUnitario = (float)0, Descripcion = "Adjunto utilizado para aligerar el cuerpo de la cerveza y aportar un perfil más suave." },
                new Insumo { Id = 7, UnidadMedida = "KG", Activo = true, Nombre = "Avena", CantidadMinima = 5, CantidadMaxima = 50, CostoUnitario = (float)22, Descripcion = "Ingrediente que aporta una textura cremosa y mejora la sensación en boca de la cerveza." },
                new Insumo { Id = 8, UnidadMedida = "KG", Activo = true, Nombre = "Malta", CantidadMinima = 100, CantidadMaxima = 1000, CostoUnitario = (float)25, Descripcion = "Grano germinado que proporciona azúcares fermentables, color y sabor a la cerveza." },
                new Insumo { Id = 9, UnidadMedida = "KG", Activo = true, Nombre = "Sal", CantidadMinima = (float)0.05, CantidadMaxima = 5, CostoUnitario = (float)10, Descripcion = "Ingrediente utilizado en ciertos estilos de cerveza para realzar sabores y crear perfiles únicos." },
                new Insumo { Id = 10, UnidadMedida = "KG", Activo = true, Nombre = "Azúcar", CantidadMinima = 5, CantidadMaxima = 50, CostoUnitario = (float)18, Descripcion = "Ingrediente que puede usarse para incrementar el contenido alcohólico o para la carbonatación." }
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
                    Email = "operador@gmail.com",
                    NormalizedEmail = "OPERADOR@GMAIL.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Contra123?"),
                    FullName = "Operador General",
                    Activo = true,
                    PhoneNumber = "1234567890",
                },
                new ApplicationUser
                {
                    Id = "U2",
                    UserName = "admin precargado",
                    NormalizedUserName = "ADMIN PRECARGADO",
                    Email = "admin@gmail.com",
                    NormalizedEmail = "ADMIN@GMAIL.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Contra123?"),
                    FullName = "Administrador Principal",
                    Activo = true,
                    PhoneNumber = "1234567890",
                },
                new ApplicationUser
                {
                    Id = "U3",
                    UserName = "mayorista precargado",
                    NormalizedUserName = "MAYORISTA PRECARGADO",
                    Email = "mayorista@gmail.com",
                    NormalizedEmail = "MAYORISTA@GMAIL.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Contra123?"),
                    PhoneNumber = "1234567890",
                    FullName = "Mayorista Distribuidor",
                    Activo = true
                },
                new ApplicationUser
                {
                    Id = "U4",
                    UserName = "cliente precargado",
                    NormalizedUserName = "CLIENTE PRECARGADO",
                    Email = "cliente@gmail.com",
                    NormalizedEmail = "CLIENTE@GMAIL.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Contra123?"),
                    FullName = "Cliente Regular",
                    PhoneNumber = "1234567890",
                    Activo = true
                },
                new ApplicationUser
                {
                    Id = "U5",
                    UserName = "agente precargado",
                    NormalizedUserName = "AGENTE PRECARGADO",
                    PhoneNumber = "1234567890",
                    Email = "agente@gmail.com",
                    NormalizedEmail = "AGENTE@GMAIL.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Contra123?"),
                    FullName = "Agente de Ventas",
                    Activo = true
                },
                new ApplicationUser
                {
                    Id = "U6",
                    UserName = "cocinero precargado",
                    NormalizedUserName = "COCINERO PRECARGADO",
                    Email = "cocinero@gmail.com",
                    NormalizedEmail = "COCINERO@GMAIL.COM",
                    EmailConfirmed = true,
                    PhoneNumber = "1234567890",
                    PasswordHash = hasher.HashPassword(null, "Contra123?"),
                    FullName = "Cocinero Principal",
                    Activo = true
                },
                new ApplicationUser
                {
                    Id = "U7",
                    UserName = "mayorista 2 precargado",
                    NormalizedUserName = "MAYORISTA 2 PRECARGADO",
                    Email = "mayorista2@gmail.com",
                    NormalizedEmail = "MAYORISTA2@GMAIL.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Contra123?"),
                    PhoneNumber = "1234567890",
                    FullName = "Mayorista 2 Distribuidor",
                    Activo = true
                },
                new ApplicationUser
                {
                    Id = "U8",
                    UserName = "cliente 2 precargado",
                    NormalizedUserName = "CLIENTE 2 PRECARGADO",
                    Email = "cliente2@gmail.com",
                    NormalizedEmail = "CLIENTE2@GMAIL.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Contra123?"),
                    FullName = "Cliente 2 Regular",
                    Activo = true,
                    PhoneNumber = "1234567890",
                },
                new ApplicationUser
                {
                    Id = "U9",
                    UserName = "agente 2 precargado",
                    NormalizedUserName = "AGENTE 2 PRECARGADO",
                    Email = "agente2@gmail.com",
                    NormalizedEmail = "AGENTE2@GMAIL.COM",
                    EmailConfirmed = true,
                    PhoneNumber = "1234567890",
                    PasswordHash = hasher.HashPassword(null, "Contra123?"),
                    FullName = "Agente 2 de Ventas",
                    Activo = true
                },
                new ApplicationUser
                {
                    Id = "U10",
                    UserName = "agente 3 precargado",
                    NormalizedUserName = "AGENTE 3 PRECARGADO",
                    Email = "agente3@gmail.com",
                    NormalizedEmail = "AGENTE3@GMAIL.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Contra123?"),
                    PhoneNumber = "1234567890",
                    FullName = "Agente 3 de Ventas",
                    Activo = true
                }
            );

            modelBuilder.Entity<ClienteMayorista>().HasData(
                new ClienteMayorista
                {
                    Id = 1,
                    IdUsuario = "U3",
                    IdAgenteVenta = "U5",
                    NombreEmpresa = "Empresa 1",
                    RFCEmpresa = "RABC030212GH2",
                    DireccionEmpresa = "Calle 123, Colonia Centro, Ciudad de México",
                    EmailEmpresa = "empresa1@empresa1.com",
                    TelefonoEmpresa = "1234567890",
                    NombreContacto = "Cliente mayorista 1",
                    EmailContacto = "mayorista@gmail.com",
                    TelefonoContacto = "0987654321",
                    CargoContacto = "Gerente de compras"
                },
                new ClienteMayorista
                {
                    Id = 2,
                    IdUsuario = "U7",
                    IdAgenteVenta = "U9",
                    NombreEmpresa = "Empresa 2",
                    RFCEmpresa = "UFYT030212GH3",
                    DireccionEmpresa = "Calle 321, Colonia Norte, León",
                    EmailEmpresa = "empresa2@empresa2.com",
                    TelefonoEmpresa = "0987654321",
                    NombreContacto = "Cliente mayorista 2",
                    EmailContacto = "mayorista2@gmail.com",
                    TelefonoContacto = "0987654321",
                    CargoContacto = "Gerente de compras"
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

            modelBuilder.Entity<Receta>().HasData(
                new Receta
                {
                    Id = 1,
                    Nombre = "Lager",
                    Descripcion = "Una cerveza ligera, de sabor suave y bajo amargor.",
                    Especificaciones = "Alcohol (ABV): ~4.5% | Amargor (IBU): 15-20 | Color (SRM): 2-4 | Carbonatación: Alta (~2.5-3.0 volúmenes de CO₂). | Fermentación: 3-4 semanas (10-12 °C).",
                    LitrosEstimados = 100,
                    PrecioLitro = (float)9.23,
                    PrecioPaquete1 = (float)15.00,
                    PrecioPaquete6 = (float)85.50,
                    PrecioPaquete12 = (float)162.00,
                    PrecioPaquete24 = (float)306.00,
                    PrecioUnitarioBaseMayoreo = (float)13.50,
                    CostoProduccion = 0,
                    TiempoVida = 300,
                    Activo = true,
                    AptaVentaMayorista = true,
                    FechaRegistrado = DateTime.Now,
                    Imagen = "https://chelar.com.mx/wp-content/uploads/2020/04/minerva-lager-light.png",
                    RutaFondo = "https://cdn.pixabay.com/photo/2018/04/23/11/40/abstract-3343966_640.jpg"
                },
                new Receta
                {
                    Id = 2,
                    Nombre = "Witbier",
                    Descripcion = "Cerveza de trigo estilo belga, refrescante y ligeramente especiado.",
                    Especificaciones = "Alcohol (ABV): ~4.8% | Amargor (IBU): 10-15 | Color (SRM): 3-5 | Carbonatación: Alta (~2.5-3.0 volúmenes de CO₂). | Fermentación: 2-3 semanas (~18-22 °C).",
                    LitrosEstimados = 100,
                    PrecioLitro = (float)8.60,
                    PrecioPaquete1 = (float)14.50,
                    PrecioPaquete6 = (float)82.65,
                    PrecioPaquete12 = (float)156.60,
                    PrecioPaquete24 = (float)294.00,
                    PrecioUnitarioBaseMayoreo = (float)11.50,
                    CostoProduccion = 0,
                    TiempoVida = 300,
                    Activo = true,
                    AptaVentaMayorista = true,
                    FechaRegistrado = DateTime.Now,
                    Imagen = "https://cerveceriaallende.mx/wp-content/uploads/elementor/thumbs/trigo-qj4t8utib67oooek6mcnhy200ptu0c01gs7vqzd7gk.png",
                    RutaFondo = "https://www.creativefabrica.com/wp-content/uploads/2021/07/31/Sunburst-blue-Background-Wallpaper-Free-Graphics-15314141-1-1-580x352.jpg"
                },
                new Receta
                {
                    Id = 3,
                    Nombre = "Stout",
                    Descripcion = "Una cerveza robusta, con sabores a malta tostada y cuerpo cremoso.",
                    Especificaciones = "Alcohol (ABV): ~6.0% | Amargor (IBU): 30-40 | Color (SRM): 30-40 | Carbonatación: Media (~2.0-2.5 volúmenes de CO₂).| Fermentación: 4-5 semanas (~18-20 °C).",
                    LitrosEstimados = 100,
                    PrecioLitro = (float)12.16,
                    PrecioPaquete1 = (float)16.50,
                    PrecioPaquete6 = (float)93.83,
                    PrecioPaquete12 = (float)178.20,
                    PrecioPaquete24 = (float)336.00,
                    PrecioUnitarioBaseMayoreo = (float)14.50,
                    CostoProduccion = 0,
                    TiempoVida = 300,
                    Activo = true,
                    AptaVentaMayorista = true,
                    FechaRegistrado = DateTime.Now,
                    Imagen = "https://labelga.com.mx/cdn/shop/products/MinervaStout.001.png?v=1648506482",
                    RutaFondo = "https://static.vecteezy.com/system/resources/thumbnails/001/849/553/small/modern-gold-background-free-vector.jpg"
                }
                );

            modelBuilder.Entity<IngredienteReceta>().HasData(
                // Ingredientes para Lager (Id = 1)
                new IngredienteReceta { IdReceta = 1, IdInsumo = 1, Cantidad = (float)90},  // Agua
                new IngredienteReceta { IdReceta = 1, IdInsumo = 8, Cantidad = (float)18 },   // Malta
                new IngredienteReceta { IdReceta = 1, IdInsumo = 4, Cantidad = (float)0.25 }, // Lúpulo
                new IngredienteReceta { IdReceta = 1, IdInsumo = 3, Cantidad = (float)0.5 },  // Levadura
                new IngredienteReceta { IdReceta = 1, IdInsumo = 10, Cantidad = (float)0.5 }, // Azúcar

                // Ingredientes para Witbier (Id = 2)
                new IngredienteReceta { IdReceta = 2, IdInsumo = 1, Cantidad = (float)91 },  // Agua
                new IngredienteReceta { IdReceta = 2, IdInsumo = 8, Cantidad = (float)12 },   // Malta
                new IngredienteReceta { IdReceta = 2, IdInsumo = 5, Cantidad = (float)6 },    // Trigo
                new IngredienteReceta { IdReceta = 2, IdInsumo = 4, Cantidad = (float)0.15 }, // Lúpulo
                new IngredienteReceta { IdReceta = 2, IdInsumo = 3, Cantidad = (float)0.6 },  // Levadura
                new IngredienteReceta { IdReceta = 2, IdInsumo = 9, Cantidad = (float)0.05 }, // Sal
                new IngredienteReceta { IdReceta = 2, IdInsumo = 10, Cantidad = (float)0.3 }, // Azúcar

                // Ingredientes para Stout (Id = 3)
                new IngredienteReceta { IdReceta = 3, IdInsumo = 1, Cantidad = (float)88 },  // Agua
                new IngredienteReceta { IdReceta = 3, IdInsumo = 8, Cantidad = (float)20 },   // Malta
                new IngredienteReceta { IdReceta = 3, IdInsumo = 7, Cantidad = (float)3 },    // Avena
                new IngredienteReceta { IdReceta = 3, IdInsumo = 4, Cantidad = (float)0.35 }, // Lúpulo
                new IngredienteReceta { IdReceta = 3, IdInsumo = 3, Cantidad = (float)0.7 },  // Levadura
                new IngredienteReceta { IdReceta = 3, IdInsumo = 10, Cantidad = (float)0.4 }  // Azúcar
            );

            modelBuilder.Entity<LoteInsumo>().HasData(
                // Agua - Bonafont
                new LoteInsumo
                {
                    Id = 1,
                    IdProveedor = 1,
                    IdInsumo = 1,
                    IdUsuario = "U2",
                    Cantidad = (float)3000,
                    FechaCaducidad = DateTime.Now.AddMonths(6),
                    FechaCompra = DateTime.Now,
                    PrecioUnidad = (float)0.15,
                    MontoCompra = (float)450,
                    Merma = 0
                },

                // Malta - Maseca
                new LoteInsumo
                {
                    Id = 2,
                    IdProveedor = 2,
                    IdInsumo = 8,
                    IdUsuario = "U2",
                    Cantidad = (float)500,
                    FechaCaducidad = DateTime.Now.AddMonths(12),
                    FechaCompra = DateTime.Now,
                    PrecioUnidad = (float)25,
                    MontoCompra = (float)12500,
                    Merma = 0
                },

                // Lúpulo - La Costeña
                new LoteInsumo
                {
                    Id = 3,
                    IdProveedor = 3,
                    IdInsumo = 4,
                    IdUsuario = "U2",
                    Cantidad = (float)7.5,
                    FechaCaducidad = DateTime.Now.AddMonths(18),
                    FechaCompra = DateTime.Now,
                    PrecioUnidad = (float)800,
                    MontoCompra = (float)6000,
                    Merma = 0
                },

                // Levadura - La Costeña
                new LoteInsumo
                {
                    Id = 4,
                    IdProveedor = 3,
                    IdInsumo = 3,
                    IdUsuario = "U2",
                    Cantidad = (float)18,
                    FechaCaducidad = DateTime.Now.AddMonths(9),
                    FechaCompra = DateTime.Now,
                    PrecioUnidad = (float)500,
                    MontoCompra = (float)9000,
                    Merma = 0
                },

                // Trigo - Maseca
                new LoteInsumo
                {
                    Id = 5,
                    IdProveedor = 2,
                    IdInsumo = 5,
                    IdUsuario = "U2",
                    Cantidad = (float)60,
                    FechaCaducidad = DateTime.Now.AddMonths(12),
                    FechaCompra = DateTime.Now,
                    PrecioUnidad = (float)20,
                    MontoCompra = (float)1200,
                    Merma = 0
                },

                // Avena - Maseca
                new LoteInsumo
                {
                    Id = 6,
                    IdProveedor = 2,
                    IdInsumo = 7,
                    IdUsuario = "U2",
                    Cantidad = (float)30,
                    FechaCaducidad = DateTime.Now.AddMonths(12),
                    FechaCompra = DateTime.Now,
                    PrecioUnidad = (float)22,
                    MontoCompra = (float)660,
                    Merma = 0
                },

                // Sal - La Costeña
                new LoteInsumo
                {
                    Id = 7,
                    IdProveedor = 3,
                    IdInsumo = 9,
                    IdUsuario = "U2",
                    Cantidad = (float)0.5,
                    FechaCaducidad = DateTime.Now.AddMonths(5),
                    FechaCompra = DateTime.Now,
                    PrecioUnidad = (float)10,
                    MontoCompra = (float)5,
                    Merma = 0
                },

                // Azúcar - La Costeña
                new LoteInsumo
                {
                    Id = 8,
                    IdProveedor = 3,
                    IdInsumo = 10,
                    IdUsuario = "U2",
                    Cantidad = (float)12,
                    FechaCaducidad = DateTime.Now.AddMonths(18),
                    FechaCompra = DateTime.Now,
                    PrecioUnidad = (float)18,
                    MontoCompra = (float)216,
                    Merma = 0
                },

                // Botellas - Bonafont
                new LoteInsumo
                {
                    Id = 9,
                    IdProveedor = 1,
                    IdInsumo = 2,
                    IdUsuario = "U2",
                    Cantidad = (float)8460,
                    FechaCaducidad = DateTime.Now.AddMonths(60),
                    FechaCompra = DateTime.Now,
                    PrecioUnidad = (float)6,
                    MontoCompra = (float)50760,
                    Merma = 0
                }
            );


        }
    }
}
