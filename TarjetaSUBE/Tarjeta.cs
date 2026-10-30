using System;
using System.Linq;

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
            }else if (!new int[] {2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000}.Contains(monto)){
                throw new ArgumentException("El monto debe ser uno de los siguientes: 2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000");
            }
            else
            {
                Excedente += monto;
                this.Acreditar();
                return this;
            }
            
        }

        public Tarjeta Acreditar()
        {
            if (Excedente + Saldo <= LIMITE_SUPERIOR)
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
                this.Acreditar(); 
                return this;
            }
            if (tarjeta_tipo == 2)
            {
                Saldo -= (monto / 2);
                Contexto.Db.SaveChanges();
                this.Acreditar();
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
                this.Acreditar();
                return this;
            }
        }
    }
}


