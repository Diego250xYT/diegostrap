using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

// Obtiene perfil publico de Roblox usando userId.
public class ObtenerPerfilPorUserIdRoblox
{
    public static async Task<RobloxUserProfile> ObtenerAsync(long userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(userId), "userId must be greater than zero.");
        }

        try
        {
            HttpClient client = RobloxHttpClient.Instancia();
            HttpResponseMessage response = await client.GetAsync($"https://users.roblox.com/v1/users/{userId}").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement root = doc.RootElement;
                string avatarUrl = await ObtenerAvatarPerfilRoblox.ObtenerUrlAvatarHeadshotAsync(userId).ConfigureAwait(false);

                return new RobloxUserProfile
                {
                    UserId = root.GetProperty("id").GetInt64(),
                    Username = root.GetProperty("name").GetString() ?? string.Empty,
                    DisplayName = root.GetProperty("displayName").GetString() ?? string.Empty,
                    AvatarImageUrl = avatarUrl
                };
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to fetch profile for user ID {userId}.", ex);
        }
    }
}
