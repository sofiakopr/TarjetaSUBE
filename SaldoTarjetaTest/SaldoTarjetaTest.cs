using Microsoft.EntityFrameworkCore;
using TarjetaSUBE;

namespace SaldoTarjeta
{
    public class SaldoTarjetaTests
    {
        private TarjetaSUBEContext _db = null!;
        private Tarjeta _tarjeta = null!;

        [SetUp]
        public void Setup()
        {
            var opciones = new DbContextOptionsBuilder<TarjetaSUBEContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _db = new TarjetaSUBEContext(opciones);
            Contexto.Db = _db;
            _tarjeta = new Tarjeta(50000);
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        [Test]
        public void CargarPorEncimaDelLimite_AcreditaHastaElMaximoYDejaExcedentePendiente()
        {
            _tarjeta.Cargar(10000);

            Assert.That(_tarjeta.Saldo, Is.EqualTo(56000));
            Assert.That(_tarjeta.Excedente, Is.EqualTo(4000));
        }

        [Test]
        public void CobrarConExcedentePendiente_AcreditaHastaElMaximoDisponible()
        {
            _tarjeta.Cargar(10000);
            _tarjeta.Cobrar(2000);

            Assert.That(_tarjeta.Saldo, Is.EqualTo(56000));
            Assert.That(_tarjeta.Excedente, Is.EqualTo(2000));
        }

        [TestCase(-2000)]
        [TestCase(1000)]
        public void CargarConMontoInvalido_LanzaExcepcionSinModificarSaldo(int monto)
        {
            Assert.Throws<ArgumentException>(() => _tarjeta.Cargar(monto));

            Assert.That(_tarjeta.Saldo, Is.EqualTo(50000));
            Assert.That(_tarjeta.Excedente, Is.Zero);
        }
    }
}