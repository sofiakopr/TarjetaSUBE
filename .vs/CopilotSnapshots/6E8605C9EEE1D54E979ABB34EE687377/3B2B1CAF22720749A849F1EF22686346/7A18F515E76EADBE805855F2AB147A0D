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
                if (ultimo_uso.Date != DateTime.Now.Date)
                {
                    tarjeta.Cobrar_Bici(2418.75m);
                }
                ultimo_uso = DateTime.Now;
                estacionada = false;
            }

            if (!estacionada)
            {
                TimeSpan diferencia = DateTime.Now - ultimo_uso;
                if (diferencia.TotalHours != 0)
                {
                    decimal total = (decimal)diferencia.TotalHours * 1250m;
                    tarjeta.Cobrar_Bici(total);
                    ultimo_uso = DateTime.Now;
                    Console.WriteLine("Se cobraron " + total + " pesos de multa por el uso excesivo de la bicicleta retirada.");
                }
                else
                {
                    ultimo_uso = DateTime.Now;
                }
                estacionada = true;
            }
            return this;
        }

    }
}
