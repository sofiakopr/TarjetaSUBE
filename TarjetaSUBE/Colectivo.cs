using System;
using System.Security.Cryptography.X509Certificates;

namespace TarjetaSUBE
{
    public class Colectivo
    {
        private const int PRECIO_URBANO = 2150;
        private const int PRECIO_INTERURBANO = 3000;
        public int IdColectivo;
        public required string Linea;
        public required string Matricula;
        public int interurbano; // 0 = urbano, 1 = interurbano

        public Colectivo() { }

        public Tarjeta pagarCon(Tarjeta tarjeta)
        {
            return tarjeta.Cobrar(2150, this);
        }

    }
}
