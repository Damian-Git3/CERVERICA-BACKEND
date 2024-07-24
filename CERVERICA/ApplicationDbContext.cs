using CERVERICA.Models;
using Microsoft.EntityFrameworkCore;

namespace CERVERICA
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Insumo> Insumos { get; set; }
        public DbSet<LoteInsumo> LotesInsumos { get; set; }
        public DbSet<Receta> Recetas { get; set; }
        public DbSet<IngredienteReceta> IngredientesReceta { get; set; }
        public DbSet<Produccion> Producciones { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<LogLogin> LogsLogin { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuraciones adicionales si es necesario
            modelBuilder.Entity<IngredienteReceta>()
                .HasKey(ir => new { ir.IdReceta, ir.IdInsumo });

            modelBuilder.Entity<DetalleVenta>()
                .HasKey(dv => new { dv.IdVenta, dv.IdStock });

            modelBuilder.Entity<Compra>()
                .HasKey(c => new { c.IdUsuario, c.LotesInsumosId, c.LotesInsumosIdProveedor, c.LotesInsumosIdInsumo });

            base.OnModelCreating(modelBuilder);
        }
    }
}
