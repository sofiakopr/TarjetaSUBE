using System;

namespace TarjetaSUBE
{
    public class Bicicleta
    {
        public int id;
        public bool estacionada; // 0 = no, 1 = si
        public DateTime ultimo_uso = DateTime.Now;
        
        public Bicicleta(int id)
        {
            this.id = id;
            this.estacionada = false;
        }

        public Bicicleta Cobrar(Tarjeta tarjeta)
        {
            if (estacionada)
            {
                if (tarjeta.Saldo < 2418.75m)
                {
                    throw new ArgumentException("Saldo insuficiente para retirar la bicicleta");
                }

                tarjeta.Cobrar_Bici(2418.75m);
                ultimo_uso = DateTime.Now;
                estacionada = false;
                return this;
            }

            TimeSpan diferencia = DateTime.Now - ultimo_uso;
            decimal multa = Math.Max(0, (decimal)diferencia.TotalHours - 1) * 1250m;
            if (tarjeta.Saldo < multa)
            {
                throw new ArgumentException("Saldo insuficiente para pagar la multa de la bicicleta");
            }

            if (multa > 0)
            {
                tarjeta.Cobrar_Bici(multa);
                Console.WriteLine("Se cobraron " + multa + " pesos de multa por el uso excesivo de la bicicleta retirada.");
            }
            ultimo_uso = DateTime.Now;
            estacionada = true;
            return this;
        }

    }
}
