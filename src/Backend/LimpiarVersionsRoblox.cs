using System;

// Elimina la carpeta Versions de Roblox.
public class LimpiarVersionsRoblox
{
    public static void Limpiar()
    {
        try
        {
            RobloxUtilidadesSistema.ValidarRobloxCerrado();
            RobloxUtilidadesSistema.EliminarDirectorioCompleto(RobloxRutas.Versions());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to delete the Roblox Versions directory.", ex);
        }
    }
}
