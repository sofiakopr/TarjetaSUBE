using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TarjetaSUBE;

namespace MasSobreElBoletoTest
{
    public class MasSobreElBoletoTest
    {
        private TarjetaSUBEContext _db = null!;
        private Tarjeta_tipo[] _tipos = null!;
        private Colectivo _colectivo = null!;

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
                new Tarjeta_tipo { id_Tipo = 3, nombre_Tipo = "Tipo 3", porcentaje_pago = 0 },
                new Tarjeta_tipo { id_Tipo = 4, nombre_Tipo = "Franquicia completa", porcentaje_pago = 0 }
            ];
            _db.Tarjeta_tipos.AddRange(_tipos);
            _colectivo = new Colectivo { Linea = "123", Matricula = "ABC 123" };
            _db.Colectivos.Add(_colectivo);
            _db.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        [Test]
        [TestCase(1, "Tarifa general", 1580, 420)]
        [TestCase(2, "Medio boleto estudiantil", 1580, 1210)]
        [TestCase(3, "Tipo 3", 1580, 2000)]
        [TestCase(4, "Franquicia completa", 1580, 2000)]
        public void BoletoInformaDatosSegunTipoDeTarjeta(int tipoId, string nombreTipo, int totalAbonado, int saldoRestante)
        {
            var tarjeta = new Tarjeta(2000)
            {
                tarjeta_tipo = tipoId,
                Tipo = _tipos.Single(tipo => tipo.id_Tipo == tipoId)
            };

            Assert.That(_colectivo.pagarCon(tarjeta), Is.True);

            var boleto = _db.Boletos.Single();

            Assert.That(DateTime.TryParseExact(
                boleto.Return_Fecha(),
                "yyyy-MM-dd HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _), Is.True);
            Assert.That(boleto.Return_Tipo_Tarjeta(), Is.EqualTo(nombreTipo));
            Assert.That(boleto.Return_Linea(), Is.EqualTo(_colectivo.Linea));
            Assert.That(boleto.Return_Total_Abonado(), Is.EqualTo(totalAbonado));
            Assert.That(boleto.Return_Saldo_Restante(), Is.EqualTo(saldoRestante));
            Assert.That(boleto.Return_ID_Tarjeta(), Is.EqualTo(tarjeta.IdTarjeta));
            Assert.That(tarjeta.Saldo, Is.EqualTo(saldoRestante));
        }

        [Test]
        public void CobrarIncluyeDeudaPreviaEnDebito()
        {
            var tarjeta = new Tarjeta(-100)
            {
                tarjeta_tipo = 1,
                Tipo = _tipos.Single(tipo => tipo.id_Tipo == 1)
            };

            Assert.That(_colectivo.pagarCon(tarjeta), Is.True);

            var boleto = _db.Boletos.Single();

            Assert.That(boleto.Return_Total_Abonado(), Is.EqualTo(1580));
            Assert.That(-100 - tarjeta.Saldo, Is.EqualTo(1680));
            Assert.That(boleto.Return_Saldo_Restante(), Is.EqualTo(-1780));
            Assert.That(tarjeta.Saldo, Is.EqualTo(-1780));
        }

        [Test]
        public void NoBoletoCuandoSaldoEsMenorALimiteDeViaje()
        {
            var tarjeta = new Tarjeta(-501)
            {
                tarjeta_tipo = 1,
                Tipo = _tipos.Single(tipo => tipo.id_Tipo == 1)
            };

            Assert.That(_colectivo.pagarCon(tarjeta), Is.False);
            Assert.That(_db.Boletos, Is.Empty);
            Assert.That(tarjeta.Saldo, Is.EqualTo(-501));
        }

        [Test]
        public void CobrarFallaSiDebitoSuperaLimiteNegativo()
        {
            var tarjeta = new Tarjeta(-500)
            {
                tarjeta_tipo = 1,
                Tipo = _tipos.Single(tipo => tipo.id_Tipo == 1)
            };

            Assert.That(() => _colectivo.pagarCon(tarjeta), Throws.TypeOf<ArgumentException>());
            Assert.That(_db.Boletos, Is.Empty);
            Assert.That(tarjeta.Saldo, Is.EqualTo(-500));
        }
    }
}