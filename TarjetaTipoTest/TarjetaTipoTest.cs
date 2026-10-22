using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TarjetaSUBE;

namespace FranquiciaBoletoYTarjetaTipoTest
{
    public class FranquiciaBoletoYTarjetaTipoTest
    {
        private TarjetaSUBEContext _db = null!;

        [SetUp]
        public void Setup()
        {
            var opciones = new DbContextOptionsBuilder<TarjetaSUBEContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _db = new TarjetaSUBEContext(opciones);
            Contexto.Db = _db;
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        [Test]
        public void FranquiciaCompletaTest()
        { 

            var tarjeta_tipo = new Tarjeta_tipo()
            {
                id_Tipo = 4,
                nombre_Tipo = "Franquicia completa",
                porcentaje_pago = 0
            };

            Contexto.Db.Tarjeta_tipos.Add(tarjeta_tipo);

            var tarjeta = new Tarjeta(0)
            {
                tarjeta_tipo = tarjeta_tipo.id_Tipo
            };

            var tarjetaGuardada = _db.Tarjetas.Single();

            tarjeta.Cobrar(1580);

            Assert.That(tarjetaGuardada, Is.SameAs(tarjeta));
            Assert.That(tarjetaGuardada.IdTarjeta, Is.EqualTo(tarjeta.IdTarjeta));
            Assert.That(tarjetaGuardada.tarjeta_tipo, Is.EqualTo(tarjeta_tipo.id_Tipo));
            Assert.That(tarjetaGuardada.Saldo, Is.EqualTo(0));
        }

        [Test]
        public void MedioBoletoTest()
        {
            var tarjeta_tipo = new Tarjeta_tipo()
            {
                id_Tipo = 2,
                nombre_Tipo = "Medio boleto estudiantil",
                porcentaje_pago = 50
            };

            Contexto.Db.Tarjeta_tipos.Add(tarjeta_tipo);

            var tarjeta2 = new Tarjeta(2000)
            {
                tarjeta_tipo = tarjeta_tipo.id_Tipo
            };

            var tarjetaGuardada2 = _db.Tarjetas.Single();

            tarjeta2.Cobrar(1580);

            Assert.That(tarjetaGuardada2, Is.SameAs(tarjeta2));
            Assert.That(tarjetaGuardada2.IdTarjeta, Is.EqualTo(tarjeta2.IdTarjeta));
            Assert.That(tarjetaGuardada2.tarjeta_tipo, Is.EqualTo(tarjeta_tipo.id_Tipo));
            Assert.That(tarjeta2.Saldo, Is.EqualTo(1210));
        }
    }
}