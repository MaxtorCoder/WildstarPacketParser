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
            StaticHandler.ReadTargetIdentity(packet, "PlayerJoined");
            packet.ReadULong("GroupId");
            packet.ReadUInt("Unk0");
            var memberCount = packet.ReadUInt("MemberCount");
            packet.ReadUInt("MaxSize");

            packet.ReadEnum<LootRule>("LootRuleNormal", 3u);
            packet.ReadEnum<LootThreshold>("LootRuleThreshold", 3u);
            packet.ReadEnum<LootThreshold>("LootThreshold", 4u);
            packet.ReadEnum<HarvestLootRule>("LootRuleHarvest", 2u);

            for (int i = 0; i < memberCount; i++)
                ReadMemberGroupInfo(packet, i, "MemberInfo");

            StaticHandler.ReadTargetIdentity(packet, "LeaderIdentity");
            packet.ReadUShort("Realm", 14u);

            var unkCount = packet.ReadUInt("");
            for (var i = 0; i < unkCount; i++)
            {
                packet.ReadUInt("UnkGroupJoin1", 32u, i);
                packet.ReadUInt("UnkGroupJoin2", 32u, i);
            }
        }

        public static void ReadMemberGroupInfo(Packet packet, params object[] idx)
        {
            StaticHandler.ReadTargetIdentity(packet, idx, "MemberIdentity");
            packet.ReadUInt("Flags", 32u, idx);
            ReadGroupMember(packet, idx, "Member");
            packet.ReadUInt("GroupIndex", 32u, idx);
        }

        public static void ReadGroupMember(Packet packet, params object[] idx)
        {
            packet.ReadWideString("Name", idx);
            packet.ReadEnum<Faction>("Faction", 14u, idx);
            packet.ReadEnum<Race>("Race", 14u, idx);
            packet.ReadEnum<Class>("Class", 14u, idx);
            packet.ReadByte("Unk1", 2u, idx);
            packet.ReadByte("Level", 7u, idx);
            packet.ReadByte("EffectiveLevel", 7u, idx);
            packet.ReadEnum<Path>("Path", 3u, idx);
            packet.ReadUInt("Unk3", 17u, idx);
            packet.ReadUShort("GroupMemberId", 16u, idx);

            for (var i = 0; i < 5; i++)
            {
                packet.ReadUShort("Unk4", 16u, idx, i);
                packet.ReadByte("Unk5", 8u, idx, i);
            }

            StaticHandler.ReadTargetIdentity(packet, idx, "MentoringTarget");
            packet.ReadUInt("Unk6", 32u, idx);

            // Skip all these unks because fuck them
            for (var i = 0; i < 12; i++)
                packet.ReadUShort(string.Empty, 16, idx, i);

            packet.ReadUShort("Realm", 14u, idx);
            packet.ReadUShort("WorldZoneId", 15u, idx);
            packet.ReadUInt("MapId", 32u, idx);
            packet.ReadUInt("PhaseId", 32u, idx);
            packet.ReadBool("SyncedToGroup", idx);
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
            StaticHandler.ReadTargetIdentity(packet, "TargetIdentity");
            packet.ReadEnum<RemoveReason>("RemoveReason", 4);
        }

        [Message(Opcodes.ServerGroupInviteResult)]
        public static void HandleGroupInviteResult(Packet packet)
        {
            packet.ReadULong("GroupId");
            packet.ReadWideString("TargetPlayerName");
            packet.ReadEnum<GroupResult>("Result", 5);
        }

        [Message(Opcodes.ServerGroupInviteReceived)]
        public static void HandleGroupInviteReceived(Packet packet)
        {
            packet.ReadULong("GroupId");
            packet.ReadUInt("Unk1");
            packet.ReadUInt("Unk2");
            packet.ReadUInt("Unk3");

            var memberCount = packet.ReadUInt("MemberCount");
            for (var i = 0; i < memberCount; ++i)
                ReadGroupMember(packet, i, "Member");
        }

        // [Message(Opcodes.ServerGroupStatUpdate)]
        // public static void HandleGroupStatUpdate(Packet packet)
        // {
        //     packet.ReadULong("GroupId");
        //     StaticHandler.HandleTargetIdentity(packet, "PlayerIdentity");
        //     packet.ReadUInt("Level", 7);
        //     packet.ReadUInt("EffectiveLevel", 7);
        // 
        //     packet.ReadUInt("Unk1", 17);
        //     packet.ReadUInt("Unk2", 16);
        // 
        //     for (var i = 0; i < 5; i++)
        //     {
        //         packet.ReadUShort("Unk3", 16u, i);
        //         packet.ReadByte("Unk4", 8u, i);
        //     }
        // 
        //     packet.ReadUShort("Health");
        //     packet.ReadUShort("HealthMax");
        //     packet.ReadUShort("Shield");
        //     packet.ReadUShort("ShieldMax");
        //     packet.ReadUShort("InterruptArmor");
        //     packet.ReadUShort("InterruptArmorMax");
        //     packet.ReadUShort("Absorption");
        //     packet.ReadUShort("AbsorptionMax");
        //     packet.ReadUShort("Mana");
        //     packet.ReadUShort("ManaMax");
        //     packet.ReadUShort("HealingAbsorb");
        //     packet.ReadUShort("HealingAbsorbMax");
        // 
        //     packet.ReadUInt("Unk5");
        //     packet.ReadUInt("Unk6");
        // 
        //     packet.ReadUInt("Unk7", 3);
        // }
    }
}
