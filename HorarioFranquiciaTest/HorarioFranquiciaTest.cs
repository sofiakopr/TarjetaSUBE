using Microsoft.EntityFrameworkCore;
using TarjetaSUBE;

namespace HorarioFranquiciaTest
{
    public class Tests
    {
        private TarjetaSUBEContext _db = null!;
        private Tarjeta_tipo[] _tipos = null!;
        private Tarjeta[] _tarjetas = null!;
        private Tarjeta[] _tarjetasEnElLimite = null!;
        private Colectivo _colectivo = null!;
        private readonly DateTime _fechaBase = new(2026, 1, 15, 12, 0, 0);

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

            _colectivo = new Colectivo { IdColectivo = 1, Linea = "123", Matricula = "ABC 123" };
            _db.Colectivos.Add(_colectivo);
            _db.SaveChanges();

            _tarjetas = _tipos
                .Select(tipo => new Tarjeta(0, tipo.id_Tipo) { Tipo = tipo })
                .ToArray();
            _tarjetasEnElLimite = _tipos
                .Select(tipo => new Tarjeta(-500, tipo.id_Tipo) { Tipo = tipo })
                .ToArray();
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        [TestCase(1, 5, 59, 1580)]
        [TestCase(1, 6, 0, 1580)]
        [TestCase(1, 21, 59, 1580)]
        [TestCase(1, 22, 0, 1580)]
        [TestCase(2, 5, 59, 1580)]
        [TestCase(2, 6, 0, 790)]
        [TestCase(2, 21, 59, 790)]
        [TestCase(2, 22, 0, 1580)]
        [TestCase(3, 5, 59, 1580)]
        [TestCase(3, 6, 0, 0)]
        [TestCase(3, 21, 59, 0)]
        [TestCase(3, 22, 0, 1580)]
        [TestCase(4, 5, 59, 1580)]
        [TestCase(4, 6, 0, 0)]
        [TestCase(4, 21, 59, 0)]
        [TestCase(4, 22, 0, 1580)]
        public void AplicaBeneficiosEntreLasSeisYLasVeintidos(
            int tipoId,
            int hora,
            int minuto,
            int montoDebitado)
        {
            var tarjeta = ObtenerTarjeta(_tarjetas, tipoId);
            tarjeta.Cargar(2000);

            var fechaActual = new DateTime(_fechaBase.Year, _fechaBase.Month, _fechaBase.Day, hora, minuto, 0);
            var estaEnHorario = hora >= 6 && hora < 22;
            if (tipoId == 2 && estaEnHorario)
            {
                tarjeta.fecha_ultimo_cobro = fechaActual.AddMinutes(-6);
            }
            else if (tipoId == 4 && estaEnHorario)
            {
                tarjeta.fecha_ultimo_cobro = fechaActual;
            }

            var boleto = new Boleto(tarjeta, _colectivo, fechaActual);

            Assert.That(boleto.Return_Tipo_Tarjeta(), Is.EqualTo(_tipos.Single(tipo => tipo.id_Tipo == tipoId).nombre_Tipo));
            Assert.That(boleto.Return_Linea(), Is.EqualTo(_colectivo.Linea));
            Assert.That(boleto.Return_Total_Abonado(), Is.EqualTo(1580));
            Assert.That(boleto.Return_Saldo_Restante(), Is.EqualTo(2000 - montoDebitado));
            Assert.That(tarjeta.Saldo, Is.EqualTo(2000 - montoDebitado));
            Assert.That(_db.Boletos.Count(), Is.EqualTo(1));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void FueraDelHorarioElCobroCompletoRespetaElLimiteNegativo(int tipoId)
        {
            var tarjeta = ObtenerTarjeta(_tarjetasEnElLimite, tipoId);
            var fueraDeHorario = new DateTime(_fechaBase.Year, _fechaBase.Month, _fechaBase.Day, 5, 59, 0);

            Assert.That(() => new Boleto(tarjeta, _colectivo, fueraDeHorario), Throws.TypeOf<ArgumentException>());
            Assert.That(tarjeta.Saldo, Is.EqualTo(-500));
            Assert.That(_db.Boletos, Is.Empty);
        }

        [Test]
        public void MedioBoletoFallaDuranteElTiempoDeEsperaDentroDelHorario()
        {
            var tarjeta = ObtenerTarjeta(_tarjetas, 2);
            tarjeta.Cargar(2000);
            var fechaActual = new DateTime(_fechaBase.Year, _fechaBase.Month, _fechaBase.Day, 12, 0, 0);
            tarjeta.fecha_ultimo_cobro = fechaActual;

            var exception = Assert.Throws<Exception>(() => new Boleto(tarjeta, _colectivo, fechaActual));

            Assert.That(exception!.Message, Is.EqualTo("No se puede cobrar el viaje, ya que no han pasado 5 minutos desde el último cobro de medio boleto"));
            Assert.That(tarjeta.Saldo, Is.EqualTo(2000));
            Assert.That(_db.Boletos, Is.Empty);
        }

        private static Tarjeta ObtenerTarjeta(Tarjeta[] tarjetas, int tipoId)
        {
            return tarjetas.Single(tarjeta => tarjeta.tarjeta_tipo == tipoId);
        }
    }
}
