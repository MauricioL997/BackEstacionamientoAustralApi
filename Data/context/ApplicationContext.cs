using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.context
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Estacionamiento> Estacionamientos { get; set; }
        public DbSet<Cochera> Cocheras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar la relación uno a muchos entre Cochera y Estacionamiento
            modelBuilder.Entity<Estacionamiento>()
                .HasOne(e => e.Cochera)
                .WithMany(c => c.Estacionamientos)
                .HasForeignKey(e => e.IdCochera);
        }
    }
}
