using System.Globalization;
using Microsoft.EntityFrameworkCore;
using TarjetaSUBE;

namespace BoletoFrecuenteTest
{
    public class BoletoFrecuenteTest
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
                new Tarjeta_tipo { id_Tipo = 1, nombre_Tipo = "Normal", porcentaje_pago = 100 },
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
        [TestCase(0, 1580)]
        [TestCase(28, 1580)]
        [TestCase(29, 1264)]
        [TestCase(78, 1264)]
        [TestCase(79, 1185)]
        [TestCase(80, 1185)]
        public void AplicaDescuentoSegunCantidadDeViajesDelMes(int viajesPrevios, int montoDebitado)
        {
            var tarjeta = ObtenerTarjeta(1);
            tarjeta.Cargar(10000);
            AgregarViajesPrevios(tarjeta, viajesPrevios, DateTime.Now);

            var saldoInicial = tarjeta.Saldo;
            var boleto = new Boleto(tarjeta, _colectivo);

            Assert.That(saldoInicial - tarjeta.Saldo, Is.EqualTo(montoDebitado));
            Assert.That(boleto.Return_Saldo_Restante(), Is.EqualTo(saldoInicial - montoDebitado));
            Assert.That(boleto.Return_Total_Abonado(), Is.EqualTo(1580));
            Assert.That(_db.Boletos.Count(), Is.EqualTo(viajesPrevios + 1));
        }

        [Test]
        public void ReiniciaLaCuentaDeViajesAlCambiarElMes()
        {
            var tarjeta = ObtenerTarjeta(1);
            tarjeta.Cargar(10000);
            AgregarViajesPrevios(tarjeta, 80, DateTime.Now.AddMonths(-1));

            var saldoInicial = tarjeta.Saldo;
            var boleto = new Boleto(tarjeta, _colectivo);

            Assert.That(saldoInicial - tarjeta.Saldo, Is.EqualTo(1580));
            Assert.That(boleto.Return_Saldo_Restante(), Is.EqualTo(saldoInicial - 1580));
        }

        [Test]
        [TestCase(2, 790)]
        [TestCase(3, 1580)]
        [TestCase(4, 0)]
        public void DescuentoFrecuenteSoloAplicaATarjetasNormales(int tipoId, int montoDebitado)
        {
            var tarjeta = ObtenerTarjeta(tipoId);
            tarjeta.Cargar(2000);
            AgregarViajesPrevios(tarjeta, 79, DateTime.Now);

            if (tipoId == 2)
            {
                tarjeta.fecha_ultimo_cobro = DateTime.Now.AddMinutes(-6);
            }
            else if (tipoId == 4)
            {
                tarjeta.fecha_ultimo_cobro = DateTime.Now;
            }

            var saldoInicial = tarjeta.Saldo;
            var boleto = new Boleto(tarjeta, _colectivo);

            Assert.That(saldoInicial - tarjeta.Saldo, Is.EqualTo(montoDebitado));
            Assert.That(boleto.Return_Saldo_Restante(), Is.EqualTo(saldoInicial - montoDebitado));
        }

        private Tarjeta ObtenerTarjeta(int tipoId)
        {
            return _tarjetas.Single(tarjeta => tarjeta.tarjeta_tipo == tipoId);
        }

        private void AgregarViajesPrevios(Tarjeta tarjeta, int cantidad, DateTime fecha)
        {
            var fechaBoleto = fecha.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            var boletos = Enumerable.Range(0, cantidad)
                .Select(_ => new Boleto
                {
                    Tarjeta = tarjeta,
                    Colectivo = _colectivo,
                    Fecha = fechaBoleto,
                    Monto = 1580,
                    SaldoRestante = tarjeta.Saldo
                });

            _db.Boletos.AddRange(boletos);
            _db.SaveChanges();
        }
    }
}
