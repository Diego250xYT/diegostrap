using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

// Cliente minimo para consultar las versiones de Roblox expuestas por WEAO.
public static class WeaoRobloxVersionClient
{
    // WEAO publica los hashes actuales y futuros/pasados en este grupo de endpoints.
    private const string VersionsBaseUrl = "https://weao.xyz/api/versions/";

    // La API de WEAO exige este user-agent.
    private const string RequiredUserAgent = "WEAO-3PService";

    private static readonly HttpClient HttpClient = CreateHttpClient();

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    private sealed class WeaoVersionResponse
    {
        public string Windows { get; set; }

        public string WindowsDate { get; set; }

        public string Mac { get; set; }

        public string MacDate { get; set; }

        public string Android { get; set; }

        public string AndroidDate { get; set; }

        public string IOS { get; set; }

        public string IOSDate { get; set; }

        public string WindowsPlayer { get; set; }

        public string WindowsPlayerDate { get; set; }

        public WeaoVersionResponse()
        {
            Windows = string.Empty;
            WindowsDate = string.Empty;
            Mac = string.Empty;
            MacDate = string.Empty;
            Android = string.Empty;
            AndroidDate = string.Empty;
            IOS = string.Empty;
            IOSDate = string.Empty;
            WindowsPlayer = string.Empty;
            WindowsPlayerDate = string.Empty;
        }
    }

    private sealed class WeaoExploitResponse
    {
        public string Title { get; set; }

        public string RbxVersion { get; set; }

        public string Platform { get; set; }

        public bool Hidden { get; set; }

        public WeaoExploitResponse()
        {
            Title = string.Empty;
            RbxVersion = string.Empty;
            Platform = string.Empty;
            Hidden = false;
        }
    }

