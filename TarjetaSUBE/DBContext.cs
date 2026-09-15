using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace tarjetaSUBE
{
    public class ClinicaContext : DbContext
    {
        public DbSet<Tarjeta> Tarjetas { get; set; }
        public DbSet<Colectivo> Colectivos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(@"Data Source=C: ..\tarjetaSUBE.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tarjeta>()
                .ToTable("tarjeta")
                .HasKey(t => t.IdTarjeta);
            modelBuilder.Entity<Tarjeta>()
                .Property(t => t.IdTarjeta).HasColumnName("id_Tarjeta");

            modelBuilder.Entity<Colectivo>()
                .ToTable("colectivo")
                .HasKey(c => c.IdColectivo);
            modelBuilder.Entity<Colectivo>()
                .Property(c => c.IdColectivo).HasColumnName("id_Colectivo");

        }
    }
}