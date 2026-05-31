using System;

// Representa la informacion minima de una version de Roblox.
public class RobloxVersionInfo
{
    // Canal asociado a la version, por ejemplo LIVE, FUTURE o PAST.
    public string Channel { get; set; }

    // Identificador de version que usa Roblox para descargar paquetes.
    public string VersionGuid { get; set; }

    // Numero de version visible para el usuario.
    public string Version { get; set; }

    // Indica si la version fue detectada como anterior al canal principal.
    public bool IsBehindLive { get; set; }

    // Fecha aproximada de la version segun WEAO, cuando este dato esta disponible.
    public DateTime? ReleaseDateUtc { get; set; }

    // Constructor por defecto para compatibilidad clasica.
    public RobloxVersionInfo()
    {
        Channel = string.Empty;
        VersionGuid = string.Empty;
        Version = string.Empty;
        IsBehindLive = false;
        ReleaseDateUtc = null;
    }
}
