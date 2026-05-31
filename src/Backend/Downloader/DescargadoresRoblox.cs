using System;
using System.Threading;
using System.Threading.Tasks;

// Canales de version de Roblox expuestos como API en espanol.
public static class RobloxVersionChannels
{
    public static string GetLive()
    {
        return RobloxVersionChannel.Live();
    }

    public static string GetFuture()
    {
        return RobloxVersionChannel.Future();
    }

    public static string GetPast()
    {
        return RobloxVersionChannel.Past();
    }

    public static string[] GetAll()
    {
        return RobloxVersionChannel.GetAll();
    }
}

// Informacion de una version de Roblox con nombres en espanol.
public sealed class RobloxVersionInfoPublic
{
    public string Channel { get; set; }

    public string VersionHash { get; set; }

    public string Version { get; set; }

    public bool IsBehindLive { get; set; }

    public DateTime? ReleaseDateUtc { get; set; }

    public RobloxVersionInfoPublic()
    {
        Channel = string.Empty;
        VersionHash = string.Empty;
        Version = string.Empty;
        IsBehindLive = false;
        ReleaseDateUtc = null;
    }

    public static RobloxVersionInfoPublic FromCore(RobloxVersionInfo info)
    {
        RobloxVersionInfoPublic version = new RobloxVersionInfoPublic();
        version.Channel = info.Channel;
        version.VersionHash = info.VersionGuid;
        version.Version = info.Version;
        version.IsBehindLive = info.IsBehindLive;
        version.ReleaseDateUtc = info.ReleaseDateUtc;
        return version;
    }
}

// Opciones de descarga RDD con nombres en espanol.
public sealed class RobloxRddDownloadOptionsPublic
{
    public string Channel { get; set; }

    public string BinaryType { get; set; }

    public string VersionHash { get; set; }

    public string? Exploit { get; set; }

    public string? BlobDirectory { get; set; }

    public bool IncludeLauncher { get; set; }

    public bool CompressZip { get; set; }

    public int CompressionLevel { get; set; }

    public bool ParallelDownloads { get; set; }

    public string? OutputZipPath { get; set; }

    public RobloxRddDownloadOptionsPublic()
    {
        Channel = RobloxVersionChannels.GetLive();
        BinaryType = "WindowsPlayer";
        VersionHash = string.Empty;
        Exploit = null;
        BlobDirectory = null;
        IncludeLauncher = false;
        CompressZip = false;
        CompressionLevel = 5;
        ParallelDownloads = true;
        OutputZipPath = null;
    }

    internal RobloxRddDownloadOptions ABase()
    {
        RobloxRddDownloadOptions opciones = new RobloxRddDownloadOptions();
        opciones.Channel = Channel;
        opciones.BinaryType = BinaryType;
        opciones.VersionHash = VersionHash;
        opciones.ExploitTitle = Exploit;
        opciones.BlobDirectory = BlobDirectory;
        opciones.IncludeLauncher = IncludeLauncher;
        opciones.CompressZip = CompressZip;
        opciones.CompressionLevel = CompressionLevel;
        opciones.ParallelDownloads = ParallelDownloads;
        opciones.OutputZipPath = OutputZipPath;
        return opciones;
    }
}

// Resultado de una descarga RDD con nombres en espanol.
public sealed class RobloxRddDownloadResultPublic
{
    public string Channel { get; set; }

    public string BinaryType { get; set; }

    public string VersionHash { get; set; }

    public string OutputZipPath { get; set; }

    public int PackageCount { get; set; }

    public long TotalDownloadedBytes { get; set; }

    public bool IncludedLauncher { get; set; }

    public bool UsedDirectZip { get; set; }

    public RobloxRddDownloadResultPublic()
    {
        Channel = string.Empty;
        BinaryType = string.Empty;
        VersionHash = string.Empty;
        OutputZipPath = string.Empty;
        PackageCount = 0;
        TotalDownloadedBytes = 0;
        IncludedLauncher = false;
        UsedDirectZip = false;
    }

    public static RobloxRddDownloadResultPublic FromCore(RobloxRddDownloadResult resultado)
    {
        RobloxRddDownloadResultPublic copia = new RobloxRddDownloadResultPublic();
        copia.Channel = resultado.Channel;
        copia.BinaryType = resultado.BinaryType;
        copia.VersionHash = resultado.VersionHash;
        copia.OutputZipPath = resultado.OutputZipPath;
        copia.PackageCount = resultado.PackageCount;
        copia.TotalDownloadedBytes = resultado.TotalDownloadedBytes;
        copia.IncludedLauncher = resultado.IncludedLauncher;
        copia.UsedDirectZip = resultado.UsedDirectZip;
        return copia;
    }
}

