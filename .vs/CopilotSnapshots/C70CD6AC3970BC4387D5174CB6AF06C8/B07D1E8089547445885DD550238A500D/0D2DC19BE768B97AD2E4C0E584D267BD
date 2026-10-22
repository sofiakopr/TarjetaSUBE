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
        public void CrearColectivo()
        {
            var colectivo = new Colectivo()
            {
                IdColectivo = 98,
                Linea = "123",
                Matricula = "ABC 123"
            };

            _db.Colectivos.Add(colectivo);
            _db.SaveChanges();
            var colectivoGuardado = _db.Colectivos.Single();

            Assert.That(colectivoGuardado, Is.SameAs(colectivo));
            Assert.That(colectivoGuardado.IdColectivo, Is.EqualTo(colectivo.IdColectivo));
            Assert.That(colectivoGuardado.Linea, Is.EqualTo(colectivo.Linea));
            Assert.That(colectivoGuardado.Matricula, Is.EqualTo(colectivo.Matricula));
        }

        public void ColectivoPagarCon()
        {
            var tarjeta = new Tarjeta()
            {
                Saldo = 2000,
                IdTarjeta = 1,
                tarjeta_tipo = 1
            };

            var colectivo2 = new Colectivo()
            {
                IdColectivo = 99,
                Linea = "321",
                Matricula = "ABC 321"
            };

            var pagoExitoso = colectivo2.pagarCon(tarjeta);

            Assert.That(pagoExitoso, Is.True);

        }
    }
}