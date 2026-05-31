using System;

// Elimina la carpeta de descargas de Roblox.
public class LimpiarDownloadsRoblox
{
    public static void Limpiar()
    {
        try
        {
            RobloxUtilidadesSistema.ValidarRobloxCerrado();
            RobloxUtilidadesSistema.EliminarDirectorioCompleto(RobloxRutas.Downloads());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to delete the Roblox Downloads directory.", ex);
        }
    }
}
