using Microsoft.EntityFrameworkCore;
using TarjetaSUBE;

namespace LimitacionFranquiciaTests
{
    public class LimitacionFranquiciaTests
    {
        private TarjetaSUBEContext _db = null!;
        private Tarjeta_tipo _tipo = null!;
        private Tarjeta _tarjeta = null!;
        private Colectivo _colectivo = null!;

        [SetUp]
        public void Setup()
        {
            var opciones = new DbContextOptionsBuilder<TarjetaSUBEContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _db = new TarjetaSUBEContext(opciones);
            Contexto.Db = _db;

            _tipo = new Tarjeta_tipo
            {
                id_Tipo = 1,
                nombre_Tipo = "Boleto gratuito estudiantil",
                porcentaje_pago = 0
            };
            _db.Tarjeta_tipos.Add(_tipo);
            _db.SaveChanges();

            _tarjeta = new Tarjeta(6000, _tipo.id_Tipo)
            {
                tipo = _tipo
            };

            _colectivo = new Colectivo
            {
                IdColectivo = 1,
                Linea = "123",
                Matricula = "ABC 123"
            };
            _db.Colectivos.Add(_colectivo);
            _db.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        [Test]
        public void SoloLosDosPrimerosViajesDelDiaSonGratuitos()
        {
            _colectivo.pagarCon(_tarjeta);
            Assert.That(_tarjeta.Saldo, Is.EqualTo(6000));

            _colectivo.pagarCon(_tarjeta);
            Assert.That(_tarjeta.Saldo, Is.EqualTo(6000));

            _colectivo.pagarCon(_tarjeta);
            Assert.That(_tarjeta.Saldo, Is.EqualTo(4500));
        }

        [Test]
        public void LosViajesPosterioresAlSegundoSeCobranAlPrecioCompleto()
        {
            _colectivo.pagarCon(_tarjeta);
            _colectivo.pagarCon(_tarjeta);

            _colectivo.pagarCon(_tarjeta);
            Assert.That(_tarjeta.Saldo, Is.EqualTo(4500));

            _colectivo.pagarCon(_tarjeta);
            Assert.That(_tarjeta.Saldo, Is.EqualTo(3000));
        }
    }
}
