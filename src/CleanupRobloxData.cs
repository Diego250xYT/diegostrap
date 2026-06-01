using System;
using System.IO;

namespace DiegoStrap
{
    internal static class CleanupRobloxData
    {
        public static bool DeleteLogs()
        {
            string robloxRoot = GetRobloxRoot();
            return DeleteFolderIfExists(Path.Combine(robloxRoot, "Logs"));
        }

        public static bool DeleteLocalStorage()
        {
            string robloxRoot = GetRobloxRoot();
            return DeleteFolderIfExists(Path.Combine(robloxRoot, "LocalStorage"));
        }

        public static bool DeleteRobloxFolder()
        {
            return DeleteFolderIfExists(GetRobloxRoot());
        }

        private static string GetRobloxRoot()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(localAppData, "Roblox");
        }

        private static bool DeleteFolderIfExists(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }

            Directory.Delete(path, recursive: true);
            return true;
        }
    }
}