namespace DiegoStrap
{
    internal sealed class ProtocolHandlerState
    {
        public string ProtocolName { get; set; } = string.Empty;

        public bool Exists { get; set; }

        public string DefaultValue { get; set; } = string.Empty;

        public string Command { get; set; } = string.Empty;

        public string DefaultIcon { get; set; } = string.Empty;

        public bool IsOwnedByCurrentLauncher { get; set; }

        public bool IsOwnedByOfficialRoblox { get; set; }

        public string OwnerDescription { get; set; } = string.Empty;
    }
}