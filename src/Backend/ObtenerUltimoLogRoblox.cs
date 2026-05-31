using System;
using System.IO;
using System.Linq;

// Obtiene la ruta del log mas reciente de Roblox.
public class ObtenerUltimoLogRoblox
{
    public static string RutaUltimoLog()
    {
        try
        {
            string logsPath = RobloxRutas.Logs();
            if (!Directory.Exists(logsPath))
            {
                throw new DirectoryNotFoundException($"Logs directory does not exist: {logsPath}");
            }

            string[] archivos = Directory.GetFiles(logsPath);
            if (archivos.Length == 0)
            {
                return string.Empty;
            }

            return archivos
                .Select(path => new FileInfo(path))
                .OrderByDescending(file => file.LastWriteTimeUtc)
                .First()
                .FullName;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to retrieve latest Roblox log file.", ex);
        }
    }
}
