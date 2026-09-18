using System;

namespace TarjetaSUBE
{
    public class Boleto
    {
        public required int IdBoleto { get; set; }
        public required int id_Tarjeta { get; set; }
        //public required int id_Colectivo { get; set; } , id_Colectivo = colectivo.IdColectivo
        public required Tarjeta Tarjeta { get; set; }
        //public required Colectivo Colectivo { get; set; } , Colectivo = colectivo
        public int Monto = 1580;
        public required string Fecha { get; set; }
        public Boleto()
        {
            
            //boleto normal

            //medio boleto

            //boleto gratuito
        }

        public Boleto Crear(Tarjeta tarjeta)
        {
            var boleto = new Boleto { IdBoleto = 1, id_Tarjeta = tarjeta.IdTarjeta, Tarjeta = tarjeta, Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
            Contexto.Db.Boletos.Add(boleto);
            Contexto.Db.SaveChanges();
            return boleto;
        }
    }
}

