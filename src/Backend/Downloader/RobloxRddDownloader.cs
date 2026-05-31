using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// Opciones para descargar y empaquetar una version de Roblox al estilo RDD.
public sealed class RobloxRddDownloadOptions
{
    // Canal de lanzamiento del paquete.
    public string Channel { get; set; }

    // Tipo de binario de Roblox.
    public string BinaryType { get; set; }

    // Hash de version concreto. Si viene vacio, se resuelve desde WEAO.
    public string VersionHash { get; set; }

    // Nombre de un ejecutor WEAO para resolver el hash automaticamente.
    public string? ExploitTitle { get; set; }

    // Directorio de blobs relativo, por ejemplo "/" o "/mac/".
    public string? BlobDirectory { get; set; }

    // Incluye weblauncher.exe para Windows.
    public bool IncludeLauncher { get; set; }

    // Comprime el ZIP final usando compresion normal.
    public bool CompressZip { get; set; }

    // Valor de compresion 1..9. Se conserva para compatibilidad con la UI y los enlaces.
    public int CompressionLevel { get; set; }

    // Descarga paquetes en paralelo cuando es posible.
    public bool ParallelDownloads { get; set; }

    // Ruta del ZIP final. Si viene vacia, se usa la carpeta actual.
    public string? OutputZipPath { get; set; }

    public RobloxRddDownloadOptions()
    {
        Channel = RobloxVersionChannel.Live();
        BinaryType = "WindowsPlayer";
        VersionHash = string.Empty;
        ExploitTitle = null;
        BlobDirectory = null;
        IncludeLauncher = false;
        CompressZip = false;
        CompressionLevel = 5;
        ParallelDownloads = true;
        OutputZipPath = null;
    }
}

// Resultado de una descarga RDD.
public sealed class RobloxRddDownloadResult
{
    // Canal efectivo usado para la descarga.
    public string Channel { get; set; }

    // Binario efectivo usado para la descarga.
    public string BinaryType { get; set; }

    // Hash de version descargado.
    public string VersionHash { get; set; }

    // Ruta final del ZIP creado.
    public string OutputZipPath { get; set; }

    // Numero de paquetes descargados y añadidos al ZIP.
    public int PackageCount { get; set; }

    // Bytes totales descargados desde Roblox.
    public long TotalDownloadedBytes { get; set; }

    // Indica si se incluyo el launcher de WEAO.
    public bool IncludedLauncher { get; set; }

    // Indica si la descarga se hizo con el flujo Mac de zip directo.
    public bool UsedDirectZip { get; set; }

    public RobloxRddDownloadResult()
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
}

// Replica en C# el flujo de descarga de rdd.weao.gg.
public static class RobloxRddDownloader
{
    private const string HostPath = "https://setup-aws.rbxcdn.com";
    private const string LauncherUrl = "https://curly-shape-1578.vnnaworks.workers.dev/";

    private static readonly HttpClient HttpClient = CreateHttpClient();

