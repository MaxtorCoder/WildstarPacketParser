using System;
using System.Collections.Generic;
using System.Text;
using WildstarPacketParser.Network;
using WildstarPacketParser.Network.Message;

namespace WildstarPacketParser.Parsing.Structures
{
    [Message(Opcodes.ServerEmote)]
    public class ServerEmote : IReadable
    {
        public uint GUID { get; set; }
        public uint StandState { get; set; }
        public uint EmoteId { get; set; }

        public void Read(GamePacketReader reader)
        {
            GUID        = reader.ReadUInt("GUID");
            StandState  = reader.ReadUInt("StandState", 5);
            EmoteId     = reader.ReadUInt("EmoteId");
        }
    }
}
