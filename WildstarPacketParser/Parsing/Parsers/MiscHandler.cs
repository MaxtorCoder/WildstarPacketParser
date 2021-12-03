using WildstarPacketParser.Network;
using WildstarPacketParser.Network.Message;

namespace WildstarPacketParser.Parsing.Parsers;

public static class MiscHandler
{
    [Message(Opcodes.ClientPackedWorld)]
    public static void HandlePackedWorld(Packet packet)
    {
        packet.ReadByte(5);
        packet.ResetBits();

        uint length = packet.ReadUInt();
        byte[] data = packet.ReadBytes(length - 4);

        using (var stream = new MemoryStream(data))
        using (var reader = new Packet(stream))
        {
            var opcode = (Opcodes)reader.ReadUShort();
            packet.AddValue("Opcode", $"{Convert.ToInt64(opcode)} ({opcode})", "PackedWorld");
        }
    }
}
