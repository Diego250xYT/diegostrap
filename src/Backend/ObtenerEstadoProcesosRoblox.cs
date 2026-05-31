using System.Diagnostics;
using System;

// Consulta estado y cantidad de procesos activos de Roblox.
public class ObtenerEstadoProcesosRoblox
{
    public static bool EstaAbierto()
    {
        try
        {
            return RobloxUtilidadesSistema.HayProcesoRobloxActivo();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to get Roblox process state.", ex);
        }
    }

    public static int CantidadProcesosJugador()
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

    public static int CantidadProcesosStudio()
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
