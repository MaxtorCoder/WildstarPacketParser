using System;
using System.Collections.Generic;
using System.Text;
using WildstarPacketParser.Network;

namespace WildstarPacketParser.Parsing.Parsers
{
    public static class StaticHandler
    {
        public static void HandleTargetIdentity(Packet packet, params object[] idx)
        {
            packet.ReadUShort("RealmId", 14u, idx);
            packet.ReadULong("CharacterId", 64u, idx);
        }
    }
}
