using System;

// Borra contenido interno de la carpeta logs sin borrar la carpeta.
public class VaciarLogsRoblox
{
    public static void Limpiar()
    {
        try
        {
            RobloxUtilidadesSistema.ValidarRobloxCerrado();
            RobloxUtilidadesSistema.LimpiarContenidoDirectorio(RobloxRutas.Logs());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to clean Roblox logs content.", ex);
        }
    }
}
