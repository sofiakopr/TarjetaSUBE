using System;

namespace TarjetaSUBE
{
    public class Boleto
    {
        public required int IdBoleto { get; set; }
        public required int id_Tarjeta { get; set; }
        public required Tarjeta Tarjeta { get; set; }
        public int Monto;
        public required string Fecha { get; set; }

        public Boleto (Tarjeta tarjeta)
        {
            id_Tarjeta = tarjeta.IdTarjeta;
            Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Tarjeta = tarjeta;
            Monto = 1500;
            Contexto.Db.Boletos.Add(this);
            Contexto.Db.SaveChanges();
        }
    }
}

