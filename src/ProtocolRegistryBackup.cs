using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DiegoStrap
{
    internal sealed class ProtocolRegistryBackup
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public List<RegistrySnapshot> Protocols { get; set; } = new List<RegistrySnapshot>();

        public static ProtocolRegistryBackup Capture()
        {
            ProtocolRegistryBackup backup = new ProtocolRegistryBackup();
            backup.Protocols.Add(CaptureKey("roblox"));
            backup.Protocols.Add(CaptureKey("roblox-player"));
            return backup;
        }

        public void Restore()
        {
            foreach (RegistrySnapshot snapshot in Protocols)
            {
                snapshot.Restore();
            }
        }

        public void SaveToFile(string path)
        {
            string? directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonSerializer.Serialize(this, JsonOptions);
            File.WriteAllText(path, json);
        }

        public static ProtocolRegistryBackup LoadFromFile(string path)
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<ProtocolRegistryBackup>(json, JsonOptions) ?? new ProtocolRegistryBackup();
        }

        private static RegistrySnapshot CaptureKey(string path)
        {
            using (RegistryKey? classesRoot = Registry.CurrentUser.OpenSubKey(@"Software\Classes", writable: false))
            using (RegistryKey? key = classesRoot?.OpenSubKey(path, writable: false))
            {
                if (key == null)
                {
                    return new RegistrySnapshot { Path = path, Exists = false };
                }

                return RegistrySnapshot.FromRegistryKey(path, key);
            }
        }
    }

    internal sealed class RegistrySnapshot
    {
        public string Path { get; set; } = string.Empty;

        public bool Exists { get; set; }

        public List<RegistryValueSnapshot> Values { get; set; } = new List<RegistryValueSnapshot>();

        public List<RegistrySnapshot> Children { get; set; } = new List<RegistrySnapshot>();

        public static RegistrySnapshot FromRegistryKey(string path, RegistryKey key)
        {
            RegistrySnapshot snapshot = new RegistrySnapshot
            {
                Path = path,
                Exists = true
            };

            foreach (string valueName in key.GetValueNames())
            {
                object? value = key.GetValue(valueName);
                RegistryValueKind kind = key.GetValueKind(valueName);
                snapshot.Values.Add(new RegistryValueSnapshot
                {
                    Name = valueName,
                    Kind = kind,
                    Data = value?.ToString() ?? string.Empty
                });
            }

            foreach (string subKeyName in key.GetSubKeyNames())
            {
                using (RegistryKey? child = key.OpenSubKey(subKeyName, writable: false))
                {
                    if (child != null)
                    {
                        snapshot.Children.Add(FromRegistryKey(path + @"\" + subKeyName, child));
                    }
                }
            }

            return snapshot;
        }

        public void Restore()
        {
            using (RegistryKey? classesRoot = Registry.CurrentUser.OpenSubKey(@"Software\Classes", writable: true))
            {
                if (classesRoot == null)
                {
                    throw new UnauthorizedAccessException("Unable to open HKCU\\Software\\Classes.");
                }

                if (!Exists)
                {
                    classesRoot.DeleteSubKeyTree(Path, throwOnMissingSubKey: false);
                    return;
                }

                classesRoot.DeleteSubKeyTree(Path, throwOnMissingSubKey: false);

                using (RegistryKey restored = classesRoot.CreateSubKey(Path, true) ?? throw new IOException("Unable to restore registry key."))
                {
                    foreach (RegistryValueSnapshot value in Values)
                    {
                        restored.SetValue(value.Name, value.Data, value.Kind);
                    }

                    foreach (RegistrySnapshot child in Children)
                    {
                        child.RestoreUnder(restored);
                    }
                }
            }
        }

        private void RestoreUnder(RegistryKey parent)
        {
            string relativePath = Path;
            int index = relativePath.LastIndexOf('\\');
            string leaf = index >= 0 ? relativePath.Substring(index + 1) : relativePath;

            using (RegistryKey child = parent.CreateSubKey(leaf, true) ?? throw new IOException("Unable to restore child registry key."))
            {
                foreach (RegistryValueSnapshot value in Values)
                {
                    child.SetValue(value.Name, value.Data, value.Kind);
                }

                foreach (RegistrySnapshot grandChild in Children)
                {
                    grandChild.RestoreUnder(child);
                }
            }
        }
    }

    internal sealed class RegistryValueSnapshot
    {
        public string Name { get; set; } = string.Empty;

        public RegistryValueKind Kind { get; set; }

        public string Data { get; set; } = string.Empty;
    }
}