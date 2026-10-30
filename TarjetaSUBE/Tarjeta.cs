using System;
using System.Reflection.Metadata.Ecma335;

namespace TarjetaSUBE
{

    public class Tarjeta
    {
        private const int LIMITE_SUPERIOR = 56000;
        public int Saldo; //límite 40k
        public int IdTarjeta;
        public int tarjeta_tipo;
        public DateTime fecha_ultimo_cobro = DateTime.Now;
        public int CantidadViajes_Dia;
        public Tarjeta_tipo Tipo { get; set; }
        public int Excedente;

        public Tarjeta() { }

        public Tarjeta(int saldo, int tipo)
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
                tarjeta_tipo = tipo;
                Excedente = 0;
                Contexto.Db.Tarjetas.Add(this);
                Contexto.Db.SaveChanges();
            }
        }

        public Tarjeta Cargar(int monto)
        {
            if (monto < 0)
            {
                throw new ArgumentException("El monto no puede ser negativo");
            } else if (!new int[] { 2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000 }.Contains(monto)) {
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

        public Tarjeta Cobrar(int monto)
        {
            TimeSpan diferencia = DateTime.Now - fecha_ultimo_cobro;
            int cantidad = Contexto.Db.Boletos
            .Where(b => b.Tarjeta.IdTarjeta == this.IdTarjeta && b.Fecha.StartsWith(DateTime.Now.ToString("yyyy-MM-dd")))
            .Count();

            //Boleto Estudiantil Gratuito
            if (this.Tipo.nombre_Tipo == "Boleto gratuito estudiantil")
            {
                if (cantidad < 2)
                {
                    // no se resta nada
                    Contexto.Db.SaveChanges();
                    return this;
                }
                else
                {
                    Saldo -= monto;
                    Contexto.Db.SaveChanges();
                    return this;
                }
            }
            // Boleto Franquicia Completa (Jubilados)
            if (this.Tipo.nombre_Tipo == "Franquicia completa")
            {
                if (diferencia.TotalMinutes < 5)
                {
                    //no se resta nada
                    Contexto.Db.SaveChanges();
                    return this;
                }
                else
                {
                    throw new Exception("EL JUBILADO PAGO HACE MENOS DE 5 MINUTOS");
                }
            }

            // Boleto Estudiantil Medio
            if (this.Tipo.nombre_Tipo == "Medio boleto estudiantil")
            {
                if (diferencia.TotalMinutes < 5)
                {
                    throw new Exception("No se puede cobrar el viaje, ya que no han pasado 5 minutos desde el último cobro de medio boleto");
                }
                else
                {
                    Saldo -= (monto / 2);
                    Contexto.Db.SaveChanges();
                    return this;
                }
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


