using WildstarPacketParser.Network;
using WildstarPacketParser.Network.Message;

namespace WildstarPacketParser.Parsing;

public class PacketParser
{
    public static void ParsePacket(PacketStruct packet, uint number, StreamWriter sw)
    {
        using (var stream = new MemoryStream(packet.Data))
        using (var reader = new Packet(stream, packet.Opcode))
        {
            reader.AddHeader(packet.Direction, (uint)packet.Opcode, packet.Data.Length, number);

            var message = MessageManager.GetMessageHandler(packet.Opcode);
            if (message == null)
                reader.Write(Extensions.ByteArrayToHexTable(packet.Data));
            else
            {
                try
                {
                    message(reader);

                    if (reader.BytesRemaining > 0)
                    {
                        reader.WriteLine($"Packet not fully read! Current position: {reader.BytePosition} Length: {packet.Data.Length} Bytes remaining: {reader.BytesRemaining}.");

                        if (packet.Data.Length < 300)
                            reader.Write(Extensions.ByteArrayToHexTable(packet.Data));
                    }
                }
                catch (Exception ex)
                {
                    reader.WriteLine(ex.GetType().ToString());
                    reader.WriteLine(ex.Message);
                    reader.WriteLine(ex.StackTrace);
                }
            }

            sw.Write(reader.Writer);
        }
    }
}
