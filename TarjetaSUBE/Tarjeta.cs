using System;

namespace TarjetaSUBE
{
    public class Tarjeta
    {

        public int Saldo; //límite 40k
        public int IdTarjeta;
        public int tarjeta_tipo;
        public DateTime fecha_ultimo_cobro = DateTime.Now;
        public int CantidadViajes_Dia;
        public Tarjeta_tipo Tipo { get; set; }


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

        public Tarjeta Cobrar(int monto)
        {
            TimeSpan diferencia = DateTime.Now - fecha_ultimo_cobro;
                if (tarjeta_tipo == 3 || tarjeta_tipo == 4)
                {
                    // no se resta nada
                    Contexto.Db.SaveChanges();
                    return this;
                }
                if (tarjeta_tipo == 2)
                {
                    if (diferencia.TotalMinutes < 5)
                    {
                        throw new Exception ("No se puede cobrar el viaje, ya que no han pasado 5 minutos desde el último cobro de medio boleto");
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
}


