using Microsoft.EntityFrameworkCore;
using TarjetaSUBE;

namespace LimitacionFranquiciaTests
{
    public class LimitacionFranquicia
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
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        [Test]
        [TestCase(1, 2700)]
        [TestCase(2, 4850)]
        [TestCase(3, 7000)]
        [TestCase(4, 7000)]
        public void CadaTipoDeTarjetaAplicaSuTarifaEnLosPrimerosDosViajes(int tipoId, int saldoEsperado)
        {
            var tarjeta = CrearTarjeta(tipoId);

            PagarViaje(tarjeta, tipoId);
            PagarViaje(tarjeta, tipoId);

            Assert.That(tarjeta.Saldo, Is.EqualTo(saldoEsperado));
        }

        [Test]
        [TestCase(1, -1600)]
        [TestCase(2, 2700)]
        [TestCase(3, 2700)]
        [TestCase(4, 7000)]
        public void CadaTipoDeTarjetaAplicaSuTarifaEnLosViajesPosteriores(int tipoId, int saldoEsperado)
        {
            var tarjeta = CrearTarjeta(tipoId);

            PagarViaje(tarjeta, tipoId);
            PagarViaje(tarjeta, tipoId);
            PagarViaje(tarjeta, tipoId);
            PagarViaje(tarjeta, tipoId);

            Assert.That(tarjeta.Saldo, Is.EqualTo(saldoEsperado));
        }

        private Tarjeta CrearTarjeta(int tipoId)
        {
            return new Tarjeta(7000, tipoId)
            {
                Tipo = _tipos.Single(tipo => tipo.id_Tipo == tipoId)
            };
        }

        private void PagarViaje(Tarjeta tarjeta, int tipoId)
        {
            if (tipoId == 2)
            {
                tarjeta.fecha_ultimo_cobro = DateTime.Now.AddMinutes(-6);
            }
            else if (tipoId == 4)
            {
                tarjeta.fecha_ultimo_cobro = DateTime.Now;
            }

            Assert.That(_colectivo.pagarCon(tarjeta), Is.True);
        }
    }
}