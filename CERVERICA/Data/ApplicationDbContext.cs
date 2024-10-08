using CERVERICA.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace CERVERICA.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

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
        public DbSet<Comprador> Compradores { get; set; }
        public DbSet<InteraccionComprador> InteraccionesCompradores { get; set; }
        public DbSet<PreferenciasComprador> PreferenciasCompradores { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

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


            modelBuilder.Entity<InteraccionComprador>()
                .HasOne(i => i.Usuario)
                .WithMany()
                .HasForeignKey(i => i.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PreferenciasComprador>()
                .HasOne(p => p.Usuario)
                .WithMany()
                .HasForeignKey(p => p.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
