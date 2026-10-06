using NUnit.Framework;
using Tecaverso.Networking;

public class LanLobbyProtocolTests
{
    [Test]
    public void ApprovalValidatesNameVersionAndCapacity()
    {
        var request=new LanLobbyProtocol.JoinRequest { name="Aluno",version="test" };
        Assert.IsNull(LanLobbyProtocol.Validate(request,"test",7));
        Assert.AreEqual(LanLobbyProtocol.Full,LanLobbyProtocol.Validate(request,"test",8));
        Assert.AreEqual(LanLobbyProtocol.Incompatible,LanLobbyProtocol.Validate(request,"other",0));
        request.protocol++;
        Assert.AreEqual(LanLobbyProtocol.Incompatible,LanLobbyProtocol.Validate(request,"test",0));
        Assert.AreEqual(LanLobbyProtocol.Invalid,LanLobbyProtocol.Validate(null,"test",0));
        request.name=" \n ";
        Assert.AreEqual(LanLobbyProtocol.Invalid,LanLobbyProtocol.Validate(request,"test",0));
    }
    [Test]
    public void NamesAreBoundedAndCannotInjectTmpMarkup()
    {
        Assert.AreEqual("bAna/b",LanLobbyProtocol.CleanName("  <b>Ana</b>\n"));
        Assert.AreEqual(24,LanLobbyProtocol.CleanName(new string('a',100)).Length);
    }
}
