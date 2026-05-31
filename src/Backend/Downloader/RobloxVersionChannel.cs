// Define los canales de version que usara el futuro descargador.
public static class RobloxVersionChannel
{
    // Devuelve el canal LIVE.
    public static string Live()
    {
        return "LIVE";
    }

    // Devuelve el canal FUTURE.
    public static string Future()
    {
        return "FUTURE";
    }

    // Devuelve el canal PAST.
    public static string Past()
    {
        return "PAST";
    }

    // Devuelve los tres canales base en orden.
    public static string[] GetAll()
    {
        return new string[] { Live(), Future(), Past() };
    }
}
