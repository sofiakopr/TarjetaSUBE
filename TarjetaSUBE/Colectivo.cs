using System;
using System.Security.Cryptography.X509Certificates;

namespace TarjetaSUBE
{
    public class Colectivo
    {
        public int IdColectivo;
        public required string Linea;
        public required string Matricula;
        
        public Colectivo() { }

        public bool pagarCon(Tarjeta tarjeta)
        {
            return tarjeta.Cobrar(2150, this);
        }

    }
}
