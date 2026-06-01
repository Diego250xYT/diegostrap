using System;
using System.Diagnostics;
using System.IO;

namespace DiegoStrap
{
    internal static class RobloxProtocolDispatcher
    {
        public static bool IsRobloxProtocolUri(string? uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                return false;
            }

            return uri.StartsWith("roblox://", StringComparison.OrdinalIgnoreCase) || uri.StartsWith("roblox-player://", StringComparison.OrdinalIgnoreCase);
        }

        public static void ForwardUri(string uri)
        {
            if (!IsRobloxProtocolUri(uri))
            {
                throw new ArgumentException("The provided URI is not a Roblox protocol URI.", nameof(uri));
            }

            string executablePath = RobloxLocator.ResolveExecutable();
            if (string.IsNullOrWhiteSpace(executablePath))
            {
                throw new FileNotFoundException("RobloxPlayerBeta.exe was not found. Install Roblox or place the executable in the portable folder.");
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = '"' + uri + '"',
                WorkingDirectory = Path.GetDirectoryName(executablePath) ?? AppContext.BaseDirectory,
                UseShellExecute = false
            };

            Process.Start(startInfo);
        }
    }
}