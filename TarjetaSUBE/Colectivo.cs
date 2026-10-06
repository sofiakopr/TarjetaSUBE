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

        public bool pagarCon(Tarjeta tarjeta)
        {
            if (tarjeta.Saldo >= -500)
            {
                var boleto = new Boleto(tarjeta);
                tarjeta.Cobrar(boleto.Monto);
                Contexto.Db.SaveChanges();
                Console.WriteLine("Pago realizado con éxito");
                return true;
            }
            else
            {
                Console.WriteLine("Saldo insuficiente.");
            }
            return false;
        }

    }
}
