using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WildstarPacketParser.Network;
using WildstarPacketParser.Network.Message;

namespace WildstarPacketParser
{
    class Parser
    {
        static List<PacketStruct> packets = new List<PacketStruct>();
        static object syncObj = new object();

        static void Main(string[] args)
        {
            // if (args.Length < 1)
            // {
            //     Console.WriteLine("Please put in a file.");
            //     return;
            // }

            string sniff = @"E:\Wildstar\NexusForever\Source\NexusForever.WorldServer\bin\Debug\netcoreapp3.1\packetlog.awps";

            Console.WriteLine("Wildstar Packet Parser");
            Console.WriteLine("Press Enter to Start");
            Console.Read();
            MessageManager.Initialise();

            using (var sr = new StreamReader(sniff))
            using (var sw = new StreamWriter(sniff.Replace(".awps", "_parsed.txt")))
            {
                while (!sr.EndOfStream)
                {
                    var str = sr.ReadLine();
                    var arr = str.Split(' ', ';');

                    var direction   = arr[3];
                    var opcode      = uint.Parse(arr[5]);
                    var data        = arr[7].ToByteArray();

                    packets.Add(new PacketStruct
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
                foreach (var packet in packets)
                {
                    Console.WriteLine($"Parsing opcode: 0x{packet.Opcode:X4} ({(Opcodes)packet.Opcode})");

                    using (var stream = new MemoryStream(packet.Data))
                    using (var reader = new Packet(stream, (Opcodes)packet.Opcode))
                    {
                        reader.AddHeader(packet.Direction, packet.Opcode, packet.Data.Length, number);

                        var message = MessageManager.GetMessageHandler((Opcodes)packet.Opcode);
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

                    number++;
                }
            }
        }
    }
}
