using System;

namespace TarjetaSUBE;

public static class Contexto
{
    public static TarjetaSUBEContext Db { get; set; } = new TarjetaSUBEContext();
    public static Func<DateTime> Reloj { get; set; } = () => DateTime.Now;
}
