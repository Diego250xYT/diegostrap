using System;

// Borra contenido interno de la carpeta logs sin borrar la carpeta.
public class EmptyRobloxLogs
{
    public static void Clear()
    {
        try
        {
            RobloxSystemUtilities.EnsureRobloxClosed();
            RobloxSystemUtilities.ClearDirectoryContents(RobloxPaths.GetLogsDirectory());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to clean Roblox logs content.", ex);
        }
    }
}
