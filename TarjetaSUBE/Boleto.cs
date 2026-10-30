using System;

namespace TarjetaSUBE
{
    public class Boleto
    {
        public int IdBoleto { get; set; }
        public Tarjeta Tarjeta { get; set; }
        public int Monto;
        public string Fecha { get; set; }
        public Colectivo Colectivo { get; set; }
        public int SaldoRestante { get; set; }

        public Boleto() { }

        public Boleto(Tarjeta tarjeta, Colectivo colectivo)
        {
            Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Tarjeta = tarjeta;
            Colectivo = colectivo;
            Monto = 1580;
            tarjeta.Cobrar(this.Monto);
            SaldoRestante = tarjeta.Saldo;
            Contexto.Db.Boletos.Add(this);
            Contexto.Db.SaveChanges();
        }
        public string Return_Fecha()
        {
            return this.Fecha;
        }

        public string Return_Tipo_Tarjeta()
        {
            return this.Tarjeta.Tipo.nombre_Tipo;
        }

        public string Return_Linea()
        {
            return this.Colectivo.Linea;
        }

        public int Return_Total_Abonado()
        {
            return this.Monto;
        }

        public int Return_Saldo_Restante()
        {
            return this.SaldoRestante;
        }

        public int Return_ID_Tarjeta()
        {
            return this.Tarjeta.IdTarjeta;
        }
    }
}

