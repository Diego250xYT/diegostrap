using System;
using System.Threading.Tasks;

// Obtiene perfil publico de Roblox usando username.
public class GetRobloxProfile
{
    public static async Task<RobloxUserProfile> GetByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("username is required.", nameof(username));
        }

        try
        {
            long userId = await GetRobloxUserId.ResolveUserIdAsync(username).ConfigureAwait(false);
            return await GetRobloxProfileByUserId.GetAsync(userId).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to fetch profile for username '{username}'.", ex);
        }
    }
}
