using Microsoft.EntityFrameworkCore;
using TarjetaSUBE;

namespace TarjetaTest
{
	public class TarjetaTests
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
		public void Cobrar()
		{
			var tarjeta = new Tarjeta(-2000);

			// Ensure the entity is tracked by the test DbContext before calling Cobrar,
			// since the entity constructor no longer adds/saves itself.
			Contexto.Db.Tarjetas.Add(tarjeta);

			Assert.That(
				() => tarjeta.Cobrar(1),
				Throws.TypeOf<ArgumentException>());
		}

        [Test]
        public void Cobrar()
        {
            var tarjeta2 = new Tarjeta(2000);

            Contexto.Db.Tarjetas.Add(tarjeta2);

            Assert.That(
                () => tarjeta2.Cobrar(1580),
                Throws.TypeOf<ArgumentException>());
        }
    }
}


