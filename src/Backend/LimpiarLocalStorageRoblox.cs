using System;

// Elimina la carpeta LocalStorage de Roblox.
public class ClearRobloxLocalStorage
{
    public static void Clear()
    {
        try
        {
            RobloxSystemUtilities.EnsureRobloxClosed();
            RobloxSystemUtilities.DeleteDirectoryRecursively(RobloxPaths.GetLocalStorageDirectory());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to delete the Roblox LocalStorage directory.", ex);
        }
    }
}
