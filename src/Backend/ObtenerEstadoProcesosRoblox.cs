using System.Diagnostics;
using System;

// Consulta estado y cantidad de procesos activos de Roblox.
public class GetRobloxProcessStatus
{
    public static bool IsRunning()
    {
        try
        {
            return RobloxSystemUtilities.HasRunningRobloxProcess();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to get Roblox process state.", ex);
        }
    }

    public static int GetPlayerProcessCount()
    {
        try
        {
            return Process.GetProcessesByName("RobloxPlayerBeta").Length;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to get Roblox Player process count.", ex);
        }
    }

    public static int GetStudioProcessCount()
    {
        try
        {
            return Process.GetProcessesByName("RobloxStudioBeta").Length;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to get Roblox Studio process count.", ex);
        }
    }
}
