using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TarjetaSUBE;

namespace TarjetaTipoTest
{
    public class TarjetaTipoTests
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

            tarjeta.Cobrar(1580);

            Assert.That(tarjeta.Saldo, Is.EqualTo(0));
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

            tarjeta2.Cobrar(1580);

            Assert.That(tarjeta2.Saldo, Is.EqualTo(1210));
        }
    }
}