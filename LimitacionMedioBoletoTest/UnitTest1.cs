using Microsoft.EntityFrameworkCore;
using TarjetaSUBE;

namespace LimitacionMedioBoleto
{
    public class Tests
    {
        private TarjetaSUBEContext _db = null!;
        private Tarjeta _tarjeta = null!;
        private Colectivo _colectivo = null!;

        private const int PrecioCompleto = 1500;
        private const int SaldoInicial = 10000;

        [SetUp]
        public void Setup()
        {
            var opciones = new DbContextOptionsBuilder<TarjetaSUBEContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _db = new TarjetaSUBEContext(opciones);
            Contexto.Db = _db;

            var tipoFranquiciaCompleta = new Tarjeta_tipo()
            {
                id_Tipo = 4,
                nombre_Tipo = "Boleto educativo gratuito",
                porcentaje_pago = 0
            };

            _db.Tarjeta_tipos.Add(tipoFranquiciaCompleta);
            _db.SaveChanges();

            _tarjeta = new Tarjeta(SaldoInicial)
            {
                tarjeta_tipo = tipoFranquiciaCompleta.id_Tipo
            };
            _db.SaveChanges();

            _colectivo = new Colectivo()
            {
                IdColectivo = 1,
                Linea = "123",
                Matricula = "ABC 123"
            };
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        [Test]
        public void NoSePuedenRealizarMasDeDosViajesGratisPorDia()
        {
            Assert.That(_colectivo.pagarCon(_tarjeta), Is.True);
            Assert.That(_tarjeta.Saldo, Is.EqualTo(SaldoInicial));

            Assert.That(_colectivo.pagarCon(_tarjeta), Is.True);
            Assert.That(_tarjeta.Saldo, Is.EqualTo(SaldoInicial));

            Assert.That(_colectivo.pagarCon(_tarjeta), Is.True);

            Assert.That(_tarjeta.Saldo, Is.EqualTo(SaldoInicial - PrecioCompleto));
        }

        [Test]
        public void ViajesPosterioresAlSegundoSeCobranPrecioCompleto()
        {
            Assert.That(_colectivo.pagarCon(_tarjeta), Is.True);
            Assert.That(_colectivo.pagarCon(_tarjeta), Is.True);

            Assert.That(_tarjeta.Saldo, Is.EqualTo(SaldoInicial));

            Assert.That(_colectivo.pagarCon(_tarjeta), Is.True);
            Assert.That(_tarjeta.Saldo, Is.EqualTo(SaldoInicial - PrecioCompleto));

            Assert.That(_colectivo.pagarCon(_tarjeta), Is.True);

            Assert.That(_tarjeta.Saldo, Is.EqualTo(SaldoInicial - 2 * PrecioCompleto));
        }
    }
}
