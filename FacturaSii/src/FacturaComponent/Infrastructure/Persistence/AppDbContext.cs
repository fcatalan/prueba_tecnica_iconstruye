// DTE.Infrastructure/Persistence/AppDbContext.cs
using FacturaSii.src.FacturaComponent.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace FacturaSii.src.FacturaComponent.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Factura> Facturas { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // Reemplaza la línea problemática con la configuración correcta para PostgreSQL Identity Columns
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1) Habilita Identity Columns globalmente (PostgreSQL ≥10)
            modelBuilder.UseIdentityByDefaultColumns(); // Método correcto para configurar Identity Columns en PostgreSQL

            // 2) Configura Factura.Id como PK y generado al añadir
            modelBuilder.Entity<Factura>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Property(f => f.Id)
                      .ValueGeneratedOnAdd();        // ← Autoincremental
                entity.Property(f => f.Folio).IsRequired();
                //entity.Property(f => f.FechaEmision).IsRequired();

                // Configuración para EstadoRespuesta
                entity.Property(f => f.EstadoRespuesta)
                      .HasMaxLength(50)
                      .IsRequired(false);

                // Configuración para XmlGenerado
                entity.Property(f => f.XmlGenerado)
                      .HasColumnType("xml")
                      .IsRequired(false);

                entity.OwnsMany(f => f.Items, item =>
                {
                    item.WithOwner().HasForeignKey("FacturaId");
                    item.Property<int>("Id");
                    item.HasKey("Id");
                    item.Property(i => i.Descripcion).IsRequired();
                    item.Property(i => i.Cantidad).IsRequired();
                    item.Property(i => i.PrecioUnitario).IsRequired();
                });
            });
        }
    }
}
