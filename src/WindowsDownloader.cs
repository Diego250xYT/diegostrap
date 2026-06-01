using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiegoStrap
{
    internal sealed class WindowsDownloader
    {
        private const string HostPath = "https://setup-aws.rbxcdn.com";
        private const string WEAOApiBase = "https://weao.xyz/api/versions/";
        private const string LauncherUrl = "https://curly-shape-1578.vnnaworks.workers.dev/";
        private const double BytesPerMegabyte = 1024d * 1024d;

        private static readonly HttpClient Http = CreateHttpClient();

        public async Task<string> DownloadAsync(DownloadRequest request, string outputPath, CancellationToken cancellationToken, IProgress<DownloadProgressInfo>? progress = null)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            WindowsBinaryTypeInfo binaryType = WindowsBinaryCatalog.Resolve(request.BinaryType);
            string channelPath = BuildChannelPath(request.Channel);
            string normalizedVersion = await ResolveVersionAsync(request, binaryType, cancellationToken).ConfigureAwait(false);
            string versionPath = BuildVersionPath(channelPath, binaryType.BlobDir, normalizedVersion);

            if (request.UseVersionHash && string.IsNullOrWhiteSpace(request.VersionHash))
            {
                throw new InvalidDataException("Version hash mode was selected, but no hash was provided.");
            }

            progress?.Report(new DownloadProgressInfo
            {
                Percent = 0,
                StatusText = "Fetching manifest..."
            });

            string manifestUrl = versionPath + "rbxPkgManifest.txt";
            string manifestText = await DownloadStringAsync(manifestUrl, cancellationToken, progress, "Manifest", 0, 1).ConfigureAwait(false);
            string[] manifestLines = manifestText.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

            if (manifestLines.Length == 0 || manifestLines[0].Trim() != "v0")
            {
                throw new InvalidDataException("Unsupported manifest format.");
            }

            List<string> packages = new List<string>();
            for (int i = 0; i < manifestLines.Length; i++)
            {
                string line = manifestLines[i].Trim();
                if (line.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    packages.Add(line);
                }
            }

            progress?.Report(new DownloadProgressInfo
            {
                Percent = 5,
                StatusText = "Packages found: " + packages.Count
            });

            using (FileStream outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            using (ZipArchive outputArchive = new ZipArchive(outputStream, ZipArchiveMode.Create, leaveOpen: false, entryNameEncoding: Encoding.UTF8))
            {
                AddAppSettings(outputArchive);

                for (int i = 0; i < packages.Count; i++)
                {
                    string packageName = packages[i];
                    progress?.Report(new DownloadProgressInfo
                    {
                        Percent = 5 + (int)((double)i / Math.Max(packages.Count, 1) * 90.0),
                        StatusText = "Downloading " + packageName + " (" + (i + 1) + "/" + packages.Count + ")"
                    });

                    byte[] packageBytes = await DownloadBytesAsync(versionPath + packageName, cancellationToken, progress, packageName, i + 1, packages.Count).ConfigureAwait(false);
                    string extractionRoot = WindowsPackageExtractionMap.GetRoot(binaryType.Name, packageName);

                    await AddPackageToArchiveAsync(outputArchive, packageBytes, packageName, extractionRoot, cancellationToken).ConfigureAwait(false);
                }

                progress?.Report(new DownloadProgressInfo
                {
                    Percent = 95,
                    StatusText = "Downloading launcher..."
                });

                byte[] launcherBytes = await DownloadBytesAsync(LauncherUrl, cancellationToken, progress, "Launcher", packages.Count + 1, packages.Count + 1).ConfigureAwait(false);
                AddFileToArchive(outputArchive, "weblauncher.exe", launcherBytes);
            }

            progress?.Report(new DownloadProgressInfo
            {
                Percent = 100,
                StatusText = "Finalizing archive..."
            });

            return normalizedVersion;
        }

        private static async Task<string> ResolveVersionAsync(DownloadRequest request, WindowsBinaryTypeInfo binaryType, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(request.VersionHash))
            {
                return NormalizeVersionHash(request.VersionHash);
            }

            string endpoint = request.VersionSource == VersionSource.Future ? "future" : request.VersionSource == VersionSource.Past ? "past" : "current";
            string apiUrl = WEAOApiBase + endpoint;
            string json = await DownloadStringAsync(apiUrl, cancellationToken).ConfigureAwait(false);
            string version = JsonFieldReader.ReadStringValue(json, "Windows");

            if (string.IsNullOrWhiteSpace(version))
            {
                throw new InvalidDataException("Could not resolve a version from the WEAO API.");
            }

            return NormalizeVersionHash(version);
        }

        private static async Task AddPackageToArchiveAsync(ZipArchive outputArchive, byte[] packageBytes, string packageName, string extractionRoot, CancellationToken cancellationToken)
        {
            using (MemoryStream packageStream = new MemoryStream(packageBytes, writable: false))
            using (ZipArchive packageArchive = new ZipArchive(packageStream, ZipArchiveMode.Read, leaveOpen: false, entryNameEncoding: Encoding.UTF8))
            {
                foreach (ZipArchiveEntry entry in packageArchive.Entries)
                {
                    if (string.IsNullOrEmpty(entry.Name) && entry.FullName.EndsWith("/", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    string outputName = string.IsNullOrWhiteSpace(extractionRoot) ? entry.FullName.Replace('\\', '/') : extractionRoot + entry.FullName.Replace('\\', '/');
                    ZipArchiveEntry outputEntry = outputArchive.CreateEntry(outputName, CompressionLevel.NoCompression);

                    using (Stream inputStream = entry.Open())
                    using (Stream outputEntryStream = outputEntry.Open())
                    {
                        await inputStream.CopyToAsync(outputEntryStream, 81920, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
        }

        private static void AddAppSettings(ZipArchive outputArchive)
        {
            string content = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<Settings>\r\n  <ContentFolder>content</ContentFolder>\r\n  <BaseUrl>http://www.roblox.com</BaseUrl>\r\n</Settings>\r\n";
            AddFileToArchive(outputArchive, "AppSettings.xml", Encoding.UTF8.GetBytes(content));
        }

        private static void AddFileToArchive(ZipArchive outputArchive, string fileName, byte[] data)
        {
            ZipArchiveEntry entry = outputArchive.CreateEntry(fileName, CompressionLevel.NoCompression);
            using (Stream entryStream = entry.Open())
            {
                entryStream.Write(data, 0, data.Length);
            }
        }

        private static string BuildChannelPath(string channel)
        {
            if (string.Equals(channel, "LIVE", StringComparison.OrdinalIgnoreCase))
            {
                return HostPath;
            }

            return HostPath + "/channel/" + channel;
        }

        private static string BuildVersionPath(string channelPath, string blobDir, string versionHash)
        {
            string normalizedBlobDir = blobDir;
            if (string.IsNullOrWhiteSpace(normalizedBlobDir))
            {
                normalizedBlobDir = "/";
            }

            if (!normalizedBlobDir.StartsWith("/", StringComparison.Ordinal))
            {
                normalizedBlobDir = "/" + normalizedBlobDir;
            }

            if (!normalizedBlobDir.EndsWith("/", StringComparison.Ordinal))
            {
                normalizedBlobDir += "/";
            }

            return channelPath + normalizedBlobDir + versionHash + "-";
        }

        private static string NormalizeVersionHash(string versionHash)
        {
            string value = versionHash.Trim().ToLowerInvariant();
            if (!value.StartsWith("version-", StringComparison.OrdinalIgnoreCase))
            {
                value = "version-" + value;
            }

            return value;
        }

        private static async Task<string> DownloadStringAsync(string url, CancellationToken cancellationToken, IProgress<DownloadProgressInfo>? progress = null, string? label = null, int currentIndex = 0, int totalCount = 0)
        {
            byte[] bytes = await DownloadBytesAsync(url, cancellationToken, progress, label, currentIndex, totalCount).ConfigureAwait(false);
            return Encoding.UTF8.GetString(bytes);
        }

        private static async Task<byte[]> DownloadBytesAsync(string url, CancellationToken cancellationToken, IProgress<DownloadProgressInfo>? progress = null, string? progressLabel = null, int currentIndex = 0, int totalCount = 0)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            using (HttpResponseMessage response = await Http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                using (Stream responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
                using (MemoryStream buffer = new MemoryStream())
                {
                    long? totalBytes = response.Content.Headers.ContentLength;
                    byte[] chunk = new byte[81920];
                    int read;
                    long loaded = 0;
                    DateTime startTime = DateTime.UtcNow;

                    while ((read = await responseStream.ReadAsync(chunk, 0, chunk.Length, cancellationToken).ConfigureAwait(false)) > 0)
                    {
                        await buffer.WriteAsync(chunk, 0, read, cancellationToken).ConfigureAwait(false);
                        loaded += read;
                        if (totalBytes.HasValue)
                        {
                            int overallPercent = totalCount > 0 ? (int)Math.Round(((currentIndex - 1) + ((double)loaded / totalBytes.Value)) * 100.0 / totalCount) : (int)Math.Round((double)loaded * 100.0 / totalBytes.Value);
                            double seconds = Math.Max((DateTime.UtcNow - startTime).TotalSeconds, 0.001);
                            double megabytesPerSecond = (loaded / BytesPerMegabyte) / seconds;
                            progress?.Report(new DownloadProgressInfo
                            {
                                Percent = Math.Max(0, Math.Min(100, overallPercent)),
                                StatusText = (progressLabel ?? "Downloading") + ": " + FormatSize(loaded) + " / " + FormatSize(totalBytes.Value),
                                SpeedText = megabytesPerSecond.ToString("0.00") + " MB/s"
                            });
                        }
                    }

                    if (totalBytes.HasValue)
                    {
                        int overallPercent = totalCount > 0 ? (int)Math.Round(((double)currentIndex / totalCount) * 100.0) : 100;
                        double seconds = Math.Max((DateTime.UtcNow - startTime).TotalSeconds, 0.001);
                        double megabytesPerSecond = (loaded / BytesPerMegabyte) / seconds;
                        progress?.Report(new DownloadProgressInfo
                        {
                            Percent = Math.Max(0, Math.Min(100, overallPercent)),
                            StatusText = (progressLabel ?? "Downloading") + " complete",
                            SpeedText = megabytesPerSecond.ToString("0.00") + " MB/s"
                        });
                    }

                    return buffer.ToArray();
                }
            }
        }

        private static HttpClient CreateHttpClient()
        {
            HttpClientHandler handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler, disposeHandler: true);
            client.Timeout = TimeSpan.FromMinutes(30);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
            client.DefaultRequestHeaders.UserAgent.ParseAdd("WEAO-3PService");
            return client;
        }

        private static string FormatSize(long bytes)
        {
            double value = bytes;
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int index = 0;

            while (value >= 1024 && index < suffixes.Length - 1)
            {
                value /= 1024;
                index++;
            }

            return value.ToString("0.##") + " " + suffixes[index];
        }
    }
}