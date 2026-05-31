using System.Diagnostics;
using System;

// Cierra procesos de Roblox Player, Studio y CrashHandler.
public class CloseRobloxProcesses
{
    public static int CloseAll()
    {
        try
        {
            int cerrados = 0;
            cerrados += Close("RobloxPlayerBeta");
            cerrados += Close("RobloxStudioBeta");
            cerrados += Close("RobloxCrashHandler");
            return cerrados;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to terminate Roblox processes.", ex);
        }
    }

    private static int Close(string processName)
    {
        int count = 0;
        foreach (Process process in Process.GetProcessesByName(processName))
        {
            process.Kill();
            count++;
        }

        return count;
    }
}
