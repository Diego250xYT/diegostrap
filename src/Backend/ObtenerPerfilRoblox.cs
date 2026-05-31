using System;
using System.Threading.Tasks;

// Obtiene perfil publico de Roblox usando username.
public class ObtenerPerfilRoblox
{
    public static async Task<RobloxUserProfile> DesdeUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("username is required.", nameof(username));
        }

        try
        {
            long userId = await ObtenerUsuarioIdRoblox.DesdeUsernameAsync(username).ConfigureAwait(false);
            return await ObtenerPerfilPorUserIdRoblox.ObtenerAsync(userId).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to fetch profile for username '{username}'.", ex);
        }
    }
}
