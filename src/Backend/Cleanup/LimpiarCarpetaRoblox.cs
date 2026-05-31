using System;

// Elimina por completo la carpeta local principal de Roblox.
public class ClearRobloxFolder
{
    // Ejecuta la limpieza completa de la carpeta local de Roblox.
    public static void Clear()
    {
        try
        {
            RobloxSystemUtilities.EnsureRobloxClosed();
            RobloxSystemUtilities.DeleteDirectoryRecursively(RobloxPaths.GetBaseLocal());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to delete the Roblox local directory.", ex);
        }
    }
}
