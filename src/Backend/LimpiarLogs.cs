using System;

// Mantiene compatibilidad con el modulo legado de limpieza de logs.
public class ClearRobloxLogs
{
    public static void Clear()
    {
        try
        {
            EmptyRobloxLogs.Clear();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to clean Roblox logs.", ex);
        }
    }
}
