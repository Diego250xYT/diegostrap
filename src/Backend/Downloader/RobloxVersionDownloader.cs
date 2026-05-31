using System;
using System.Threading;
using System.Threading.Tasks;

// Modulo principal base para el descargador de versiones de Roblox.
public static class RobloxVersionDownloader
{
    // Devuelve el canal de version principal que usara el descargador.
    public static string GetMainChannel()
    {
        return RobloxVersionChannel.Live();
    }

    // Devuelve la informacion real de la version principal.
    public static Task<RobloxVersionInfo> GetMainVersionInfoAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return RobloxVersionResolver.ResolveLiveAsync(cancellationToken);
    }

    // Mantiene una version sincronica para compatibilidad con llamadas antiguas.
    public static RobloxVersionInfo GetMainVersionInfo()
    {
        return GetMainVersionInfoAsync().GetAwaiter().GetResult();
    }

    // Devuelve el canal alternativo FUTURE.
    public static string GetFutureChannel()
    {
        return RobloxVersionChannel.Future();
    }

    // Devuelve la informacion real de la version FUTURE.
    public static Task<RobloxVersionInfo> GetFutureVersionInfoAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return RobloxVersionResolver.ResolveFutureAsync(cancellationToken);
    }

    // Mantiene una version sincronica para compatibilidad con llamadas antiguas.
    public static RobloxVersionInfo GetFutureVersionInfo()
    {
        return GetFutureVersionInfoAsync().GetAwaiter().GetResult();
    }

    // Devuelve el canal alternativo PAST.
    public static string GetPastChannel()
    {
        return RobloxVersionChannel.Past();
    }

    // Devuelve la informacion real de la version PAST.
    public static Task<RobloxVersionInfo> GetPastVersionInfoAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return RobloxVersionResolver.ResolvePastAsync(cancellationToken);
    }

    // Mantiene una version sincronica para compatibilidad con llamadas antiguas.
    public static RobloxVersionInfo GetPastVersionInfo()
    {
        return GetPastVersionInfoAsync().GetAwaiter().GetResult();
    }

    // Descarga la version mas reciente usando el flujo RDD.
    public static Task<RobloxRddDownloadResult> DownloadLatestAsync(RobloxRddDownloadOptions options, CancellationToken cancellationToken = default(CancellationToken))
    {
        return RobloxRddDownloader.DownloadLatestAsync(options, cancellationToken);
    }

    // Descarga la version anterior usando el flujo RDD.
    public static Task<RobloxRddDownloadResult> DownloadPreviousAsync(RobloxRddDownloadOptions options, CancellationToken cancellationToken = default(CancellationToken))
    {
        return RobloxRddDownloader.DownloadPreviousAsync(options, cancellationToken);
    }

    // Descarga una version especifica usando el flujo RDD.
    public static Task<RobloxRddDownloadResult> DownloadSpecifiedAsync(RobloxRddDownloadOptions options, CancellationToken cancellationToken = default(CancellationToken))
    {
        return RobloxRddDownloader.DownloadSpecifiedAsync(options, cancellationToken);
    }

    // Genera un enlace directo equivalente al que construye RDD.
    public static string BuildDownloadLink(RobloxRddDownloadOptions options)
    {
        return RobloxRddDownloader.BuildDownloadLink(options);
    }

    // Placeholder para validar que el modulo ya existe sin descargar nada todavia.
    public static void EnsureReady()
    {
        if (string.IsNullOrWhiteSpace(GetMainChannel()))
        {
            throw new InvalidOperationException("Main version channel is not configured.");
        }
    }
}
