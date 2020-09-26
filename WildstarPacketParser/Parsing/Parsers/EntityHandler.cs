using WildstarPacketParser.Constants;
using WildstarPacketParser.Network;
using WildstarPacketParser.Network.Message;

namespace WildstarPacketParser.Parsing.Parsers
{
    public static class EntityHandler
    {
        public static void ReadCommandSetKeys(Packet packet, params object[] idx)
        {
            uint count = packet.ReadByte("Count", 8, idx);

            for (var i = 0; i < count; ++i)
                packet.ReadUInt("Times", 32, i, idx);

            for (var i = 0; i < count; ++i)
                packet.ReadUInt("Modes", 32, i, idx);

            packet.ReadUInt("Offset", 32, idx);
        }

        public static void ReadCommandSetMoveKeys(Packet packet, EntityCommand command, params object[] idx)
        {
            uint count = packet.ReadUShort("Count", 10, idx);

            for (var i = 0; i < count; ++i)
                packet.ReadUInt("Times", 32, i, idx);

            for (var i = 0; i < count; ++i)
            {
                switch (command)
                {
                    case EntityCommand.SetModeKeys:
                        StaticHandler.ReadMoveData(packet, i, "MoveData");
                        break;
                    case EntityCommand.SetPositionKeys:
                        StaticHandler.ReadVector3(packet, i, "PositionData");
                        break;
                }
            }

            packet.ReadByte("Type", 2, idx);
            packet.ReadUInt("Offset", 32, idx);
            packet.ReadBool("Blend", idx);
        }

        [Message(Opcodes.ServerEntityCommand)]
        public static void HandleEntityCommand(Packet packet)
        {
            packet.ReadUInt("Guid");
            packet.ReadUInt("Time");
            packet.ReadBool("TimeReset");
            packet.ReadBool("ServerControlled");

            uint commandCount = packet.ReadUInt("CommandCount", 5);
            for (var i = 0; i < commandCount; ++i)
            {
                EntityCommand command = packet.ReadEnum<EntityCommand>("EntityCommand", 5, i);
                switch (command)
                {
                    case EntityCommand.SetMode:
                        packet.ReadUInt("Mode", 32, i);
                        break;
                    case EntityCommand.SetMoveDefaults:
                    case EntityCommand.SetModeDefault:
                        packet.ReadByte("Unused", 1, i);
                        break;
                    case EntityCommand.SetModeKeys:
                        ReadCommandSetKeys(packet, command, i, "ModeKeys");
                        break;
                    case EntityCommand.SetPositionKeys:
                        ReadCommandSetKeys(packet, command, i, "PositionKeys");
                        break;
                    case EntityCommand.SetMove:
                        StaticHandler.ReadMoveData(packet, i, "MoveData");
                        packet.ReadBool("Blend", i);
                        break;
                    case EntityCommand.SetPlatform:
                        packet.ReadUInt("UnitId", 32, i);
                        break;
                    case EntityCommand.SetPosition:
                        StaticHandler.ReadVector3(packet, i, "Position");
                        packet.ReadBool("Blend", i);
                        break;
                    case EntityCommand.SetTime:
                        packet.ReadUInt("Time", 32, i);
                        break;
                    case EntityCommand.SetRotationFaceUnit:
                        packet.ReadUInt("UnitId", 32, i);
                        packet.ReadBool("Blend", i);
                        break;
                    case EntityCommand.SetRotation:
                    case EntityCommand.SetRotationFacePosition:
                        StaticHandler.ReadVector3(packet, i, "Position");
                        packet.ReadBool("Blend", i);
                        break;
                    case EntityCommand.SetVelocity:
                        StaticHandler.ReadMoveData(packet, i, "VelocityData");
                        packet.ReadBool("Blend", i);
                        break;
                    case EntityCommand.SetState:
                        packet.ReadUInt("State", 32, i);
                        break;
                    default:
                        packet.WriteLine($"[{i}] Unsuported EntityCommand: {command}");
                        break;
                }
            }
        }
    }
}
