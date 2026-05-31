using System;

// Mantiene compatibilidad con el modulo legado de limpieza de logs.
public class LimpiarLogs
{
    public static void Limpiar()
    {
        try
        {
            VaciarLogsRoblox.Limpiar();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to clean Roblox logs.", ex);
        }
    }
}
