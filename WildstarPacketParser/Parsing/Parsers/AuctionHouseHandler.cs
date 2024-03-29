﻿using WildstarPacketParser.Network;
using WildstarPacketParser.Network.Message;

namespace WildstarPacketParser.Parsing.Parsers;

public static class AuctionHouseHandler
{
    [Message(Opcodes.ServerOwnedItemAuctions)]
    public static void HandleOwnedItemsResponse(Packet packet)
    {
        var itemCount = packet.ReadUInt("ItemCount");
    }
}
