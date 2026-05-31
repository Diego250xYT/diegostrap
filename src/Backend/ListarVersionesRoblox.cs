using System;
using System.Collections.Generic;
using System.IO;

// Lista las versiones instaladas de Roblox dentro de la carpeta Versions.
public class ListarVersionesRoblox
{
    public static IReadOnlyList<string> ObtenerTodas()
    {
        try
        {
            string versionsPath = RobloxRutas.Versions();
            if (!Directory.Exists(versionsPath))
            {
                throw new DirectoryNotFoundException($"Versions directory does not exist: {versionsPath}");
            }

            List<string> versiones = new List<string>();
            foreach (string dir in Directory.GetDirectories(versionsPath))
            {
                versiones.Add(Path.GetFileName(dir));
            }

            return versiones;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to list Roblox versions.", ex);
        }
    }
}
