using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Tecaverso.Networking
{
    /// <summary>LAN endpoints without DNS queries or an Internet connectivity probe.</summary>
    public static class LanAddress
    {
        public const ushort DefaultPort = 7777;

        public static bool TryNormalize(string value, out string address)
        {
            address = null;
            var parts = (value ?? string.Empty).Trim().Split('.');
            if (parts.Length != 4) return false;
            var bytes = new byte[4];
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Length == 0 || parts[i].Length > 3) return false;
                foreach (char c in parts[i]) if (c < '0' || c > '9') return false;
                if (!byte.TryParse(parts[i], out bytes[i])) return false;
            }
            bool local = bytes[0] == 10 || bytes[0] == 127 ||
                (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) ||
                (bytes[0] == 192 && bytes[1] == 168) ||
                (bytes[0] == 169 && bytes[1] == 254);
            if (!local) return false;
            address = new IPAddress(bytes).ToString();
            return true;
        }

        public static string[] GetLocalAddresses()
        {
            var addresses = new List<string>();
            try
            {
                foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (adapter.OperationalStatus != OperationalStatus.Up ||
                        adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;
                    foreach (var unicast in adapter.GetIPProperties().UnicastAddresses)
                    {
                        var ip = unicast.Address;
                        if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip) &&
                            TryNormalize(ip.ToString(), out var address) && !addresses.Contains(address))
                            addresses.Add(address);
                    }
                }
            }
            catch (NetworkInformationException) { }
            catch (PlatformNotSupportedException) { }
            return addresses.ToArray();
        }
    }
}
