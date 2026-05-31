using System;

// Elimina la carpeta temporal de Roblox en el directorio Temp del usuario.
public class LimpiarTempRoblox
{
    public static void Limpiar()
    {
        try
        {
            RobloxUtilidadesSistema.ValidarRobloxCerrado();
            RobloxUtilidadesSistema.EliminarDirectorioCompleto(RobloxRutas.TempRoblox());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to delete the Roblox temp directory.", ex);
        }
    }
}
