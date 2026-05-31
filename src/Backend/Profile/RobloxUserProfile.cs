// Modelo con datos publicos de perfil de usuario de Roblox.
public class RobloxUserProfile
{
    // Identificador numerico del usuario.
    public long UserId { get; set; }

    // Nombre de usuario de Roblox.
    public string Username { get; set; }

    // Nombre visible del usuario.
    public string DisplayName { get; set; }

    // URL publica de la imagen del avatar.
    public string AvatarImageUrl { get; set; }

    // Constructor por defecto para compatibilidad clasica.
    public RobloxUserProfile()
    {
        Username = string.Empty;
        DisplayName = string.Empty;
        AvatarImageUrl = string.Empty;
    }
}