    // Devuelve la version LIVE actual.
    public static Task<RobloxVersionInfo> GetLiveAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return GetVersionAsync("current", "WindowsPlayer", RobloxVersionChannel.Live(), false, cancellationToken);
    }

    // Devuelve la version LIVE actual para un tipo de binario concreto.
    public static Task<RobloxVersionInfo> GetLiveAsync(string binaryType, CancellationToken cancellationToken)
    {
        return GetVersionAsync("current", binaryType, RobloxVersionChannel.Live(), false, cancellationToken);
    }

    // Devuelve la siguiente version publicada por WEAO.
    public static Task<RobloxVersionInfo> GetFutureAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return GetVersionAsync("future", "WindowsPlayer", RobloxVersionChannel.Future(), false, cancellationToken);
    }

    // Devuelve la siguiente version publicada por WEAO para un tipo de binario concreto.
    public static Task<RobloxVersionInfo> GetFutureAsync(string binaryType, CancellationToken cancellationToken)
    {
        return GetVersionAsync("future", binaryType, RobloxVersionChannel.Future(), false, cancellationToken);
    }

    // Devuelve la version anterior que WEAO expone para downgrade.
    public static Task<RobloxVersionInfo> GetPastAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return GetVersionAsync("past", "WindowsPlayer", RobloxVersionChannel.Past(), true, cancellationToken);
    }

    // Devuelve la version anterior que WEAO expone para downgrade, para un binario concreto.
    public static Task<RobloxVersionInfo> GetPastAsync(string binaryType, CancellationToken cancellationToken)
    {
        return GetVersionAsync("past", binaryType, RobloxVersionChannel.Past(), true, cancellationToken);
    }

    // Busca la version hash asociada a un ejecutor en WEAO.
    public static async Task<string> GetExploitVersionAsync(string exploitTitle, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (string.IsNullOrWhiteSpace(exploitTitle))
        {
            throw new ArgumentException("Exploit title cannot be empty.", nameof(exploitTitle));
        }

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://weao.xyz/api/status/exploits");
        request.Headers.UserAgent.ParseAdd(RequiredUserAgent);

        using (request)
        using (HttpResponseMessage response = await HttpClient.SendAsync(request, cancellationToken))
        {
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            List<WeaoExploitResponse> payload = JsonSerializer.Deserialize<List<WeaoExploitResponse>>(json, JsonOptions);

            if (payload == null)
            {
                throw new InvalidOperationException("WEAO returned an invalid exploit list.");
            }

            int index = payload.FindIndex(delegate (WeaoExploitResponse item)
            {
                return item != null && string.Equals(item.Title, exploitTitle, StringComparison.OrdinalIgnoreCase);
            });

            if (index < 0)
            {
                throw new InvalidOperationException("WEAO did not return a matching exploit title.");
            }

            string version = payload[index].RbxVersion;

            if (string.IsNullOrWhiteSpace(version))
            {
                throw new InvalidOperationException("WEAO returned an empty Roblox version for the requested exploit.");
            }

            return version.Trim();
        }
    }

    private static async Task<RobloxVersionInfo> GetVersionAsync(string endpoint, string binaryType, string channel, bool isBehindLive, CancellationToken cancellationToken)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, VersionsBaseUrl + endpoint);
        request.Headers.UserAgent.ParseAdd(RequiredUserAgent);

        using (request)
        using (HttpResponseMessage response = await HttpClient.SendAsync(request, cancellationToken))
        {
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            WeaoVersionResponse payload = JsonSerializer.Deserialize<WeaoVersionResponse>(json, JsonOptions);

            if (payload == null)
            {
                throw new InvalidOperationException("WEAO returned an invalid Roblox version response.");
            }

            string versionGuid = SelectVersionGuid(binaryType, payload);

            if (string.IsNullOrWhiteSpace(versionGuid))
            {
                throw new InvalidOperationException("WEAO returned an empty Roblox version hash.");
            }

            RobloxVersionInfo info = new RobloxVersionInfo();
            info.Channel = channel.ToUpperInvariant();
            info.VersionGuid = versionGuid;
            info.Version = versionGuid;
            info.IsBehindLive = isBehindLive;
            info.ReleaseDateUtc = ParseDate(SelectVersionDate(binaryType, payload));
            return info;
        }
    }

    private static string SelectVersionGuid(string binaryType, WeaoVersionResponse payload)
    {
        string normalizedBinaryType = NormalizeBinaryType(binaryType);

        if (normalizedBinaryType == "Windows")
        {
            if (!string.IsNullOrWhiteSpace(payload.WindowsPlayer))
            {
                return payload.WindowsPlayer.Trim();
            }

            if (!string.IsNullOrWhiteSpace(payload.Windows))
            {
                return payload.Windows.Trim();
            }
        }
        else if (normalizedBinaryType == "Mac")
        {
            if (!string.IsNullOrWhiteSpace(payload.Mac))
            {
                return payload.Mac.Trim();
            }
        }
        else if (normalizedBinaryType == "Android")
        {
            if (!string.IsNullOrWhiteSpace(payload.Android))
            {
                return payload.Android.Trim();
            }
        }
        else if (normalizedBinaryType == "iOS")
        {
            if (!string.IsNullOrWhiteSpace(payload.IOS))
            {
                return payload.IOS.Trim();
            }
        }

        if (!string.IsNullOrWhiteSpace(payload.WindowsPlayer))
        {
            return payload.WindowsPlayer.Trim();
        }

        if (!string.IsNullOrWhiteSpace(payload.Windows))
        {
            return payload.Windows.Trim();
        }

        return string.Empty;
    }

    private static string SelectVersionDate(string binaryType, WeaoVersionResponse payload)
    {
        string normalizedBinaryType = NormalizeBinaryType(binaryType);

        if (normalizedBinaryType == "Windows")
        {
            if (!string.IsNullOrWhiteSpace(payload.WindowsPlayerDate))
            {
                return payload.WindowsPlayerDate.Trim();
            }

            if (!string.IsNullOrWhiteSpace(payload.WindowsDate))
            {
                return payload.WindowsDate.Trim();
            }
        }
        else if (normalizedBinaryType == "Mac")
        {
            if (!string.IsNullOrWhiteSpace(payload.MacDate))
            {
                return payload.MacDate.Trim();
            }
        }
        else if (normalizedBinaryType == "Android")
        {
            if (!string.IsNullOrWhiteSpace(payload.AndroidDate))
            {
                return payload.AndroidDate.Trim();
            }
        }
        else if (normalizedBinaryType == "iOS")
        {
            if (!string.IsNullOrWhiteSpace(payload.IOSDate))
            {
                return payload.IOSDate.Trim();
            }
        }

        if (!string.IsNullOrWhiteSpace(payload.WindowsPlayerDate))
        {
            return payload.WindowsPlayerDate.Trim();
        }

        if (!string.IsNullOrWhiteSpace(payload.WindowsDate))
        {
            return payload.WindowsDate.Trim();
        }

        return string.Empty;
    }

    private static string NormalizeBinaryType(string binaryType)
    {
        if (string.Equals(binaryType, "WindowsStudio64", StringComparison.OrdinalIgnoreCase) || string.Equals(binaryType, "WindowsPlayer", StringComparison.OrdinalIgnoreCase))
        {
            return "Windows";
        }

        if (string.Equals(binaryType, "MacPlayer", StringComparison.OrdinalIgnoreCase) || string.Equals(binaryType, "MacStudio", StringComparison.OrdinalIgnoreCase))
        {
            return "Mac";
        }

        if (string.Equals(binaryType, "Android", StringComparison.OrdinalIgnoreCase))
        {
            return "Android";
        }

        if (string.Equals(binaryType, "iOS", StringComparison.OrdinalIgnoreCase))
        {
            return "iOS";
        }

        return binaryType;
    }

    private static DateTime? ParseDate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        string normalized = value.Replace(" at ", " ");

        DateTime result;
        if (DateTime.TryParse(normalized, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out result))
        {
            return result;
        }

        return null;
    }

    private static HttpClient CreateHttpClient()
    {
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd(RequiredUserAgent);
        client.Timeout = TimeSpan.FromSeconds(20);
        return client;
    }
}