using System;

// Elimina la cache HTTP local de Roblox.
public class ClearRobloxHttpCache
{
    public static void Clear()
    {
        try
        {
            RobloxSystemUtilities.EnsureRobloxClosed();
            RobloxSystemUtilities.DeleteDirectoryRecursively(RobloxPaths.GetHttpCacheDirectory());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to delete the Roblox HTTP cache directory.", ex);
        }
    }
}
