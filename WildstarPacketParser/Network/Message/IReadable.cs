namespace WildstarPacketParser.Network.Message;

public interface IReadable
{
    void Read(Packet reader, string idx);
}
