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
        public DbSet<Tarjeta_tipo> Tarjeta_tipos => Set<Tarjeta_tipo>();
        public DbSet<Bicicleta> Bicicletas => Set<Bicicleta>();
        public DbSet<Estacion_Bici> Estaciones_Bici => Set<Estacion_Bici>();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlite(@"Data Source=C: ..\tarjetaSUBE.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tarjeta>()
                .ToTable("tarjeta")
                .HasKey(t => t.IdTarjeta);
            modelBuilder.Entity<Tarjeta>()
                .Property(t => t.IdTarjeta).HasColumnName("id_Tarjeta");
            modelBuilder.Entity<Tarjeta>()
                .HasOne(t => t.Tipo)
                .WithMany()
                .HasForeignKey("tarjeta_tipo");

            modelBuilder.Entity<Tarjeta_tipo>()
                .ToTable("tarjeta_tipo")
                .HasKey("id_Tipo");

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
                .HasForeignKey("id_Tarjeta");
            modelBuilder.Entity<Boleto>()
                .HasOne(b => b.Colectivo)
                .WithMany()
                .HasForeignKey("id_Colectivo");

            modelBuilder.Entity<Bicicleta>()
                .ToTable("bicicleta")
                .HasKey(b => b.id);
            modelBuilder.Entity<Bicicleta>()
                .Property(b => b.estacionada)
                .HasColumnName("estacionada")
                .HasConversion(
                    v => v ? 1 : 0,
                    v => v == 1);

            modelBuilder.Entity<Estacion_Bici>()
                .ToTable("estacion_Bici")
                .HasKey(e => e.id);
            modelBuilder.Entity<Estacion_Bici>()
                .Property(e => e.limite).HasColumnName("limite");
        }
    }
}