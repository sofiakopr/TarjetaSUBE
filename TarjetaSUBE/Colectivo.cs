using System;
using System.Security.Cryptography.X509Certificates;

namespace TarjetaSUBE
{
    public class Colectivo
    {
        public int IdColectivo;
        public required string Linea;
        public required string Matricula;
        
        public Colectivo() { }

        public void pagarCon(Tarjeta tarjeta)
        {
            if (tarjeta.Saldo >= 1580)
            {
                var boleto = new Boleto{ IdBoleto = 0, id_Tarjeta = tarjeta.IdTarjeta, Tarjeta = tarjeta, Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
                boleto.Crear(tarjeta);
                Console.WriteLine("Pago realizado con éxito");
            }
            else
            {
                Console.WriteLine("Saldo insuficiente.");
            }
        }

    }
}
