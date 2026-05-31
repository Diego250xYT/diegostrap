using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// Resuelve el userId de Roblox a partir de un username.
public class GetRobloxUserId
{
    public static async Task<long> ResolveUserIdAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("username is required.", nameof(username));
        }

        try
        {
            string[] usernames = new string[] { username };
            RobloxUsernamesRequest payloadObject = new RobloxUsernamesRequest
            {
                usernames = usernames,
                excludeBannedUsers = false
            };

            string payload = JsonSerializer.Serialize(payloadObject);
            StringContent content = new StringContent(payload, Encoding.UTF8, "application/json");

            HttpClient client = RobloxHttpClient.Instancia();
            HttpResponseMessage response = await client.PostAsync("https://users.roblox.com/v1/usernames/users", content).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement data = doc.RootElement.GetProperty("data");

                if (data.GetArrayLength() == 0)
                {
                    throw new InvalidOperationException("No Roblox user was found for the provided username.");
                }

                return data[0].GetProperty("id").GetInt64();
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to resolve user ID for username '{username}'.", ex);
        }
    }

    // Modelo de request para serializar body en forma explicita, sin tipos anonimos.
    private class RobloxUsernamesRequest
    {
        public string[] usernames { get; set; }
        public bool excludeBannedUsers { get; set; }
    }
}
