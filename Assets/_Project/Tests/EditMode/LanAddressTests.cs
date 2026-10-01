using NUnit.Framework;
using Tecaverso.Networking;

public class LanAddressTests
{
    [TestCase("192.168.1.10", "192.168.1.10")]
    [TestCase(" 10.0.0.2 ", "10.0.0.2")]
    [TestCase("172.16.0.5", "172.16.0.5")]
    [TestCase("172.31.255.1", "172.31.255.1")]
    [TestCase("127.0.0.1", "127.0.0.1")]
    [TestCase("169.254.10.12", "169.254.10.12")]
    public void AcceptsLocalIPv4(string input, string expected)
    {
        Assert.That(LanAddress.TryNormalize(input, out var address), Is.True);
        Assert.That(address, Is.EqualTo(expected));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("192.168.1")]
    [TestCase("192.168.1.256")]
    [TestCase("192.168.1.-1")]
    [TestCase("localhost")]
    [TestCase("8.8.8.8")]
    [TestCase("172.32.0.1")]
    [TestCase("0.0.0.0")]
    [TestCase("255.255.255.255")]
    [TestCase("::1")]
    [TestCase("192.168.1.10:7777")]
    public void RejectsInvalidOrNonLocalEndpoints(string input)
    {
        Assert.That(LanAddress.TryNormalize(input, out var address), Is.False);
        Assert.That(address, Is.Null);
    }
}
