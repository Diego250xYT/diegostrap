using System;
using System.IO;

// Crea respaldo completo de la carpeta local de Roblox.
public class BackupRobloxFolder
{
    public static void CreateBackup(string destinationPath)
    {
        if (string.IsNullOrWhiteSpace(destinationPath))
        {
            throw new ArgumentException("destinationPath is required.", nameof(destinationPath));
        }

        try
        {
            string origen = RobloxPaths.GetBaseLocal();
            if (!Directory.Exists(origen))
            {
                throw new DirectoryNotFoundException($"Roblox directory does not exist: {origen}");
            }

            string destinationFullPath = Path.Combine(destinationPath, "Roblox_Backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            CopyDirectoryRecursively(origen, destinationFullPath);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to create Roblox backup.", ex);
        }
    }

    private static void CopyDirectoryRecursively(string sourcePath, string destinationPath)
    {
        Directory.CreateDirectory(destinationPath);

        foreach (string archivo in Directory.GetFiles(sourcePath))
        {
            string destinationFilePath = Path.Combine(destinationPath, Path.GetFileName(archivo));
            File.Copy(archivo, destinationFilePath, true);
        }

        foreach (string directorio in Directory.GetDirectories(sourcePath))
        {
            string destinationDirectoryPath = Path.Combine(destinationPath, Path.GetFileName(directorio));
            CopyDirectoryRecursively(directorio, destinationDirectoryPath);
        }
    }
}
