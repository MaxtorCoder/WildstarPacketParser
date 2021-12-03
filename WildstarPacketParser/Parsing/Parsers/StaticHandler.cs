using WildstarPacketParser.Network;

namespace WildstarPacketParser.Parsing.Parsers;

public static class StaticHandler
{
    public static void ReadTargetIdentity(Packet packet, params object[] idx)
    {
        packet.ReadUShort("RealmId", 14u, idx);
        packet.ReadULong("CharacterId", 64u, idx);
    }

    public static void ReadMoveData(Packet packet, params object[] idx)
    {
        packet.ReadUShort("X", 16, idx);
        packet.ReadUShort("Y", 16, idx);
        packet.ReadUShort("Z", 16, idx);
    }

    public static void ReadVector3(Packet packet, params object[] idx)
    {
        packet.ReadSingle("X", 32, idx);
        packet.ReadSingle("Y", 32, idx);
        packet.ReadSingle("Z", 32, idx);
    }
}
