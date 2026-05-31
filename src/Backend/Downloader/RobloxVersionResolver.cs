using System;
using System.Threading;
using System.Threading.Tasks;

// Resuelve los canales LIVE, FUTURE y PAST contra WEAO.
public static class RobloxVersionResolver
{
    // Devuelve el canal LIVE.
    public static Task<RobloxVersionInfo> ResolveLiveAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return WeaoRobloxVersionClient.GetLiveAsync(cancellationToken);
    }

    // Devuelve el canal FUTURE.
    public static Task<RobloxVersionInfo> ResolveFutureAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return WeaoRobloxVersionClient.GetFutureAsync(cancellationToken);
    }

    // Devuelve el canal PAST.
    public static Task<RobloxVersionInfo> ResolvePastAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return WeaoRobloxVersionClient.GetPastAsync(cancellationToken);
    }

    // Resuelve un canal por nombre.
    public static Task<RobloxVersionInfo> ResolveAsync(string channel, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (string.IsNullOrWhiteSpace(channel))
        {
            throw new ArgumentException("Channel cannot be empty.", nameof(channel));
        }

        if (string.Equals(channel, RobloxVersionChannel.Live(), StringComparison.OrdinalIgnoreCase))
        {
            return ResolveLiveAsync(cancellationToken);
        }

        if (string.Equals(channel, RobloxVersionChannel.Future(), StringComparison.OrdinalIgnoreCase))
        {
            return ResolveFutureAsync(cancellationToken);
        }

        if (string.Equals(channel, RobloxVersionChannel.Past(), StringComparison.OrdinalIgnoreCase))
        {
            return ResolvePastAsync(cancellationToken);
        }

        throw new ArgumentException("Unsupported version channel.", nameof(channel));
    }

    // Indica si un canal es valido para el modulo.
    public static bool IsSupportedChannel(string channel)
    {
        if (string.IsNullOrWhiteSpace(channel))
        {
            return false;
        }

        return string.Equals(channel, RobloxVersionChannel.Live(), StringComparison.OrdinalIgnoreCase)
            || string.Equals(channel, RobloxVersionChannel.Future(), StringComparison.OrdinalIgnoreCase)
            || string.Equals(channel, RobloxVersionChannel.Past(), StringComparison.OrdinalIgnoreCase);
    }

    // Crea una estructura base de version sin descargar nada todavia.
    public static RobloxVersionInfo CreateVersionInfo(string channel)
    {
        if (!IsSupportedChannel(channel))
        {
            throw new ArgumentException("Unsupported version channel.", nameof(channel));
        }

        RobloxVersionInfo info = new RobloxVersionInfo();
        info.Channel = channel.ToUpperInvariant();
        info.VersionGuid = string.Empty;
        info.Version = string.Empty;
        info.IsBehindLive = string.Equals(channel, RobloxVersionChannel.Past(), StringComparison.OrdinalIgnoreCase);
        info.ReleaseDateUtc = null;
        return info;
    }
}
