using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace DiegoStrap
{
    internal static class MultiInstanceManager
    {
        private const string SettingsFolderName = "DiegoStrap";
        private const string SettingsFileName = "roblox-player-path.txt";

        public static bool IsRobloxRunning()
        {
            try
            {
                Process[] processes = Process.GetProcessesByName("RobloxPlayerBeta");
                return processes.Length > 0;
            }
            catch
            {
                return false;
            }
        }

        public static string FindRobloxPlayerPath()
        {
            string savedPath = LoadSavedRobloxPlayerPath();
            if (!string.IsNullOrWhiteSpace(savedPath) && File.Exists(savedPath))
            {
                return savedPath;
            }

            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string versionsRoot = Path.Combine(localAppData, "Roblox", "Versions");

            if (!Directory.Exists(versionsRoot))
            {
                return string.Empty;
            }

            string[] versionFolders = Directory.GetDirectories(versionsRoot);
            Array.Sort(versionFolders, StringComparer.OrdinalIgnoreCase);

            for (int i = versionFolders.Length - 1; i >= 0; i--)
            {
                string candidate = Path.Combine(versionFolders[i], "RobloxPlayerBeta.exe");
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            return string.Empty;
        }

        public static void SaveRobloxPlayerPath(string executablePath)
        {
            if (string.IsNullOrWhiteSpace(executablePath))
            {
                throw new ArgumentException("Executable path is required.", nameof(executablePath));
            }

            string settingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), SettingsFolderName);
            Directory.CreateDirectory(settingsDirectory);
            File.WriteAllText(Path.Combine(settingsDirectory, SettingsFileName), executablePath.Trim());
        }

        public static string LoadSavedRobloxPlayerPath()
        {
            string settingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), SettingsFolderName);
            string settingsPath = Path.Combine(settingsDirectory, SettingsFileName);

            if (!File.Exists(settingsPath))
            {
                return string.Empty;
            }

            try
            {
                return File.ReadAllText(settingsPath).Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetConfiguredRobloxPlayerPath()
        {
            string savedPath = LoadSavedRobloxPlayerPath();
            if (!string.IsNullOrWhiteSpace(savedPath) && File.Exists(savedPath))
            {
                return savedPath;
            }

            string discoveredPath = FindRobloxPlayerPath();
            if (!string.IsNullOrWhiteSpace(discoveredPath))
            {
                return discoveredPath;
            }

            string protocolCommandPath = GetProtocolHandlerExecutablePath();
            if (!string.IsNullOrWhiteSpace(protocolCommandPath))
            {
                return protocolCommandPath;
            }

            return string.Empty;
        }

        public static string GetCurrentProtocolHandlerCommand()
        {
            string command = ReadProtocolCommand("roblox-player");
            if (string.IsNullOrWhiteSpace(command))
            {
                command = ReadProtocolCommand("roblox");
            }

            return command;
        }

        public static void LaunchRoblox()
        {
            ProcessStartInfo? startInfo = CreateLaunchStartInfo();
            if (startInfo == null)
            {
                throw new InvalidOperationException("RobloxPlayerBeta.exe was not found in the local Roblox installation.");
            }

            Process.Start(startInfo);
        }

        public static ProcessStartInfo? CreateLaunchStartInfo()
        {
            string executablePath = GetConfiguredRobloxPlayerPath();
            if (!string.IsNullOrWhiteSpace(executablePath))
            {
                return new ProcessStartInfo
                {
                    FileName = executablePath,
                    WorkingDirectory = Path.GetDirectoryName(executablePath) ?? string.Empty,
                    UseShellExecute = false
                };
            }

            string command = GetCurrentProtocolHandlerCommand();
            if (string.IsNullOrWhiteSpace(command))
            {
                return null;
            }

            if (!TryParseCommand(command, out string fileName, out string arguments))
            {
                return null;
            }

            if (!File.Exists(fileName))
            {
                return null;
            }

            return new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = Path.GetDirectoryName(fileName) ?? string.Empty,
                UseShellExecute = false
            };
        }

        private static string GetProtocolHandlerExecutablePath()
        {
            string command = GetCurrentProtocolHandlerCommand();
            if (string.IsNullOrWhiteSpace(command))
            {
                return string.Empty;
            }

            if (!TryParseCommand(command, out string fileName, out _))
            {
                return string.Empty;
            }

            return File.Exists(fileName) ? fileName : string.Empty;
        }

        private static string ReadProtocolCommand(string protocolName)
        {
            using (RegistryKey? classesRoot = Registry.CurrentUser.OpenSubKey(@"Software\Classes", writable: false))
            using (RegistryKey? protocolKey = classesRoot?.OpenSubKey(protocolName, writable: false))
            using (RegistryKey? shellKey = protocolKey?.OpenSubKey(@"shell\open\command", writable: false))
            {
                return shellKey?.GetValue(string.Empty) as string ?? string.Empty;
            }
        }

        private static bool TryParseCommand(string command, out string fileName, out string arguments)
        {
            fileName = string.Empty;
            arguments = string.Empty;

            string normalized = command.Trim();
            if (normalized.Length == 0)
            {
                return false;
            }

            if (normalized.StartsWith("\"", StringComparison.Ordinal))
            {
                int closingQuote = normalized.IndexOf('"', 1);
                if (closingQuote <= 1)
                {
                    return false;
                }

                fileName = normalized.Substring(1, closingQuote - 1);
                arguments = normalized.Substring(closingQuote + 1).Trim();
            }
            else
            {
                int firstSpace = normalized.IndexOf(' ');
                if (firstSpace < 0)
                {
                    fileName = normalized;
                    return true;
                }

                fileName = normalized.Substring(0, firstSpace);
                arguments = normalized.Substring(firstSpace + 1).Trim();
            }

            arguments = arguments.Replace("%1", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();
            return fileName.Length > 0;
        }
    }
}