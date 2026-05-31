using System;

// Calcula tamano total de la carpeta local de Roblox.
public class GetRobloxFolderSize
{
    public static long GetBytes()
    {
        try
        {
            return RobloxSystemUtilities.CalculateDirectorySizeInBytes(RobloxPaths.GetBaseLocal());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to calculate Roblox directory size in bytes.", ex);
        }
    }

    public static double GetMegabytes()
    {
        try
        {
            return GetBytes() / 1024d / 1024d;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to calculate Roblox directory size in megabytes.", ex);
        }
    }
}
