using Microsoft.Win32;
using System;
using System.IO;

namespace DiegoStrap
{
    internal sealed class ProtocolHandlerRegistrar
    {
        private const string AppRoot = @"Software\Classes";
        private const string BackupFileName = "protocol-handler-backup.json";

        private readonly string executablePath;
        private readonly string executableDirectory;
        private readonly string backupPath;

        public ProtocolHandlerRegistrar(string executablePath)
        {
            if (string.IsNullOrWhiteSpace(executablePath))
            {
                throw new ArgumentException("Executable path is required.", nameof(executablePath));
            }

            this.executablePath = executablePath;
            executableDirectory = Path.GetDirectoryName(executablePath) ?? AppContext.BaseDirectory;
            backupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DiegoStrap", BackupFileName);
        }

        public void RegisterProtocols()
        {
            EnsureBackupIfNeeded();
            WriteProtocolKey("roblox", BuildCommandLine());
            WriteProtocolKey("roblox-player", BuildCommandLine());
        }

        public void UnregisterProtocols()
        {
            DeleteProtocolKey("roblox");
            DeleteProtocolKey("roblox-player");
        }

        public void RestoreDefaultRobloxHandler()
        {
            if (File.Exists(backupPath))
            {
                ProtocolRegistryBackup backup = ProtocolRegistryBackup.LoadFromFile(backupPath);
                backup.Restore();
                return;
            }

            string officialPath = RobloxLocator.FindRobloxPlayerBetaExecutable();
            if (string.IsNullOrWhiteSpace(officialPath))
            {
                throw new FileNotFoundException("RobloxPlayerBeta.exe was not found in the local Roblox installation.");
            }

            string command = BuildCommandLine(officialPath);
            WriteProtocolKey("roblox", command);
            WriteProtocolKey("roblox-player", command);
        }

        public bool IsRegistered()
        {
            return IsProtocolOwnedByThisApp("roblox") && IsProtocolOwnedByThisApp("roblox-player");
        }

        public bool IsOwnedByAnotherApp(out string? owner)
        {
            owner = GetCurrentOwnerName("roblox");
            string? playerOwner = GetCurrentOwnerName("roblox-player");

            if (string.IsNullOrWhiteSpace(owner) && !string.IsNullOrWhiteSpace(playerOwner))
            {
                owner = playerOwner;
            }

            if (string.IsNullOrWhiteSpace(owner))
            {
                return false;
            }

            return !IsRegistered();
        }

        public string GetStatus()
        {
            if (IsRegistered())
            {
                return "Registered to DiegoStrap.";
            }

            string? owner = GetCurrentOwnerName("roblox");
            if (string.IsNullOrWhiteSpace(owner))
            {
                owner = "Unregistered";
            }

            return "Current owner: " + owner;
        }

        private void EnsureBackupIfNeeded()
        {
            if (File.Exists(backupPath))
            {
                return;
            }

            ProtocolRegistryBackup backup = ProtocolRegistryBackup.Capture();
            Directory.CreateDirectory(Path.GetDirectoryName(backupPath) ?? AppContext.BaseDirectory);
            backup.SaveToFile(backupPath);
        }

        private void WriteProtocolKey(string protocolName, string command)
        {
            using (RegistryKey? classesRoot = Registry.CurrentUser.OpenSubKey(AppRoot, writable: true))
            {
                if (classesRoot == null)
                {
                    throw new UnauthorizedAccessException("Unable to open HKCU\\Software\\Classes.");
                }

                using (RegistryKey protocolKey = classesRoot.CreateSubKey(protocolName, true) ?? throw new IOException("Unable to create protocol key."))
                {
                    protocolKey.SetValue(string.Empty, "URL:" + protocolName + " Protocol", RegistryValueKind.String);
                    protocolKey.SetValue("URL Protocol", string.Empty, RegistryValueKind.String);

                    using (RegistryKey defaultIcon = protocolKey.CreateSubKey("DefaultIcon", true) ?? throw new IOException("Unable to create DefaultIcon key."))
                    {
                        defaultIcon.SetValue(string.Empty, Quote(executablePath) + ",0", RegistryValueKind.String);
                    }

                    using (RegistryKey shellKey = protocolKey.CreateSubKey(@"shell\open\command", true) ?? throw new IOException("Unable to create command key."))
                    {
                        shellKey.SetValue(string.Empty, command, RegistryValueKind.String);
                    }
                }
            }
        }

        private void DeleteProtocolKey(string protocolName)
        {
            using (RegistryKey? classesRoot = Registry.CurrentUser.OpenSubKey(AppRoot, writable: true))
            {
                classesRoot?.DeleteSubKeyTree(protocolName, throwOnMissingSubKey: false);
            }
        }

        private bool IsProtocolOwnedByThisApp(string protocolName)
        {
            string? command = ReadProtocolCommand(protocolName);
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return command.Contains(executablePath, StringComparison.OrdinalIgnoreCase);
        }

        private string? GetCurrentOwnerName(string protocolName)
        {
            string? command = ReadProtocolCommand(protocolName);
            if (string.IsNullOrWhiteSpace(command))
            {
                return null;
            }

            if (command.Contains(executablePath, StringComparison.OrdinalIgnoreCase))
            {
                return "DiegoStrap";
            }

            if (command.Contains("RobloxPlayerBeta.exe", StringComparison.OrdinalIgnoreCase))
            {
                return "Roblox";
            }

            return command;
        }

        private string? ReadProtocolCommand(string protocolName)
        {
            using (RegistryKey? classesRoot = Registry.CurrentUser.OpenSubKey(AppRoot, writable: false))
            using (RegistryKey? protocolKey = classesRoot?.OpenSubKey(protocolName, writable: false))
            using (RegistryKey? shellKey = protocolKey?.OpenSubKey(@"shell\open\command", writable: false))
            {
                return shellKey?.GetValue(string.Empty) as string;
            }
        }

        private string BuildCommandLine()
        {
            return BuildCommandLine(executablePath);
        }

        private static string BuildCommandLine(string executablePath)
        {
            return Quote(executablePath) + " \"%1\"";
        }

        private static string Quote(string value)
        {
            return '"' + value + '"';
        }
    }
}