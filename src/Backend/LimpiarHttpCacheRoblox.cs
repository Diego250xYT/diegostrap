using System;

// Elimina la cache HTTP local de Roblox.
public class LimpiarHttpCacheRoblox
{
    public static void Limpiar()
    {
        try
        {
            RobloxUtilidadesSistema.ValidarRobloxCerrado();
            RobloxUtilidadesSistema.EliminarDirectorioCompleto(RobloxRutas.HttpCache());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to delete the Roblox HTTP cache directory.", ex);
        }
    }
}
