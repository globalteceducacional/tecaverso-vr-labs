using System;
using System.Linq;

namespace Tecaverso.Networking
{
    /// <summary>Small, versioned LAN contract. No cloud identity or arbitrary scene paths.</summary>
    public static class LanLobbyProtocol
    {
        public const int Version = 1;
        public const int Capacity = 8;
        public const string Full = "ROOM_FULL", Incompatible = "VERSION_MISMATCH", Invalid = "INVALID_JOIN";

        public static string CleanName(string value, int limit = 24)
        {
            return new string((value ?? "").Trim().Where(c => !char.IsControl(c) && c != '<' && c != '>').Take(limit).ToArray());
        }

        public static string Validate(JoinRequest request, string applicationVersion, int occupied)
        {
            if (request == null || string.IsNullOrWhiteSpace(CleanName(request.name))) return Invalid;
            if (request.protocol != Version || request.version != applicationVersion) return Incompatible;
            return occupied >= Capacity ? Full : null;
        }

        [Serializable] public sealed class JoinRequest { public int protocol = Version; public string version, name; }
        [Serializable] public sealed class Participant { public ulong id; public string name; public bool ready, host, labReady; }
        [Serializable] public sealed class Snapshot
        {
            public string room, contentId, contentTitle;
            public Participant[] participants = Array.Empty<Participant>();
            public bool supportsMultiplayer;
            public bool experimentActive;
        }
    }
}
