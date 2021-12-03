using WildstarPacketParser.Network;
using WildstarPacketParser.Network.Message;
using WildstarPacketParser.Parsing;

namespace WildstarPacketParser;

class Parser
{
    static readonly List<PacketStruct> _packets = new();

    static void Main(string[] args)
    {
        // if (args.Length < 1)
        // {
        //     Console.WriteLine("Please put in a file.");
        //     return;
        // }
        //
        // string sniff = args[0];
        // if (!File.Exists(sniff))
        // {
        //     Console.WriteLine("File does not exist, please put in a new file.");
        //     return;
        // }

        var sniff = "packetsniff.awps";

        Console.WriteLine("Wildstar Packet Parser");
        Console.WriteLine("Press Enter to Start");
        Console.Read();
        MessageManager.Initialise();

        using (var sr = new StreamReader(sniff))
        {
            while (!sr.EndOfStream)
            {
                var str = sr.ReadLine();
                var arr = str.Split(new char[] { ' ', ';' });

                var direction   = arr[3];
                var opcode      = (Opcodes)uint.Parse(arr[5]);
                var data        = arr[7].ToByteArray();

                _packets.Add(new PacketStruct
                {
                    Direction = direction,
                    Opcode = opcode,
                    Data = data
                });
            }
        }

        uint number = 0;
        using (var sw = new StreamWriter(sniff.Replace(".awps", "_parsed.txt")))
        {
            foreach (var packet in _packets)
            {
                // Console.WriteLine($"Parsing opcode: 0x{packet.Opcode:X4} ({(Opcodes)packet.Opcode})");
                PacketParser.ParsePacket(packet, number++, sw);
            }
        }
    }
}
