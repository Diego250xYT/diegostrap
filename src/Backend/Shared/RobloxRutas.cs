using System;
using System.IO;

// Centraliza rutas locales de Roblox en AppData y Temp.
public static class RobloxPaths
{
    // Devuelve la carpeta principal de Roblox en LocalAppData.
    public static string GetBaseLocal()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox");
    }

    // Devuelve la carpeta de logs de Roblox.
    public static string GetLogsDirectory()
    {
        return Path.Combine(GetBaseLocal(), "logs");
    }

    // Devuelve la carpeta Versions de Roblox.
    public static string GetVersionsDirectory()
    {
        return Path.Combine(GetBaseLocal(), "Versions");
    }

    // Devuelve la carpeta LocalStorage de Roblox.
    public static string GetLocalStorageDirectory()
    {
        return Path.Combine(GetBaseLocal(), "LocalStorage");
    }

    // Devuelve la carpeta http cache de Roblox.
    public static string GetHttpCacheDirectory()
    {
        return Path.Combine(GetBaseLocal(), "http");
    }

    // Devuelve la carpeta Downloads de Roblox.
    public static string GetDownloadsDirectory()
    {
        return Path.Combine(GetBaseLocal(), "Downloads");
    }

    // Devuelve la carpeta de crashes de Roblox.
    public static string GetCrashesDirectory()
    {
        return Path.Combine(GetBaseLocal(), "crashes");
    }

    // Devuelve la carpeta temporal de Roblox.
    public static string GetTempDirectory()
    {
        return Path.Combine(Path.GetTempPath(), "Roblox");
    }

    // Devuelve el archivo de configuracion global de Roblox.
    public static string GetGlobalSettingsFilePath()
    {
        return Path.Combine(GetBaseLocal(), "GlobalBasicSettings_13.xml");
    }
}
