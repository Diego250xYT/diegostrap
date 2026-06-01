using System;
using System.IO;

namespace DiegoStrap
{
    internal static class RobloxLocator
    {
        public static string FindRobloxPlayerBetaExecutable()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string versionsRoot = Path.Combine(localAppData, "Roblox", "Versions");

            if (!Directory.Exists(versionsRoot))
            {
                return string.Empty;
            }

            string[] versionFolders = Directory.GetDirectories(versionsRoot);
            Array.Sort(versionFolders, CompareByWriteTimeDescending);

            for (int i = 0; i < versionFolders.Length; i++)
            {
                string candidate = Path.Combine(versionFolders[i], "RobloxPlayerBeta.exe");
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            return string.Empty;
        }

        public static string FindPortableExecutable()
        {
            string candidate = Path.Combine(AppContext.BaseDirectory, "RobloxPlayerBeta.exe");
            return File.Exists(candidate) ? candidate : string.Empty;
        }

        public static string ResolveExecutable()
        {
            string portable = FindPortableExecutable();
            if (!string.IsNullOrWhiteSpace(portable))
            {
                return portable;
            }

            return FindRobloxPlayerBetaExecutable();
        }

        private static int CompareByWriteTimeDescending(string left, string right)
        {
            DateTime leftTime = Directory.GetLastWriteTimeUtc(left);
            DateTime rightTime = Directory.GetLastWriteTimeUtc(right);
            return rightTime.CompareTo(leftTime);
        }
    }
}