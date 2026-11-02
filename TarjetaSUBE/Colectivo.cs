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

        public bool pagarCon(Tarjeta tarjeta)
        {
            if (interurbano == 0)
            {
                var boleto = new Boleto(tarjeta, this, PRECIO_URBANO);
                return true;
            }
            else
            {
                var boleto = new Boleto(tarjeta, this, PRECIO_INTERURBANO);
                return true;
            }
            return false;
        }

    }
}
