using System;

// Elimina la carpeta temporal de Roblox en el directorio Temp del usuario.
public class ClearRobloxTemp
{
    public static void Clear()
    {
        try
        {
            RobloxSystemUtilities.EnsureRobloxClosed();
            RobloxSystemUtilities.DeleteDirectoryRecursively(RobloxPaths.GetTempDirectory());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to delete the Roblox temp directory.", ex);
        }
    }
}
