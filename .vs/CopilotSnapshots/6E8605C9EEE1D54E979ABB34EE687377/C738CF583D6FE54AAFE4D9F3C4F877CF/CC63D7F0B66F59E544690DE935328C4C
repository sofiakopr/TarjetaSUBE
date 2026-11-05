using Microsoft.EntityFrameworkCore;
using TarjetaSUBE;

namespace LineasInterurbanasTest
{
    public class Tests
    {
        private TarjetaSUBEContext _db = null!;
        private Tarjeta_tipo _tipoGeneral = null!;
        private Tarjeta _tarjeta = null!;
        private Tarjeta _tarjetaEnLimite = null!;
        private Colectivo _colectivoUrbano = null!;
        private Colectivo _colectivoInterurbano = null!;

        [SetUp]
        public void Setup()
        {
            var opciones = new DbContextOptionsBuilder<TarjetaSUBEContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _db = new TarjetaSUBEContext(opciones);
            Contexto.Db = _db;

            _tipoGeneral = new Tarjeta_tipo
            {
                id_Tipo = 1,
                nombre_Tipo = "Tarifa general",
                porcentaje_pago = 100
            };
            _db.Tarjeta_tipos.Add(_tipoGeneral);

            _colectivoUrbano = new Colectivo
            {
                IdColectivo = 1,
                Linea = "123",
                Matricula = "URB 123",
                interurbano = 0
            };
            _colectivoInterurbano = new Colectivo
            {
                IdColectivo = 2,
                Linea = "456",
                Matricula = "INT 456",
                interurbano = 1
            };
            _db.Colectivos.AddRange(_colectivoUrbano, _colectivoInterurbano);
            _db.SaveChanges();

            _tarjeta = new Tarjeta(0, _tipoGeneral.id_Tipo)
            {
                Tipo = _tipoGeneral
            };
            _tarjetaEnLimite = new Tarjeta(-2000, _tipoGeneral.id_Tipo)
            {
                Tipo = _tipoGeneral
            };
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        [Test]
        [TestCase(0, 3000, 2150, 850)]
        [TestCase(1, 4000, 3000, 1000)]
        public void PagarConAplicaLaTarifaSegunElTipoDeLinea(int tipoLinea, int montoCargado, int tarifaEsperada, int saldoEsperado)
        {
            _tarjeta.Cargar(montoCargado);
            var colectivo = ObtenerColectivo(tipoLinea);

            Assert.That(colectivo.pagarCon(_tarjeta), Is.True);

            var boleto = _db.Boletos.Single();
            Assert.That(boleto.Return_Total_Abonado(), Is.EqualTo(tarifaEsperada));
            Assert.That(boleto.Return_Saldo_Restante(), Is.EqualTo(saldoEsperado));
            Assert.That(boleto.Return_Linea(), Is.EqualTo(colectivo.Linea));
            Assert.That(_tarjeta.Saldo, Is.EqualTo(saldoEsperado));
        }

        [Test]
        [TestCase(0, 2150)]
        [TestCase(1, 3000)]
        public void PagarConFallaSiLaTarifaSuperaElLimiteNegativo(int tipoLinea, int tarifa)
        {
            var colectivo = ObtenerColectivo(tipoLinea);

            Assert.That(() => colectivo.pagarCon(_tarjetaEnLimite), Throws.TypeOf<ArgumentException>());
            Assert.That(_db.Boletos, Is.Empty);
            Assert.That(_tarjetaEnLimite.Saldo, Is.EqualTo(-2000));
        }

        [Test]
        public void PagarConRechazaUnValorDeLineaNoSoportado()
        {
            _colectivoUrbano.interurbano = 2;

            Assert.That(_colectivoUrbano.pagarCon(_tarjeta), Is.False);
            Assert.That(_db.Boletos, Is.Empty);
            Assert.That(_tarjeta.Saldo, Is.Zero);
        }

        private Colectivo ObtenerColectivo(int tipoLinea)
        {
            return tipoLinea == 0 ? _colectivoUrbano : _colectivoInterurbano;
        }
    }
}
