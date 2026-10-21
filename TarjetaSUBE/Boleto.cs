using System;

namespace TarjetaSUBE
{
    public class Boleto
    {
        public int IdBoleto { get; set; }
        public int id_Tarjeta { get; set; }
        public Tarjeta Tarjeta { get; set; }
        public int Monto;
        public string Fecha { get; set; }

        public Boleto() { }

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

