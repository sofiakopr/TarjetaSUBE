using System;

namespace TarjetaSUBE
{
    public class Tarjeta
    {

        public int Saldo; //límite 40k
        public int IdTarjeta;
        public Tarjeta(int saldo)
        {
            if (saldo > 40000)
            {
                throw new ArgumentException("El saldo no puede superar los 40.000");
            }else if (saldo < -2000)
            {
                throw new ArgumentException("El saldo no puede ser negativo");
            }
            else
            {
                Saldo = saldo;
                var tarjeta = new Tarjeta { Saldo = saldo };
                Contexto.Db.Tarjetas.Add(tarjeta);
                Contexto.Db.SaveChanges();
                return tarjeta;

            }
    }
}


