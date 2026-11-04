using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TarjetaSUBE;

namespace BicicletaTests
{
    public class BicicletaTests
    {
        private TarjetaSUBEContext _db = null!;
        private TarjetaSUBEContext _contextoAnterior = null!;
        private Tarjeta_tipo[] _tipos = null!;
        private Tarjeta _tarjeta = null!;
        private Bicicleta _bicicleta = null!;
        private Estacion_Bici _estacion = null!;

        [SetUp]
        public void Setup()
        {
            var opciones = new DbContextOptionsBuilder<TarjetaSUBEContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _db = new TarjetaSUBEContext(opciones);
            _contextoAnterior = Contexto.Db;
            Contexto.Db = _db;

            _tipos =
            [
                new Tarjeta_tipo { id_Tipo = 1, nombre_Tipo = "Tarifa general", porcentaje_pago = 100 },
                new Tarjeta_tipo { id_Tipo = 2, nombre_Tipo = "Medio boleto estudiantil", porcentaje_pago = 50 },
                new Tarjeta_tipo { id_Tipo = 3, nombre_Tipo = "Boleto gratuito estudiantil", porcentaje_pago = 0 },
                new Tarjeta_tipo { id_Tipo = 4, nombre_Tipo = "Franquicia completa", porcentaje_pago = 0 }
            ];
            _db.Tarjeta_tipos.AddRange(_tipos);
            _db.SaveChanges();

            _tarjeta = CrearTarjetaConSaldo(8000m);
            _bicicleta = new Bicicleta(1);
            _estacion = new Estacion_Bici(2);
            _estacion.bicis[0] = _bicicleta;
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
            Contexto.Db = _contextoAnterior;
        }

        [Test]
        public void PagaLaTarifaDiariaYRetiraLaBicicletaConSaldoSuficiente()
        {
            _bicicleta.estacionada = true;
            _bicicleta.ultimo_uso = DateTime.Now.AddDays(-1);

            _bicicleta.Cobrar(_tarjeta);
            _estacion.retirar_bici(0);

            Assert.That(_tarjeta.Saldo, Is.EqualTo(5581.25m));
            Assert.That(_bicicleta.estacionada, Is.False);
            Assert.That(_estacion.bicis[0], Is.Null);
        }

        [Test]
        public void NoAplicaMultaCuandoElUsoNoSuperaUnaHora()
        {
            var saldoInicial = _tarjeta.Saldo;
            _bicicleta.ultimo_uso = DateTime.Now.AddMinutes(-30);

            _bicicleta.Cobrar(_tarjeta);

            Assert.That(_tarjeta.Saldo, Is.EqualTo(saldoInicial));
        }

        [Test]
        public void RechazaElRetiroSiNoHaySaldoNiMultaPendiente()
        {
            var tarjetaSinSaldo = CrearTarjetaConSaldo(0m);
            _bicicleta.estacionada = true;
            _bicicleta.ultimo_uso = DateTime.Now.AddDays(-1);

            Assert.That(() => _bicicleta.Cobrar(tarjetaSinSaldo), Throws.Exception);

            Assert.That(tarjetaSinSaldo.Saldo, Is.Zero);
            Assert.That(_estacion.bicis[0], Is.SameAs(_bicicleta));
        }

        [Test]
        public void RechazaElRetiroSiElSaldoNoCubreLaMultaPendiente()
        {
            var tarjetaConSaldoInsuficiente = CrearTarjetaConSaldo(2000m);
            _bicicleta.ultimo_uso = DateTime.Now.AddHours(-4);

            Assert.That(() => _bicicleta.Cobrar(tarjetaConSaldoInsuficiente), Throws.Exception);

            Assert.That(tarjetaConSaldoInsuficiente.Saldo, Is.EqualTo(2000m));
            Assert.That(_estacion.bicis[0], Is.SameAs(_bicicleta));
        }

        [Test]
        public void AcumulaLasMultasDeVariosUsosQueSuperanUnaHora()
        {
            var saldoInicial = _tarjeta.Saldo;

            _bicicleta.ultimo_uso = DateTime.Now.AddHours(-2);
            _bicicleta.Cobrar(_tarjeta);

            _bicicleta.estacionada = false;
            _bicicleta.ultimo_uso = DateTime.Now.AddHours(-3);
            _bicicleta.Cobrar(_tarjeta);

            Assert.That(saldoInicial - _tarjeta.Saldo, Is.EqualTo(3750m).Within(0.02m));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void CobraLaTarifaCompletaSinDescuentoPorTipoDeTarjeta(int tipoId)
        {
            var tarjeta = CrearTarjetaConSaldo(8000m, tipoId);
            _bicicleta.estacionada = true;
            _bicicleta.ultimo_uso = DateTime.Now.AddDays(-1);

            _bicicleta.Cobrar(tarjeta);

            Assert.That(8000m - tarjeta.Saldo, Is.EqualTo(2418.75m));
        }

        [Test]
        public void IngresarBicicletaEnPosicionDisponible()
        {
            var estacion = new Estacion_Bici(2);
            var bicicleta = new Bicicleta(2);

            estacion.ingresar_bici(bicicleta, 0);

            Assert.That(estacion.bicis[0], Is.SameAs(bicicleta));
        }

        [Test]
        public void IngresarBicicletaEnPosicionOcupadaFalla()
        {
            Assert.That(
                () => _estacion.ingresar_bici(new Bicicleta(2), 0),
                Throws.Exception);
        }

        [Test]
        public void RetirarBicicletaDePosicionVaciaFalla()
        {
            var estacionVacia = new Estacion_Bici(1);

            Assert.That(() => estacionVacia.retirar_bici(0), Throws.Exception);
        }

        [TestCase(-1)]
        [TestCase(2)]
        public void IngresarBicicletaEnPosicionInvalidaFalla(int posicion)
        {
            var estacion = new Estacion_Bici(2);

            Assert.That(
                () => estacion.ingresar_bici(new Bicicleta(2), posicion),
                Throws.Exception);
        }

        [TestCase(-1)]
        [TestCase(2)]
        public void RetirarBicicletaDePosicionInvalidaFalla(int posicion)
        {
            Assert.That(() => _estacion.retirar_bici(posicion), Throws.Exception);
        }

        private Tarjeta CrearTarjetaConSaldo(decimal saldo, int tipoId = 1)
        {
            var tipo = _tipos.Single(tarjetaTipo => tarjetaTipo.id_Tipo == tipoId);
            var tarjeta = new Tarjeta(0m, tipoId)
            {
                Tipo = tipo
            };

            if (saldo > 0)
            {
                tarjeta.Cargar(saldo);
            }

            return tarjeta;
        }
    }
}
