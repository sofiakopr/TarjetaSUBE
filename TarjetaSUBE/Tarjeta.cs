using System;

namespace TarjetaSUBE
{
    public class Tarjeta
    {

        public int Saldo; //límite 40k
        public int IdTarjeta;
        public int tarjeta_tipo;

        public Tarjeta() { }

        public Tarjeta(int saldo)
        {
            if (saldo > 40000)
            {
                throw new ArgumentException("El saldo no puede superar los 40.000");
            }
            else if (saldo < -2000)
            {
                throw new ArgumentException("El saldo no puede ser menor a -2000");
            }
            else
            {
                Saldo = saldo;
                Contexto.Db.Tarjetas.Add(this);
                Contexto.Db.SaveChanges();
            }
        }
        public Tarjeta Cobrar (int monto)
        {
            if (tarjeta_tipo == 3 || tarjeta_tipo == 4)
            {
                // no se resta nada
                Contexto.Db.SaveChanges();
                return this;
            }
            if (tarjeta_tipo == 2)
            {
                Saldo -= (monto / 2);
                Contexto.Db.SaveChanges();
                return this;
            }
            if (Saldo - monto < -2000)
            {
                throw new ArgumentException("El saldo no puede ser menor a -2000");
            }
            else
            {
                Saldo -= monto;
                Contexto.Db.SaveChanges();
                return this;
            }
        }
    }
}


