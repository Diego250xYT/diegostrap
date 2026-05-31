using System.Diagnostics;
using System;

// Cierra procesos de Roblox Player, Studio y CrashHandler.
public class CerrarProcesosRoblox
{
    public static int CerrarTodos()
    {
        try
        {
            int cerrados = 0;
            cerrados += Cerrar("RobloxPlayerBeta");
            cerrados += Cerrar("RobloxStudioBeta");
            cerrados += Cerrar("RobloxCrashHandler");
            return cerrados;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to terminate Roblox processes.", ex);
        }
    }

    private static int Cerrar(string processName)
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
