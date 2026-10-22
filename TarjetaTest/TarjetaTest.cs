using Microsoft.EntityFrameworkCore;
using TarjetaSUBE;

namespace CobrarSaldosNegativoYPositivoTests
{
	public class CobrarSaldosNegativoYPositivoTests
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
		public void CobrarPasajeConSaldoNegativo()
		{
			var tarjeta = new Tarjeta(-2000);

			Contexto.Db.Tarjetas.Add(tarjeta);

			var tarjetaGuardada = _db.Tarjetas.Single();

            Assert.That(tarjetaGuardada, Is.SameAs(tarjeta));
            Assert.That(
				() => tarjeta.Cobrar(1),
				Throws.TypeOf<ArgumentException>());
		}

        [Test]
        public void CobrarPasajeConSaldoPositivo()
        {
            var tarjeta2 = new Tarjeta(2000);

            Contexto.Db.Tarjetas.Add(tarjeta2);

            var tarjetaGuardada = _db.Tarjetas.Single();

            Assert.That(tarjetaGuardada, Is.SameAs(tarjeta2));
            Assert.That(
                () => tarjeta2.Cobrar(1580),
                Throws.TypeOf<ArgumentException>());
        }
    }
}


