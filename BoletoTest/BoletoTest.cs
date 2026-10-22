using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TarjetaSUBE;

namespace BoletoTest
{
    public class BoletoTest
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
        public void CrearBoletoAsignaTarjeta()
        {
            var tarjeta = new Tarjeta()
            {
                Saldo = 2000,
                IdTarjeta = 1,
                tarjeta_tipo = 1
            };

            var boleto = new Boleto(tarjeta);

            var boletoGuardado = _db.Boletos.Single();

            Assert.That(boletoGuardado, Is.SameAs(boleto));
            Assert.That(boletoGuardado.id_Tarjeta, Is.EqualTo(tarjeta.IdTarjeta));
            Assert.That(boletoGuardado.Tarjeta, Is.SameAs(tarjeta));
            Assert.That(boletoGuardado.Monto, Is.EqualTo(1500));
            Assert.That(DateTime.TryParseExact(
                boletoGuardado.Fecha,
                "yyyy-MM-dd HH:mm:ss",
                null,
                System.Globalization.DateTimeStyles.None,
                out _), Is.True);
        }
    }
}