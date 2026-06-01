namespace DiegoStrap
{
    internal sealed class DownloadProgressInfo
    {
        public int Percent { get; set; }

        public string StatusText { get; set; } = string.Empty;

        public string SpeedText { get; set; } = string.Empty;
    }
}