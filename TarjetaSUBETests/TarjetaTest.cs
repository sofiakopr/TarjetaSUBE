using Microsoft.EntityFrameworkCore;
using TarjetaSUBE;

namespace CobrarSaldosNegativoYPositivoTests
{
	public class CobrarSaldosNegativoYPositivoTests
	{
		private TarjetaSUBEContext _db = null!;
		private Tarjeta_tipo[] _tipos = null!;
		private Tarjeta[] _tarjetas = null!;

		[SetUp]
		public void Setup()
		{
			var opciones = new DbContextOptionsBuilder<TarjetaSUBEContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			_db = new TarjetaSUBEContext(opciones);
			Contexto.Db = _db;

			_tipos =
			[
				new Tarjeta_tipo { id_Tipo = 1, nombre_Tipo = "Tarifa general", porcentaje_pago = 100 },
				new Tarjeta_tipo { id_Tipo = 2, nombre_Tipo = "Medio boleto estudiantil", porcentaje_pago = 50 },
				new Tarjeta_tipo { id_Tipo = 3, nombre_Tipo = "Boleto gratuito estudiantil", porcentaje_pago = 0 },
				new Tarjeta_tipo { id_Tipo = 4, nombre_Tipo = "Franquicia completa", porcentaje_pago = 0 }
			];
			_db.Tarjeta_tipos.AddRange(_tipos);
			_db.SaveChanges();

			_tarjetas = _tipos
				.Select(tipo => new Tarjeta(-2000, tipo.id_Tipo) { Tipo = tipo })
				.ToArray();
		}

		[TearDown]
		public void TearDown()
		{
			_db.Dispose();
		}

		[Test]
		[TestCase(1, 1, true, -2000)]
		[TestCase(2, 2, false, -2001)]
		[TestCase(3, 1, false, -2000)]
		[TestCase(4, 1, false, -2000)]
		public void CobrarPasajeConSaldoNegativo(int tipoId, int monto, bool debeLanzarExcepcion, int saldoEsperado)
		{
			var tarjeta = ObtenerTarjeta(tipoId);
			if (tipoId == 2)
			{
				tarjeta.fecha_ultimo_cobro = DateTime.Now.AddMinutes(-6);
			}
			else if (tipoId == 4)
			{
				tarjeta.fecha_ultimo_cobro = DateTime.Now;
			}

			if (debeLanzarExcepcion)
			{
				Assert.That(() => tarjeta.Cobrar(monto), Throws.TypeOf<ArgumentException>());
			}
			else
			{
				Assert.That(tarjeta.Cobrar(monto), Is.SameAs(tarjeta));
			}

			Assert.That(tarjeta.Saldo, Is.EqualTo(saldoEsperado));
		}

		[Test]
		[TestCase(1, -150)]
		[TestCase(2, 925)]
		[TestCase(3, 2000)]
		[TestCase(4, 2000)]
		public void CobrarPasajeConSaldoPositivo(int tipoId, int saldoEsperado)
		{
			var tarjeta = ObtenerTarjeta(tipoId);
			tarjeta.Cargar(4000);
			if (tipoId == 2)
			{
				tarjeta.fecha_ultimo_cobro = DateTime.Now.AddMinutes(-6);
			}
			else if (tipoId == 4)
			{
				tarjeta.fecha_ultimo_cobro = DateTime.Now;
			}

			Assert.That(tarjeta.Cobrar(2150), Is.SameAs(tarjeta));
			Assert.That(tarjeta.Saldo, Is.EqualTo(saldoEsperado));
		}

		private Tarjeta ObtenerTarjeta(int tipoId)
		{
			return _tarjetas.Single(tarjeta => tarjeta.tarjeta_tipo == tipoId);
		}
    }
}


