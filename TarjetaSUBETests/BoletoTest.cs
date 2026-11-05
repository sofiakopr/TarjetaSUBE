using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TarjetaSUBE;

namespace BoletoTest
{
    public class BoletoTest
    {
        private TarjetaSUBEContext _db = null!;
        private Tarjeta_tipo[] _tipos = null!;
        private Tarjeta[] _tarjetas = null!;
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
                new Tarjeta_tipo { id_Tipo = 3, nombre_Tipo = "Boleto gratuito estudiantil", porcentaje_pago = 0 },
                new Tarjeta_tipo { id_Tipo = 4, nombre_Tipo = "Franquicia completa", porcentaje_pago = 0 }
            ];
            _db.Tarjeta_tipos.AddRange(_tipos);

            _colectivo = new Colectivo
            {
                IdColectivo = 1,
                Linea = "123",
                Matricula = "ABC 123"
            };
            _db.Colectivos.Add(_colectivo);
            _db.SaveChanges();

            _tarjetas = _tipos
                .Select(tipo => new Tarjeta(0, tipo.id_Tipo) { Tipo = tipo })
                .ToArray();
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        [Test]
        [TestCase(1, "Tarifa general", -150)]
        [TestCase(2, "Medio boleto estudiantil", 925)]
        [TestCase(3, "Boleto gratuito estudiantil", 2000)]
        [TestCase(4, "Franquicia completa", 2000)]
        public void CrearBoletoAsignaTarjeta(int tipoId, string nombreTipo, int saldoEsperado)
        {
            var tarjeta = ObtenerTarjeta(tipoId);
            tarjeta.Cargar(2000);

            if (tipoId == 2)
            {
                tarjeta.fecha_ultimo_cobro = DateTime.Now.AddMinutes(-6);
            }
            else if (tipoId == 4)
            {
                tarjeta.fecha_ultimo_cobro = DateTime.Now;
            }

            tarjeta.Cobrar(2150m, _colectivo);
            var boleto = _db.Boletos.Single();

            var boletoGuardado = _db.Boletos.Single();

            Assert.That(boletoGuardado.Return_Tipo_Tarjeta(), Is.EqualTo(nombreTipo));
            Assert.That(boletoGuardado.Return_Linea(), Is.EqualTo(_colectivo.Linea));
            Assert.That(boletoGuardado.Return_Total_Abonado(), Is.EqualTo(tipoId == 2 ? 1075 : tipoId >= 3 ? 0 : 2150));
            Assert.That(boletoGuardado.Return_Saldo_Restante(), Is.EqualTo(saldoEsperado));
            Assert.That(boletoGuardado.Return_ID_Tarjeta(), Is.EqualTo(tarjeta.IdTarjeta));
            Assert.That(tarjeta.Saldo, Is.EqualTo(saldoEsperado));
            Assert.That(DateTime.TryParseExact(
                boletoGuardado.Return_Fecha(),
                "yyyy-MM-dd HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out _), Is.True);
        }

        [Test]
        public void CrearBoletoFallaCuandoElDebitoSuperaElLimiteNegativo()
        {
            var tarjeta = ObtenerTarjeta(1);
            tarjeta.Cargar(4000);

            tarjeta.Cobrar(2150m, _colectivo);
            tarjeta.Cobrar(2150m, _colectivo);

            Assert.That(() => tarjeta.Cobrar(2150m, _colectivo), Throws.TypeOf<ArgumentException>());
            Assert.That(_db.Boletos.Count(), Is.EqualTo(2));
            Assert.That(tarjeta.Saldo, Is.EqualTo(-300));
        }

        [Test]
        public void CrearBoletoMedioBoletoFallaDuranteElTiempoDeEspera()
        {
            var tarjeta = ObtenerTarjeta(2);
            tarjeta.Cargar(2000);
            tarjeta.fecha_ultimo_cobro = DateTime.Now;

            var exception = Assert.Throws<Exception>(() => tarjeta.Cobrar(2150m, _colectivo));

            Assert.That(exception!.Message, Is.EqualTo("No se puede cobrar el viaje, ya que no han pasado 5 minutos desde el último cobro de medio boleto"));
            Assert.That(_db.Boletos, Is.Empty);
            Assert.That(tarjeta.Saldo, Is.EqualTo(2000));
        }

        private Tarjeta ObtenerTarjeta(int tipoId)
        {
            return _tarjetas.Single(tarjeta => tarjeta.tarjeta_tipo == tipoId);
        }
    }
}