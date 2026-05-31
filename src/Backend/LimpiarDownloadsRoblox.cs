using System;

// Elimina la carpeta de descargas de Roblox.
public class ClearRobloxDownloads
{
    public static void Clear()
    {
        try
        {
            RobloxSystemUtilities.EnsureRobloxClosed();
            RobloxSystemUtilities.DeleteDirectoryRecursively(RobloxPaths.GetDownloadsDirectory());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to delete the Roblox Downloads directory.", ex);
        }
    }
}
