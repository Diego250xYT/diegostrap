using System;
using System.IO;

// Centraliza rutas locales de Roblox en AppData y Temp.
public static class RobloxRutas
{
    public static string BaseLocal()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox");
    }

    public static string Logs()
    {
        return Path.Combine(BaseLocal(), "logs");
    }

    public static string Versions()
    {
        return Path.Combine(BaseLocal(), "Versions");
    }

    public static string LocalStorage()
    {
        return Path.Combine(BaseLocal(), "LocalStorage");
    }

    public static string HttpCache()
    {
        return Path.Combine(BaseLocal(), "http");
    }

    public static string Downloads()
    {
        return Path.Combine(BaseLocal(), "Downloads");
    }

    public static string Crashes()
    {
        return Path.Combine(BaseLocal(), "crashes");
    }

    public static string TempRoblox()
    {
        return Path.Combine(Path.GetTempPath(), "Roblox");
    }

    public static string ConfiguracionGlobal()
    {
        return Path.Combine(BaseLocal(), "GlobalBasicSettings_13.xml");
    }
}
