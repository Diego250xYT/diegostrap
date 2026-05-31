using System;
using System.IO;

// Centraliza rutas locales de Roblox en AppData y Temp.
public static class RobloxRutas
{
    // Devuelve la carpeta principal de Roblox en LocalAppData.
    public static string BaseLocal()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox");
    }

    // Devuelve la carpeta de logs de Roblox.
    public static string Logs()
    {
        return Path.Combine(BaseLocal(), "logs");
    }

    // Devuelve la carpeta Versions de Roblox.
    public static string Versions()
    {
        return Path.Combine(BaseLocal(), "Versions");
    }

    // Devuelve la carpeta LocalStorage de Roblox.
    public static string LocalStorage()
    {
        return Path.Combine(BaseLocal(), "LocalStorage");
    }

    // Devuelve la carpeta http cache de Roblox.
    public static string HttpCache()
    {
        return Path.Combine(BaseLocal(), "http");
    }

    // Devuelve la carpeta Downloads de Roblox.
    public static string Downloads()
    {
        return Path.Combine(BaseLocal(), "Downloads");
    }

    // Devuelve la carpeta de crashes de Roblox.
    public static string Crashes()
    {
        return Path.Combine(BaseLocal(), "crashes");
    }

    // Devuelve la carpeta temporal de Roblox.
    public static string TempRoblox()
    {
        return Path.Combine(Path.GetTempPath(), "Roblox");
    }

    // Devuelve el archivo de configuracion global de Roblox.
    public static string ConfiguracionGlobal()
    {
        return Path.Combine(BaseLocal(), "GlobalBasicSettings_13.xml");
    }
}
