using System;

// Calcula tamano total de la carpeta local de Roblox.
public class ObtenerTamanoCarpetaRoblox
{
    public static long EnBytes()
    {
        try
        {
            return RobloxUtilidadesSistema.CalcularTamanoDirectorioEnBytes(RobloxRutas.BaseLocal());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to calculate Roblox directory size in bytes.", ex);
        }
    }

    public static double EnMegabytes()
    {
        try
        {
            return EnBytes() / 1024d / 1024d;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to calculate Roblox directory size in megabytes.", ex);
        }
    }
}