    private static readonly Dictionary<string, string> WindowsPlayerRoots = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "RobloxApp.zip", string.Empty },
        { "redist.zip", string.Empty },
        { "shaders.zip", "shaders/" },
        { "ssl.zip", "ssl/" },
        { "WebView2.zip", string.Empty },
        { "WebView2RuntimeInstaller.zip", "WebView2RuntimeInstaller/" },
        { "content-avatar.zip", "content/avatar/" },
        { "content-configs.zip", "content/configs/" },
        { "content-fonts.zip", "content/fonts/" },
        { "content-sky.zip", "content/sky/" },
        { "content-sounds.zip", "content/sounds/" },
        { "content-textures2.zip", "content/textures/" },
        { "content-models.zip", "content/models/" },
        { "content-platform-fonts.zip", "PlatformContent/pc/fonts/" },
        { "content-platform-dictionaries.zip", "PlatformContent/pc/shared_compression_dictionaries/" },
        { "content-terrain.zip", "PlatformContent/pc/terrain/" },
        { "content-textures3.zip", "PlatformContent/pc/textures/" },
        { "extracontent-luapackages.zip", "ExtraContent/LuaPackages/" },
        { "extracontent-translations.zip", "ExtraContent/translations/" },
        { "extracontent-models.zip", "ExtraContent/models/" },
        { "extracontent-textures.zip", "ExtraContent/textures/" },
        { "extracontent-places.zip", "ExtraContent/places/" }
    };

    private static readonly Dictionary<string, string> WindowsStudioRoots = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "RobloxStudio.zip", string.Empty },
        { "RibbonConfig.zip", "RibbonConfig/" },
        { "redist.zip", string.Empty },
        { "Libraries.zip", string.Empty },
        { "LibrariesQt5.zip", string.Empty },
        { "WebView2.zip", string.Empty },
        { "WebView2RuntimeInstaller.zip", string.Empty },
        { "shaders.zip", "shaders/" },
        { "ssl.zip", "ssl/" },
        { "Qml.zip", "Qml/" },
        { "Plugins.zip", "Plugins/" },
        { "StudioFonts.zip", "StudioFonts/" },
        { "BuiltInPlugins.zip", "BuiltInPlugins/" },
        { "ApplicationConfig.zip", "ApplicationConfig/" },
        { "BuiltInStandalonePlugins.zip", "BuiltInStandalonePlugins/" },
        { "content-qt_translations.zip", "content/qt_translations/" },
        { "content-sky.zip", "content/sky/" },
        { "content-fonts.zip", "content/fonts/" },
        { "content-avatar.zip", "content/avatar/" },
        { "content-models.zip", "content/models/" },
        { "content-sounds.zip", "content/sounds/" },
        { "content-configs.zip", "content/configs/" },
        { "content-api-docs.zip", "content/api_docs/" },
        { "content-textures2.zip", "content/textures/" },
        { "content-studio_svg_textures.zip", "content/studio_svg_textures/" },
        { "content-platform-fonts.zip", "PlatformContent/pc/fonts/" },
        { "content-platform-dictionaries.zip", "PlatformContent/pc/shared_compression_dictionaries/" },
        { "content-terrain.zip", "PlatformContent/pc/terrain/" },
        { "content-textures3.zip", "PlatformContent/pc/textures/" },
        { "extracontent-translations.zip", "ExtraContent/translations/" },
        { "extracontent-luapackages.zip", "ExtraContent/LuaPackages/" },
        { "extracontent-textures.zip", "ExtraContent/textures/" },
        { "extracontent-scripts.zip", "ExtraContent/scripts/" },
        { "extracontent-models.zip", "ExtraContent/models/" },
        { "studiocontent-models.zip", "StudioContent/models/" },
        { "studiocontent-textures.zip", "StudioContent/textures/" }
    };

    // Descarga el paquete mas reciente para la configuracion dada.
    public static Task<RobloxRddDownloadResult> DownloadLatestAsync(RobloxRddDownloadOptions options, CancellationToken cancellationToken = default(CancellationToken))
    {
        return DownloadAsync(options, RddVersionSource.Latest, cancellationToken);
    }

    // Descarga el paquete anterior para la configuracion dada.
    public static Task<RobloxRddDownloadResult> DownloadPreviousAsync(RobloxRddDownloadOptions options, CancellationToken cancellationToken = default(CancellationToken))
    {
        return DownloadAsync(options, RddVersionSource.Previous, cancellationToken);
    }

    // Descarga un paquete especifico para la configuracion dada.
    public static Task<RobloxRddDownloadResult> DownloadSpecifiedAsync(RobloxRddDownloadOptions options, CancellationToken cancellationToken = default(CancellationToken))
    {
        return DownloadAsync(options, RddVersionSource.Specified, cancellationToken);
    }

    // Genera un enlace directo con la misma forma que rdd.weao.gg.
    public static string BuildDownloadLink(RobloxRddDownloadOptions options)
    {
        RobloxRddDownloadOptions normalizedOptions = NormalizeOptions(options);
        string channel = NormalizeChannel(normalizedOptions.Channel);
        string binaryType = normalizedOptions.BinaryType;
        string versionHash = NormalizeVersionHash(normalizedOptions.VersionHash);

        string query = "?channel=" + Uri.EscapeDataString(channel);
        query += "&binaryType=" + Uri.EscapeDataString(binaryType);

        if (!string.IsNullOrWhiteSpace(versionHash))
        {
            query += "&version=" + Uri.EscapeDataString(versionHash);
        }

        if (!string.IsNullOrWhiteSpace(normalizedOptions.BlobDirectory))
        {
            query += "&blobDir=" + Uri.EscapeDataString(NormalizeBlobDirectory(normalizedOptions.BlobDirectory, binaryType));
        }

        if (normalizedOptions.CompressZip)
        {
            query += "&compressZip=true&compressionLevel=" + normalizedOptions.CompressionLevel;
        }

        if (normalizedOptions.IncludeLauncher)
        {
            query += "&includeLauncher=true";
        }

        query += "&parallelDownloads=" + normalizedOptions.ParallelDownloads.ToString().ToLowerInvariant();

        return HostPath + query;
    }

    // Verifica si una version concreta sigue existiendo en la base de Roblox.
    public static async Task<bool> VersionExistsAsync(string channel, string binaryType, string versionHash, CancellationToken cancellationToken = default(CancellationToken))
    {
        string normalizedChannel = NormalizeChannel(channel);
        string normalizedBinaryType = NormalizeBinaryType(binaryType);
        string normalizedVersionHash = NormalizeVersionHash(versionHash);

        if (string.IsNullOrWhiteSpace(normalizedVersionHash))
        {
            return false;
        }

        string blobDirectory = NormalizeBlobDirectory(null, normalizedBinaryType);
        string versionPath = BuildChannelPath(normalizedChannel) + blobDirectory + normalizedVersionHash + "-rbxPkgManifest.txt";

        using (HttpResponseMessage response = await HttpClient.GetAsync(versionPath, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
        {
            return response.IsSuccessStatusCode;
        }
    }

    private enum RddVersionSource
    {
        Latest,
        Previous,
        Specified
    }

    private static async Task<RobloxRddDownloadResult> DownloadAsync(RobloxRddDownloadOptions options, RddVersionSource versionSource, CancellationToken cancellationToken)
    {
        RobloxRddDownloadOptions normalizedOptions = NormalizeOptions(options);
        string channel = NormalizeChannel(normalizedOptions.Channel);
        string binaryType = normalizedOptions.BinaryType;
        string versionHash = await ResolveVersionHashAsync(normalizedOptions, binaryType, versionSource, cancellationToken);
        string blobDirectory = NormalizeBlobDirectory(normalizedOptions.BlobDirectory, binaryType);
        string channelPath = BuildChannelPath(channel);
        string versionPath = channelPath + blobDirectory + versionHash + "-";
        string outputZipPath = ResolveOutputPath(normalizedOptions, channel, binaryType, versionHash);

        if (IsMacBinary(binaryType))
        {
            byte[] zipData = await DownloadBytesAsync(versionPath + GetMacZipFileName(binaryType), cancellationToken);
            WriteAllBytes(outputZipPath, zipData);

            RobloxRddDownloadResult directResult = new RobloxRddDownloadResult();
            directResult.Channel = channel;
            directResult.BinaryType = binaryType;
            directResult.VersionHash = versionHash;
            directResult.OutputZipPath = outputZipPath;
            directResult.PackageCount = 1;
            directResult.TotalDownloadedBytes = zipData.LongLength;
            directResult.IncludedLauncher = false;
            directResult.UsedDirectZip = true;
            return directResult;
        }

        string manifestText = await DownloadTextAsync(versionPath + "rbxPkgManifest.txt", cancellationToken);
        string[] manifestLines = SplitLines(manifestText);

        if (manifestLines.Length == 0 || !string.Equals(manifestLines[0], "v0", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Unknown rbxPkgManifest format version.");
        }

        bool isStudio = IsWindowsStudioBinary(binaryType) || ContainsLine(manifestLines, "RobloxStudio.zip");
        bool isPlayer = IsWindowsPlayerBinary(binaryType) || ContainsLine(manifestLines, "RobloxApp.zip");

        if (isStudio && IsWindowsPlayerBinary(binaryType))
        {
            throw new InvalidOperationException("Binary type WindowsPlayer was provided but the manifest is for Roblox Studio.");
        }

        if (isPlayer && IsWindowsStudioBinary(binaryType))
        {
            throw new InvalidOperationException("Binary type WindowsStudio64 was provided but the manifest is for Roblox Player.");
        }

        Dictionary<string, string> extractRoots = isStudio ? WindowsStudioRoots : WindowsPlayerRoots;
        List<string> packageNames = ExtractPackageNames(manifestLines);
        Dictionary<string, byte[]> packageBytes = normalizedOptions.ParallelDownloads
            ? await DownloadPackagesInParallelAsync(versionPath, packageNames, cancellationToken)
            : await DownloadPackagesSequentialAsync(versionPath, packageNames, cancellationToken);

        EnsureDirectoryForFile(outputZipPath);

        long totalDownloadedBytes = 0;
        int packageCount = 0;
        bool outputCompressed = normalizedOptions.CompressZip;
        CompressionLevel compressionLevel = outputCompressed ? CompressionLevel.Optimal : CompressionLevel.NoCompression;
        HashSet<string> addedEntries = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        using (FileStream outputStream = new FileStream(outputZipPath, FileMode.Create, FileAccess.Write, FileShare.None))
        using (ZipArchive archive = new ZipArchive(outputStream, ZipArchiveMode.Create, false))
        {
            AddTextEntry(archive, "AppSettings.xml", AppSettingsXml(), compressionLevel, addedEntries);

            foreach (string packageName in packageNames)
            {
                byte[] blobData = packageBytes[packageName];
                totalDownloadedBytes += blobData.LongLength;
                packageCount++;

                if (!extractRoots.ContainsKey(packageName))
                {
                    AddBytesEntry(archive, packageName, blobData, compressionLevel, addedEntries);
                    continue;
                }

                string extractRootFolder = extractRoots[packageName];
                AddExtractedPackageEntries(archive, blobData, extractRootFolder, compressionLevel, addedEntries);
            }

            if (normalizedOptions.IncludeLauncher)
            {
                byte[] launcherData = await DownloadBytesAsync(LauncherUrl, cancellationToken);
                totalDownloadedBytes += launcherData.LongLength;
                AddBytesEntry(archive, "weblauncher.exe", launcherData, compressionLevel, addedEntries);
            }
        }

        RobloxRddDownloadResult result = new RobloxRddDownloadResult();
        result.Channel = channel;
        result.BinaryType = binaryType;
        result.VersionHash = versionHash;
        result.OutputZipPath = outputZipPath;
        result.PackageCount = packageCount;
        result.TotalDownloadedBytes = totalDownloadedBytes;
        result.IncludedLauncher = normalizedOptions.IncludeLauncher;
        result.UsedDirectZip = false;
        return result;
    }

    private static async Task<string> ResolveVersionHashAsync(RobloxRddDownloadOptions options, string binaryType, RddVersionSource versionSource, CancellationToken cancellationToken)
    {
        string versionHash = NormalizeVersionHash(options.VersionHash);

        if (!string.IsNullOrWhiteSpace(versionHash))
        {
            return versionHash;
        }

        if (!string.IsNullOrWhiteSpace(options.ExploitTitle))
        {
            return NormalizeVersionHash(await WeaoRobloxVersionClient.GetExploitVersionAsync(options.ExploitTitle, cancellationToken));
        }

        if (versionSource == RddVersionSource.Latest)
        {
            return NormalizeVersionHash((await WeaoRobloxVersionClient.GetLiveAsync(binaryType, cancellationToken)).VersionGuid);
        }

        if (versionSource == RddVersionSource.Previous)
        {
            return NormalizeVersionHash((await WeaoRobloxVersionClient.GetPastAsync(binaryType, cancellationToken)).VersionGuid);
        }

        throw new InvalidOperationException("A specific version hash is required for the specified download mode.");
    }

    private static RobloxRddDownloadOptions NormalizeOptions(RobloxRddDownloadOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        RobloxRddDownloadOptions copy = new RobloxRddDownloadOptions();
        copy.Channel = NormalizeChannel(options.Channel);
        copy.BinaryType = NormalizeBinaryType(options.BinaryType);
        copy.VersionHash = NormalizeVersionHash(options.VersionHash);
        copy.ExploitTitle = options.ExploitTitle;
        copy.BlobDirectory = string.IsNullOrWhiteSpace(options.BlobDirectory) ? null : NormalizeBlobDirectory(options.BlobDirectory, copy.BinaryType);
        copy.IncludeLauncher = options.IncludeLauncher;
        copy.CompressZip = options.CompressZip;
        copy.CompressionLevel = ClampCompressionLevel(options.CompressionLevel);
        copy.ParallelDownloads = options.ParallelDownloads;
        copy.OutputZipPath = options.OutputZipPath;
        return copy;
    }

    private static string NormalizeChannel(string channel)
    {
        if (string.IsNullOrWhiteSpace(channel))
        {
            return "LIVE";
        }

        if (string.Equals(channel, "production", StringComparison.OrdinalIgnoreCase) || string.Equals(channel, "live", StringComparison.OrdinalIgnoreCase))
        {
            return "LIVE";
        }

        return channel.Trim().ToLowerInvariant();
    }

    private static string NormalizeBinaryType(string binaryType)
    {
        if (string.IsNullOrWhiteSpace(binaryType))
        {
            return "WindowsPlayer";
        }

        return binaryType.Trim();
    }

    private static string NormalizeVersionHash(string versionHash)
    {
        if (string.IsNullOrWhiteSpace(versionHash))
        {
            return string.Empty;
        }

        string normalized = versionHash.Trim();

        if (!normalized.StartsWith("version-", StringComparison.OrdinalIgnoreCase))
        {
            normalized = "version-" + normalized;
        }

        return normalized;
    }

    private static string NormalizeBlobDirectory(string blobDirectory, string binaryType)
    {
        if (string.IsNullOrWhiteSpace(blobDirectory))
        {
            return GetDefaultBlobDirectory(binaryType);
        }

        string normalized = blobDirectory.Trim();

        if (!normalized.StartsWith("/", StringComparison.Ordinal))
        {
            normalized = "/" + normalized;
        }

        if (!normalized.EndsWith("/", StringComparison.Ordinal))
        {
            normalized += "/";
        }

        return normalized;
    }

    private static string GetDefaultBlobDirectory(string binaryType)
    {
        if (IsMacBinary(binaryType))
        {
            return "/mac/";
        }

        return "/";
    }

    private static string BuildChannelPath(string channel)
    {
        if (string.Equals(channel, "LIVE", StringComparison.OrdinalIgnoreCase))
        {
            return HostPath;
        }

        return HostPath + "/channel/" + channel;
    }

    private static bool IsMacBinary(string binaryType)
    {
        return string.Equals(binaryType, "MacPlayer", StringComparison.OrdinalIgnoreCase)
            || string.Equals(binaryType, "MacStudio", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsWindowsStudioBinary(string binaryType)
    {
        return string.Equals(binaryType, "WindowsStudio64", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsWindowsPlayerBinary(string binaryType)
    {
        return string.Equals(binaryType, "WindowsPlayer", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetMacZipFileName(string binaryType)
    {
        if (string.Equals(binaryType, "MacStudio", StringComparison.OrdinalIgnoreCase))
        {
            return "RobloxStudioApp.zip";
        }

        return "RobloxPlayer.zip";
    }

    private static List<string> ExtractPackageNames(string[] manifestLines)
    {
        List<string> packageNames = new List<string>();

        for (int index = 0; index < manifestLines.Length; index++)
        {
            string line = manifestLines[index].Trim();

            if (line.Length == 0)
            {
                continue;
            }

            if (line.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) && line.Contains("."))
            {
                packageNames.Add(line);
            }
        }

        return packageNames;
    }

    private static bool ContainsLine(string[] lines, string needle)
    {
        for (int index = 0; index < lines.Length; index++)
        {
            if (string.Equals(lines[index].Trim(), needle, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static string[] SplitLines(string text)
    {
        string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        for (int index = 0; index < lines.Length; index++)
        {
            lines[index] = lines[index].TrimStart('\uFEFF');
        }

        return lines;
    }

    private static async Task<Dictionary<string, byte[]>> DownloadPackagesInParallelAsync(string versionPath, List<string> packageNames, CancellationToken cancellationToken)
    {
        Dictionary<string, Task<byte[]>> downloadTasks = new Dictionary<string, Task<byte[]>>(StringComparer.OrdinalIgnoreCase);

        for (int index = 0; index < packageNames.Count; index++)
        {
            string packageName = packageNames[index];
            downloadTasks[packageName] = DownloadBytesAsync(versionPath + packageName, cancellationToken);
        }

        await Task.WhenAll(downloadTasks.Values);

        Dictionary<string, byte[]> result = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

        foreach (KeyValuePair<string, Task<byte[]>> pair in downloadTasks)
        {
            result[pair.Key] = pair.Value.Result;
        }

        return result;
    }

    private static async Task<Dictionary<string, byte[]>> DownloadPackagesSequentialAsync(string versionPath, List<string> packageNames, CancellationToken cancellationToken)
    {
        Dictionary<string, byte[]> result = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

        for (int index = 0; index < packageNames.Count; index++)
        {
            string packageName = packageNames[index];
            result[packageName] = await DownloadBytesAsync(versionPath + packageName, cancellationToken);
        }

        return result;
    }

    private static async Task<byte[]> DownloadBytesAsync(string url, CancellationToken cancellationToken)
    {
        using (HttpResponseMessage response = await HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
    }

    private static async Task<string> DownloadTextAsync(string url, CancellationToken cancellationToken)
    {
        using (HttpResponseMessage response = await HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }

    private static void AddExtractedPackageEntries(ZipArchive archive, byte[] packageBytes, string extractRootFolder, CompressionLevel compressionLevel, HashSet<string> addedEntries)
    {
        using (MemoryStream packageStream = new MemoryStream(packageBytes, false))
        using (ZipArchive packageArchive = new ZipArchive(packageStream, ZipArchiveMode.Read, false))
        {
            foreach (ZipArchiveEntry entry in packageArchive.Entries)
            {
                if (string.IsNullOrEmpty(entry.FullName) || entry.FullName.EndsWith("/", StringComparison.Ordinal))
                {
                    continue;
                }

                string normalizedPath = NormalizeZipPath(extractRootFolder + entry.FullName);

                if (addedEntries.Contains(normalizedPath))
                {
                    continue;
                }

                ZipArchiveEntry outputEntry = archive.CreateEntry(normalizedPath, compressionLevel);
                using (Stream inputStream = entry.Open())
                using (Stream outputStream = outputEntry.Open())
                {
                    inputStream.CopyTo(outputStream);
                }

                addedEntries.Add(normalizedPath);
            }
        }
    }

    private static void AddTextEntry(ZipArchive archive, string entryName, string content, CompressionLevel compressionLevel, HashSet<string> addedEntries)
    {
        AddBytesEntry(archive, entryName, Encoding.UTF8.GetBytes(content), compressionLevel, addedEntries);
    }

    private static void AddBytesEntry(ZipArchive archive, string entryName, byte[] content, CompressionLevel compressionLevel, HashSet<string> addedEntries)
    {
        string normalizedEntryName = NormalizeZipPath(entryName);

        if (addedEntries.Contains(normalizedEntryName))
        {
            return;
        }

        ZipArchiveEntry outputEntry = archive.CreateEntry(normalizedEntryName, compressionLevel);
        using (Stream outputStream = outputEntry.Open())
        {
            outputStream.Write(content, 0, content.Length);
        }

        addedEntries.Add(normalizedEntryName);
    }

    private static string NormalizeZipPath(string path)
    {
        string normalized = path.Replace('\\', '/');

        while (normalized.StartsWith("/", StringComparison.Ordinal))
        {
            normalized = normalized.Substring(1);
        }

        return normalized;
    }

    private static string ResolveOutputPath(RobloxRddDownloadOptions options, string channel, string binaryType, string versionHash)
    {
        if (!string.IsNullOrWhiteSpace(options.OutputZipPath))
        {
            return options.OutputZipPath;
        }

        string fileName = "WEAO-" + channel + "-" + binaryType + "-" + versionHash + ".zip";
        return Path.Combine(Directory.GetCurrentDirectory(), fileName);
    }

    private static void EnsureDirectoryForFile(string filePath)
    {
        string directory = Path.GetDirectoryName(filePath) ?? string.Empty;

        if (string.IsNullOrWhiteSpace(directory))
        {
            return;
        }

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    private static void WriteAllBytes(string filePath, byte[] data)
    {
        EnsureDirectoryForFile(filePath);
        File.WriteAllBytes(filePath, data);
    }

    private static string AppSettingsXml()
    {
        return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Settings>\n    <ContentFolder>content</ContentFolder>\n    <BaseUrl>http://www.roblox.com</BaseUrl>\n</Settings>\n";
    }

    private static CompressionLevel ClampCompressionLevel(int compressionLevel)
    {
        if (compressionLevel <= 3)
        {
            return CompressionLevel.Fastest;
        }

        return CompressionLevel.Optimal;
    }

    private static HttpClient CreateHttpClient()
    {
        HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(60);
        return client;
    }
}