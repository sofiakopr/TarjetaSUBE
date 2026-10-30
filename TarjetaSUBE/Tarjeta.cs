using System;

namespace TarjetaSUBE
{
    
    public class Tarjeta
    {
        private const int LIMITE_SUPERIOR = 56000;
        public int Saldo; //límite 40k
        public int IdTarjeta;
        public int tarjeta_tipo;
        public int Excedente;

        public Tarjeta() { }

        public Tarjeta(int saldo)
        {
            if (saldo > LIMITE_SUPERIOR)
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
                Excedente = 0;
                Contexto.Db.Tarjetas.Add(this);
                Contexto.Db.SaveChanges();
            }
        }

        public Tarjeta Cargar (int monto)
        {
            if (monto < 0)
            {
                throw new ArgumentException("El monto no puede ser negativo");
            }
            else
            {
                Excedente += monto;
                return this;
                this.Acreditar();
            }
            
        }

        public Tarjeta Acreditar()
        {
            if (Excedente + Saldo < LIMITE_SUPERIOR)
            {
                Saldo += Excedente;
                Excedente = 0;
                Contexto.Db.SaveChanges();
            }
            else
            {
                Excedente = (Excedente + Saldo) - LIMITE_SUPERIOR;
                Saldo = LIMITE_SUPERIOR;
                Contexto.Db.SaveChanges();
            }
            return this;
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
            this.Acreditar();
        }
    }
}


