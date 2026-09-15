using System;

namespace TarjetaSUBE
{
    public class Boleto
    {
        public required int IdBoleto;
        public required int id_Tarjeta;
        public required int id_Colectivo;
        public required Tarjeta Tarjeta;
        public required Colectivo Colectivo;
        public int Monto;
        public required string Fecha;
        public Boleto()
        {
            //boleto normal

            //medio boleto

            //boleto gratuito
        }
    }
}

