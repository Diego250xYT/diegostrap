// Contenedor con metodo para limpiar en segundo plano los logs de Roblox
using System;
using System.Diagnostics;
using System.IO;

public class LimpiarLogs
{
    public static void Limpiar()
    {
        // Construye la ruta local donde Roblox guarda sus logs.
        string logsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox", "logs");

        // Lanza una excepción si la carpeta no existe para evitar continuar con una ruta invalida.
        if (!Directory.Exists(logsPath))
        {
            throw new DirectoryNotFoundException($"Roblox logs directory does not exist: {logsPath}");
        }

        // No permite borrar los logs mientras el juego sigue ejecutandose.
        if (Process.GetProcessesByName("RobloxPlayerBeta").Length > 0)
        {
            throw new InvalidOperationException("Roblox is running. Close the game before cleaning the logs.");
        }

        // Solo intenta eliminar archivos si realmente hay logs dentro del directorio.
        if (Directory.GetFiles(logsPath).Length > 0)
        {
            try
            {
                // Recorre todos los archivos del directorio y los elimina uno por uno.
                DirectoryInfo di = new DirectoryInfo(logsPath);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            catch (Exception ex)
            {
                // Muestra el motivo exacto si falla la limpieza de archivos.
                Console.WriteLine("Error cleaning logs: " + ex.Message);
            }
        }
    }
}
