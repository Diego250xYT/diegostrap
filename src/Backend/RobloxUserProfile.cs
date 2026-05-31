// Modelo con datos publicos de perfil de usuario de Roblox.
public class RobloxUserProfile
{
    public long UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string AvatarImageUrl { get; set; } = string.Empty;
}
