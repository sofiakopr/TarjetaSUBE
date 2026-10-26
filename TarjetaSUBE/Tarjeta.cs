using System;
using System.Reflection.Metadata.Ecma335;

namespace TarjetaSUBE
{
    public class Tarjeta
    {

        public int Saldo; //límite 40k
        public int IdTarjeta;
        public Tarjeta_tipo tipo;
        public DateTime fecha_ultimo_cobro = DateTime.Now;
        public int tarjeta_tipo;

        public Tarjeta() { }

        public Tarjeta(int saldo, int tipo)
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
                tarjeta_tipo = tipo;
                Contexto.Db.Tarjetas.Add(this);
                Contexto.Db.SaveChanges();
            }
        }
        public Tarjeta Cobrar (int monto) {
        
            int cantidad = Contexto.Db.Boletos
            .Where(b => b.IdTarjeta == this.IdTarjeta && b.fecha.Date == DateTime.Now.Date)
            .Count();

            // Boleto Estudiantil Completo
            if (this.tipo.nombre_Tipo ==  "Boleto gratuito estudiantil")
            {
                if(cantidad < 2)
                {
                    // no se resta nada
                    Contexto.Db.SaveChanges();
                    return this;
                }
                else
                {
                    Saldo -= (monto / 2);
                    Contexto.Db.SaveChanges();
                    return this;
                }
            }
            // Boleto Franquicia Completa (Juvilados supongo)
            if (this.tipo.nombre_Tipo == "Franquicia completa")
            {
                //no se resta nada
                Contexto.Db.SaveChanges();
                return this;
            }
            // Boleto Estudiantil Medio
            if (this.tipo.nombre_Tipo == "Medio boleto estudiantil")
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


