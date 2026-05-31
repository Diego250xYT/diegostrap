// Contenedor con metodo para limpiar en segundo plano los logs de Roblox
using System;
using System.Diagnostics;
using System.IO;

public class LimpiarLogs
{
    public static void Limpiar()
    {
        // Ruta de los logs de Roblox
        string logsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox", "logs");

        if (!Directory.Exists(logsPath))
        {
            throw new DirectoryNotFoundException($"Roblox logs directory does not exist: {logsPath}");
        }

        if (Process.GetProcessesByName("RobloxPlayerBeta").Length > 0)
        {
            throw new InvalidOperationException("Roblox is running. Close the game before cleaning the logs.");
        }

        if (Directory.GetFiles(logsPath).Length > 0) // Verificar si la carpeta de logs existe y tiene archivos antes de intentar limpiarla
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(logsPath);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error cleaning logs: " + ex.Message);
            }
        }
    }
}
