using System;

// Elimina la carpeta Versions de Roblox.
public class ClearRobloxVersions
{
    // Ejecuta la limpieza de la carpeta Versions de Roblox.
    public static void Clear()
    {
        try
        {
            RobloxSystemUtilities.EnsureRobloxClosed();
            RobloxSystemUtilities.DeleteDirectoryRecursively(RobloxPaths.GetVersionsDirectory());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to delete the Roblox Versions directory.", ex);
        }
    }
}
