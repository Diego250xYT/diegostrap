namespace DiegoStrap
{
    internal enum VersionSource
    {
        Live,
        Future,
        Past
    }

    internal sealed class DownloadRequest
    {
        public string Channel { get; set; } = string.Empty;

        public string BinaryType { get; set; } = string.Empty;

        public VersionSource VersionSource { get; set; }

        public bool UseVersionHash { get; set; }

        public string VersionHash { get; set; } = string.Empty;
    }
}