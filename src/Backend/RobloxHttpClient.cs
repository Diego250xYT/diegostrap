using System;
using System.Net.Http;

// Provee un HttpClient compartido para llamadas a APIs publicas de Roblox.
public static class RobloxHttpClient
{
    private static readonly HttpClient client = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(20)
    };

    public static HttpClient Instancia()
    {
        return client;
    }
}
