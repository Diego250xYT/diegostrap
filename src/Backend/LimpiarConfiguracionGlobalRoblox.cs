using System;

// Elimina el archivo de configuracion global de Roblox si existe.
public class ClearRobloxGlobalConfiguration
{
    public static void Clear()
    {
        try
        {
            RobloxSystemUtilities.EnsureRobloxClosed();
            RobloxSystemUtilities.DeleteFileIfExists(RobloxPaths.GetGlobalSettingsFilePath());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to delete Roblox global settings file.", ex);
        }
    }
}
