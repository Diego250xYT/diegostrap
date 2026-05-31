using System;

// Elimina la carpeta LocalStorage de Roblox.
public class LimpiarLocalStorageRoblox
{
    public static void Limpiar()
    {
        try
        {
            RobloxUtilidadesSistema.ValidarRobloxCerrado();
            RobloxUtilidadesSistema.EliminarDirectorioCompleto(RobloxRutas.LocalStorage());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to delete the Roblox LocalStorage directory.", ex);
        }
    }
}
