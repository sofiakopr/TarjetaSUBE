using System;

namespace TarjetaSUBE
{
    public class Estacion_Bici
    {
        public int id;
        public int limite;
        public Bicicleta[] bicis;

        public Estacion_Bici(int limite)
        {
            this.limite = limite;
            this.bicis = new Bicicleta[limite];
        }

        public Estacion_Bici ingresar_bici(Bicicleta bici, int posicion)
        {
            int bicisIngresadas = bicis.Count();

            if (bicisIngresadas < limite)
            {
                if (posicion < 0 || posicion >= limite)
                {
                    throw new Exception("Posición inválida");
                }
                if (bicis[posicion] != null)
                {
                    throw new Exception("La posición ya está ocupada");
                }
                bicis[posicion] = bici;
                return this;
            }
            throw new Exception("No hay espacio en la estación");
        }
        public Estacion_Bici retirar_bici(int posicion)
        {
            if (posicion < 0 || posicion >= limite)
            {
                throw new Exception("Posición inválida");
            }
            if (bicis[posicion] == null)
            {
                throw new Exception("No hay bicicleta en esa posición");
            }
            bicis[posicion] = null;
            return this;
        }


    }
}
