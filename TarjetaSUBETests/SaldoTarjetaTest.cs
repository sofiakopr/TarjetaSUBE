using Microsoft.EntityFrameworkCore;
using TarjetaSUBE;

namespace SaldoTarjeta
{
    public class SaldoTarjetaTests
    {
        private TarjetaSUBEContext _db = null!;
        private Tarjeta_tipo[] _tipos = null!;

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
            _db.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void CargarPorEncimaDelLimite_AcreditaHastaElMaximoYDejaExcedentePendiente(int tipoId)
        {
            var tarjeta = CrearTarjeta(tipoId);
            tarjeta.Cargar(10000);

            Assert.That(tarjeta.Saldo, Is.EqualTo(56000));
            Assert.That(tarjeta.Excedente, Is.EqualTo(4000));
        }

        [Test]
        [TestCase(1, 53850)]
        [TestCase(2, 54925)]
        [TestCase(3, 56000)]
        [TestCase(4, 56000)]
        public void CobrarConExcedentePendiente_AplicaLaReglaDelTipoDeTarjeta(int tipoId, int saldoEsperado)
        {
            var tarjeta = CrearTarjeta(tipoId);
            tarjeta.Cargar(10000);
            if (tipoId == 2)
            {
                tarjeta.fecha_ultimo_cobro = DateTime.Now.AddMinutes(-6);
            }
            else if (tipoId == 4)
            {
                tarjeta.fecha_ultimo_cobro = DateTime.Now;
            }
            tarjeta.Cobrar(2150);

            Assert.That(tarjeta.Saldo, Is.EqualTo(saldoEsperado));
            Assert.That(tarjeta.Excedente, Is.EqualTo(4000));
        }

        [TestCase(1, -2000)]
        [TestCase(1, 1000)]
        [TestCase(2, -2000)]
        [TestCase(2, 1000)]
        [TestCase(3, -2000)]
        [TestCase(3, 1000)]
        [TestCase(4, -2000)]
        [TestCase(4, 1000)]
        public void CargarConMontoInvalido_LanzaExcepcionSinModificarSaldo(int tipoId, int monto)
        {
            var tarjeta = CrearTarjeta(tipoId);
            Assert.Throws<ArgumentException>(() => tarjeta.Cargar(monto));

            Assert.That(tarjeta.Saldo, Is.EqualTo(50000));
            Assert.That(tarjeta.Excedente, Is.Zero);
        }

        private Tarjeta CrearTarjeta(int tipoId)
        {
            return new Tarjeta(50000, tipoId)
            {
                Tipo = _tipos.Single(tipo => tipo.id_Tipo == tipoId)
            };
        }
    }
}