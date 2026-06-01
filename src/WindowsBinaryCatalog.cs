using System;

namespace DiegoStrap
{
    internal sealed class WindowsBinaryTypeInfo
    {
        public WindowsBinaryTypeInfo(string name, string versionFile, string blobDir)
        {
            Name = name;
            VersionFile = versionFile;
            BlobDir = blobDir;
        }

        public string Name { get; }

        public string VersionFile { get; }

        public string BlobDir { get; }
    }

    internal static class WindowsBinaryCatalog
    {
        public static WindowsBinaryTypeInfo Resolve(string binaryTypeName)
        {
            if (binaryTypeName == null)
            {
                throw new ArgumentNullException(nameof(binaryTypeName));
            }

            string normalized = binaryTypeName.Trim();

            if (normalized.Length == 0)
            {
                throw new ArgumentException("Binary type cannot be empty.", nameof(binaryTypeName));
            }

            if (normalized.Equals("WindowsPlayer", StringComparison.OrdinalIgnoreCase))
            {
                return new WindowsBinaryTypeInfo("WindowsPlayer", "/version", "/");
            }

            if (normalized.Equals("WindowsStudio64", StringComparison.OrdinalIgnoreCase))
            {
                return new WindowsBinaryTypeInfo("WindowsStudio64", "/versionQTStudio", "/");
            }

            throw new ArgumentException("Unsupported Windows binary type: " + binaryTypeName, nameof(binaryTypeName));
        }
    }
}