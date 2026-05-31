using System;
using System.IO;

// Crea respaldo completo de la carpeta local de Roblox.
public class RespaldarCarpetaRoblox
{
    public static void Crear(string rutaDestino)
    {
        if (string.IsNullOrWhiteSpace(rutaDestino))
        {
            throw new ArgumentException("rutaDestino is required.", nameof(rutaDestino));
        }

        try
        {
            string origen = RobloxRutas.BaseLocal();
            if (!Directory.Exists(origen))
            {
                throw new DirectoryNotFoundException($"Roblox directory does not exist: {origen}");
            }

            string destinoCompleto = Path.Combine(rutaDestino, "Roblox_Backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            CopiarDirectorioRecursivo(origen, destinoCompleto);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to create Roblox backup.", ex);
        }
    }

    private static void CopiarDirectorioRecursivo(string origen, string destino)
    {
        Directory.CreateDirectory(destino);

        foreach (string archivo in Directory.GetFiles(origen))
        {
            string destinoArchivo = Path.Combine(destino, Path.GetFileName(archivo));
            File.Copy(archivo, destinoArchivo, true);
        }

        foreach (string directorio in Directory.GetDirectories(origen))
        {
            string destinoSubdirectorio = Path.Combine(destino, Path.GetFileName(directorio));
            CopiarDirectorioRecursivo(directorio, destinoSubdirectorio);
        }
    }
}
