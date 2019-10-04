using System;
using System.Collections.Generic;
using System.Text;
using WildstarPacketParser.Constants;
using WildstarPacketParser.Network;
using WildstarPacketParser.Network.Message;

namespace WildstarPacketParser.Parsing.Parsers
{
    public class GroupHandler
    {
        [Message(Opcodes.ServerGroupJoin)]
        public static void HandleGroupJoin(Packet packet)
        {
            StaticHandler.HandleTargetIdentity(packet, "PlayerJoined");
            packet.ReadULong("GroupId");
            packet.ReadUInt("Unk0");
            var memberCount = packet.ReadUInt("MemberCount");
            packet.ReadUInt("MaxSize");

            packet.ReadByte("LootRuleNormal", 3u);
            packet.ReadByte("LootRuleThreshold", 3u);
            packet.ReadByte("LootThreshold", 4u);
            packet.ReadByte("LootRuleHarvest", 2u);

            for (int i = 0; i < memberCount; i++)
                HandleGroupMemberInfo(packet, i, "MemberInfo");

            StaticHandler.HandleTargetIdentity(packet, "LeaderIdentity");
            packet.ReadUShort("Realm", 14u);

            var unkCount = packet.ReadUInt("");
            for (var i = 0; i < unkCount; i++)
            {
                packet.ReadUInt("UnkGroupJoin1", 32u, i);
                packet.ReadUInt("UnkGroupJoin2", 32u, i);
            }
        }

        public static void HandleGroupMemberInfo(Packet packet, params object[] idx)
        {
            StaticHandler.HandleTargetIdentity(packet, idx, "MemberIdentity");
            packet.ReadUInt("Flags", 32u, idx);
            HandleGroupMember(packet, idx, "Member");
            packet.ReadUInt("GroupIndex", 32u, idx);
        }

        public static void HandleGroupMember(Packet packet, params object[] idx)
        {
            packet.ReadWideString("Name", idx);
            packet.ReadEnum<Faction>("Faction", 14u, idx);
            packet.ReadEnum<Race>("Race", 14u, idx);
            packet.ReadEnum<Class>("Class", 14u, idx);
            packet.ReadByte("Unk1", 2u, idx);
            packet.ReadByte("Level", 7u, idx);
            packet.ReadByte("Unk2", 7u, idx);
            packet.ReadEnum<Path>("Path", 3u, idx);
            packet.ReadUInt("Unk3", 17u, idx);
            packet.ReadUShort("GroupMemberId", 16u, idx);

            for (var i = 0; i < 5; i++)
            {
                packet.ReadUShort("Unk4", 16u, idx, i);
                packet.ReadByte("Unk5", 8u, idx, i);
            }

            StaticHandler.HandleTargetIdentity(packet, idx, "UnkIdentity");
            packet.ReadUInt("Unk6", 32u, idx);

            // Skip all these unks because fuck them
            for (var i = 0; i < 12; i++)
                packet.ReadUShort(string.Empty, 16, idx, i);

            packet.ReadUShort("Realm", 14u, idx);
            packet.ReadUShort("WorldZoneId", 15u, idx);
            packet.ReadUInt("Unk7", 32u, idx);
            packet.ReadUInt("Unk8", 32u, idx);
            packet.ReadBool("Unk9", idx);
            packet.ReadUInt("Unk10", 32u, idx);
            packet.ReadUInt("Unk11", 32u, idx);

            var unkCount = packet.ReadUInt("UnkCount", 32u, idx);
            for (var i = 0; i < unkCount / 8; i++)
            {
                packet.ReadUShort("Unk12", 15u, idx, i);
                packet.ReadUShort("Unk13", 16u, idx, i);
            }
        }

        [Message(Opcodes.ServerGroupLeave)]
        public static void HandleGroupLeave(Packet packet)
        {
            packet.ReadULong("GroupId");
            packet.ReadUInt("MemberId");
            StaticHandler.HandleTargetIdentity(packet, "TargetIdentity");
            packet.ReadEnum<RemoveReason>("RemoveReason", 4);
        }
    }
}
