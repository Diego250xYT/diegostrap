using System;

// Elimina por completo la carpeta local principal de Roblox.
public class LimpiarCarpetaRoblox
{
    // Ejecuta la limpieza completa de la carpeta local de Roblox.
    public static void Limpiar()
    {
        try
        {
            RobloxUtilidadesSistema.ValidarRobloxCerrado();
            RobloxUtilidadesSistema.EliminarDirectorioCompleto(RobloxRutas.BaseLocal());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to delete the Roblox local directory.", ex);
        }
    }
}
