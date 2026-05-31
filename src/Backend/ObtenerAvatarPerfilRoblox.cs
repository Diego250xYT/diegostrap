using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

// Consulta la URL del avatar publico (headshot) de un usuario de Roblox.
public class ObtenerAvatarPerfilRoblox
{
    public static async Task<string> ObtenerUrlAvatarHeadshotAsync(long userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(userId), "userId must be greater than zero.");
        }

        try
        {
            string url = $"https://thumbnails.roblox.com/v1/users/avatar-headshot?userIds={userId}&size=420x420&format=Png&isCircular=false";

            HttpClient client = RobloxHttpClient.Instancia();
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement data = doc.RootElement.GetProperty("data");

                if (data.GetArrayLength() == 0)
                {
                    return string.Empty;
                }

                return data[0].GetProperty("imageUrl").GetString() ?? string.Empty;
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to fetch avatar for user ID {userId}.", ex);
        }
    }
}
