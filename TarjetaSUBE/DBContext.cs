using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace TarjetaSUBE
{
    public class TarjetaSUBEContext : DbContext
    {
        public TarjetaSUBEContext() { }

        public TarjetaSUBEContext(DbContextOptions<TarjetaSUBEContext> opciones) : base(opciones) { }

        public DbSet<Tarjeta> Tarjetas => Set<Tarjeta>();
        public DbSet<Colectivo> Colectivos => Set<Colectivo>();
        public DbSet<Boleto> Boletos => Set<Boleto>();

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

            modelBuilder.Entity<Boleto>()
                .ToTable("boleto")
                .HasKey(b => b.IdBoleto);
            modelBuilder.Entity<Boleto>()
                .Property(b => b.IdBoleto).HasColumnName("id_Boleto");
            modelBuilder.Entity<Boleto>()
                .HasOne(b => b.Tarjeta)
                .WithMany()
                .HasForeignKey(b => b.id_Tarjeta);
            modelBuilder.Entity<Boleto>()
                .HasOne(b => b.Colectivo)
                .WithMany()
                .HasForeignKey(b => b.id_Colectivo);
        }
    }
}