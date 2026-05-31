using System;
using System.Diagnostics;
using System.IO;

// Reune utilidades de procesos y sistema de archivos para Roblox.
public static class RobloxSystemUtilities
{
    private static readonly string[] RobloxProcesses = new string[]
    {
        "RobloxPlayerBeta",
        "RobloxStudioBeta",
        "RobloxCrashHandler"
    };

    // Indica si hay algun proceso conocido de Roblox en ejecucion.
    public static bool HasRunningRobloxProcess()
    {
        try
        {
            int index = 0;

            while (index < RobloxProcesses.Length)
            {
                if (Process.GetProcessesByName(RobloxProcesses[index]).Length > 0)
                {
                    return true;
                }

                index++;
            }

            return false;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to inspect Roblox process state.", ex);
        }
    }

    // Lanza error si Roblox sigue abierto.
    public static void EnsureRobloxClosed()
    {
        if (HasRunningRobloxProcess())
        {
            throw new InvalidOperationException("Roblox is running. Close Roblox before this operation.");
        }
    }

    // Borra todo el contenido de una carpeta sin borrar la carpeta base.
    public static void ClearDirectoryContents(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException("Directory does not exist: " + directoryPath);
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
            throw new InvalidOperationException("Failed to clean directory content: " + directoryPath, ex);
        }
    }

    // Borra una carpeta completa de forma recursiva.
    public static void DeleteDirectoryRecursively(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException("Directory does not exist: " + directoryPath);
        }

        try
        {
            Directory.Delete(directoryPath, true);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to delete directory: " + directoryPath, ex);
        }
    }

    // Borra un archivo solo si existe.
    public static void DeleteFileIfExists(string filePath)
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
            throw new InvalidOperationException("Failed to delete file: " + filePath, ex);
        }
    }

    // Calcula el tamano total de una carpeta en bytes.
    public static long CalculateDirectorySizeInBytes(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException("Directory does not exist: " + directoryPath);
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
            throw new InvalidOperationException("Failed to calculate directory size: " + directoryPath, ex);
        }
    }
}