// Resolutor publico en espanol.
public static class RobloxVersionResolverApi
{
    public static Task<RobloxVersionInfoPublic> ResolveLiveAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return ConvertAsync(RobloxVersionResolver.ResolveLiveAsync(cancellationToken));
    }

    public static Task<RobloxVersionInfoPublic> ResolveFutureAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return ConvertAsync(RobloxVersionResolver.ResolveFutureAsync(cancellationToken));
    }

    public static Task<RobloxVersionInfoPublic> ResolvePastAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return ConvertAsync(RobloxVersionResolver.ResolvePastAsync(cancellationToken));
    }

    public static Task<RobloxVersionInfoPublic> ResolveAsync(string channel, CancellationToken cancellationToken = default(CancellationToken))
    {
        return ConvertAsync(RobloxVersionResolver.ResolveAsync(channel, cancellationToken));
    }

    public static bool IsChannelSupported(string channel)
    {
        return RobloxVersionResolver.IsSupportedChannel(channel);
    }

    public static RobloxVersionInfoPublic CreateVersionInfo(string channel)
    {
        return RobloxVersionInfoPublic.FromCore(RobloxVersionResolver.CreateVersionInfo(channel));
    }

    private static async Task<RobloxVersionInfoPublic> ConvertAsync(Task<RobloxVersionInfo> task)
    {
        return RobloxVersionInfoPublic.FromCore(await task);
    }
}

// Cliente WEAO expuesto con nombres en espanol.
public static class RobloxWeaoClientApi
{
    public static Task<RobloxVersionInfoPublic> GetLiveAsync(string binaryType, CancellationToken cancellationToken = default(CancellationToken))
    {
        return ConvertAsync(WeaoRobloxVersionClient.GetLiveAsync(binaryType, cancellationToken));
    }

    public static Task<RobloxVersionInfoPublic> GetFutureAsync(string binaryType, CancellationToken cancellationToken = default(CancellationToken))
    {
        return ConvertAsync(WeaoRobloxVersionClient.GetFutureAsync(binaryType, cancellationToken));
    }

    public static Task<RobloxVersionInfoPublic> GetPastAsync(string binaryType, CancellationToken cancellationToken = default(CancellationToken))
    {
        return ConvertAsync(WeaoRobloxVersionClient.GetPastAsync(binaryType, cancellationToken));
    }

    public static Task<string> GetExploitVersionAsync(string exploit, CancellationToken cancellationToken = default(CancellationToken))
    {
        return WeaoRobloxVersionClient.GetExploitVersionAsync(exploit, cancellationToken);
    }

    public static Task<bool> VersionExistsAsync(string channel, string binaryType, string versionHash, CancellationToken cancellationToken = default(CancellationToken))
    {
        return RobloxRddDownloader.VersionExistsAsync(channel, binaryType, versionHash, cancellationToken);
    }

    private static async Task<RobloxVersionInfoPublic> ConvertAsync(Task<RobloxVersionInfo> task)
    {
        return RobloxVersionInfoPublic.FromCore(await task);
    }
}

// Descargador RDD expuesto con nombres en espanol.
public static class RobloxRddDownloaderApi
{
    public static Task<RobloxRddDownloadResultPublic> DownloadLatestAsync(RobloxRddDownloadOptionsPublic options, CancellationToken cancellationToken = default(CancellationToken))
    {
        return ConvertAsync(RobloxRddDownloader.DownloadLatestAsync(options.ABase(), cancellationToken));
    }

    public static Task<RobloxRddDownloadResultPublic> DownloadPreviousAsync(RobloxRddDownloadOptionsPublic options, CancellationToken cancellationToken = default(CancellationToken))
    {
        return ConvertAsync(RobloxRddDownloader.DownloadPreviousAsync(options.ABase(), cancellationToken));
    }

    public static Task<RobloxRddDownloadResultPublic> DownloadSpecifiedAsync(RobloxRddDownloadOptionsPublic options, CancellationToken cancellationToken = default(CancellationToken))
    {
        return ConvertAsync(RobloxRddDownloader.DownloadSpecifiedAsync(options.ABase(), cancellationToken));
    }

    public static string BuildDownloadLink(RobloxRddDownloadOptionsPublic options)
    {
        return RobloxRddDownloader.BuildDownloadLink(options.ABase());
    }

    public static Task<bool> VersionExistsAsync(string channel, string binaryType, string versionHash, CancellationToken cancellationToken = default(CancellationToken))
    {
        return RobloxRddDownloader.VersionExistsAsync(channel, binaryType, versionHash, cancellationToken);
    }

    private static async Task<RobloxRddDownloadResultPublic> ConvertAsync(Task<RobloxRddDownloadResult> task)
    {
        return RobloxRddDownloadResultPublic.FromCore(await task);
    }
}