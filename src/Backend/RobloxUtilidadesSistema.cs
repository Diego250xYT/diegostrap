using System;
using System.Diagnostics;
using System.IO;

// Reune utilidades de procesos y sistema de archivos para Roblox.
public static class RobloxUtilidadesSistema
{
    private static readonly string[] ProcesosRoblox =
    {
        "RobloxPlayerBeta",
        "RobloxStudioBeta",
        "RobloxCrashHandler"
    };

    public static bool HayProcesoRobloxActivo()
    {
        try
        {
            foreach (string proceso in ProcesosRoblox)
            {
                if (Process.GetProcessesByName(proceso).Length > 0)
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to inspect Roblox process state.", ex);
        }
    }

    public static void ValidarRobloxCerrado()
    {
        if (HayProcesoRobloxActivo())
        {
            throw new InvalidOperationException("Roblox is running. Close Roblox before this operation.");
        }
    }

    public static void LimpiarContenidoDirectorio(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"Directory does not exist: {directoryPath}");
        }

        try
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);

            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo subDirectory in directoryInfo.GetDirectories())
            {
                subDirectory.Delete(true);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to clean directory content: {directoryPath}", ex);
        }
    }

    public static void EliminarDirectorioCompleto(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"Directory does not exist: {directoryPath}");
        }

        try
        {
            Directory.Delete(directoryPath, true);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to delete directory: {directoryPath}", ex);
        }
    }

    public static void EliminarArchivoSiExiste(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to delete file: {filePath}", ex);
        }
    }

    public static long CalcularTamanoDirectorioEnBytes(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"Directory does not exist: {directoryPath}");
        }

        try
        {
            long total = 0;
            DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);

            foreach (FileInfo file in dirInfo.GetFiles("*", SearchOption.AllDirectories))
            {
                total += file.Length;
            }

            return total;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to calculate directory size: {directoryPath}", ex);
        }
    }
}
