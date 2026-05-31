using System;

// Elimina la carpeta de reportes de fallos de Roblox.
public class LimpiarCrashesRoblox
{
    public static void Limpiar()
    {
        try
        {
            RobloxUtilidadesSistema.ValidarRobloxCerrado();
            RobloxUtilidadesSistema.EliminarDirectorioCompleto(RobloxRutas.Crashes());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to delete the Roblox crashes directory.", ex);
        }
    }
}
