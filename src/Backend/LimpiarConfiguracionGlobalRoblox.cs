using System;

// Elimina el archivo de configuracion global de Roblox si existe.
public class LimpiarConfiguracionGlobalRoblox
{
    public static void Limpiar()
    {
        try
        {
            RobloxUtilidadesSistema.ValidarRobloxCerrado();
            RobloxUtilidadesSistema.EliminarArchivoSiExiste(RobloxRutas.ConfiguracionGlobal());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to delete Roblox global settings file.", ex);
        }
    }
}
