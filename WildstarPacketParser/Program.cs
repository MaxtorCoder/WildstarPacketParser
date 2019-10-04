using System;
using System.IO;
using System.Linq;
using WildstarPacketParser.Network;
using WildstarPacketParser.Network.Message;

namespace WildstarPacketParser
{
    class Parser
    {
        static void Main(string[] args)
        {
            // if (args.Length < 1)
            // {
            //     Console.WriteLine("Please put in a file.");
            //     return;
            // }

            string sniff = "party_inv_accept.awps";

            Console.WriteLine("Wildstar Packet Parser");
            Console.WriteLine("Press Enter to Start");
            Console.Read();
            MessageManager.Initialise();

            using (var sr = new StreamReader(sniff))
            using (var sw = new StreamWriter(sniff.Replace(".awps", "_parsed.txt")))
            {
                uint number = 0;
                while (!sr.EndOfStream)
                {
                    var str = sr.ReadLine();
                    var arr = str.Split(' ', ';');

                    var direction   = arr[3];
                    var opcode      = uint.Parse(arr[5]);
                    var data        = arr[7].ToByteArray();

                    Console.WriteLine($"Opcode: 0x{opcode:X4} ({(Opcodes)opcode})");

                    using (var stream = new MemoryStream(data))
                    using (var reader = new Packet(stream))
                    {
                        reader.AddHeader(direction, opcode, data.Length, number);
                        var message = MessageManager.GetMessageHandler((Opcodes)opcode);
                        if (message == null)
                            reader.Write(Extensions.ByteArrayToHexTable(data));
                        else
                            message.Invoke(reader);

                        sw.Write(reader.Writer);
                    }

                    number++;
                }
            }
        }
    }
}
