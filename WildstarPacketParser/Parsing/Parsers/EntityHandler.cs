using WildstarPacketParser.Constants;
using WildstarPacketParser.Network;
using WildstarPacketParser.Network.Message;

namespace WildstarPacketParser.Parsing.Parsers;

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

    public static void ReadCommandSetRotationSpline(Packet packet, params object[] idx)
    {
        packet.ReadUInt("SplineID", 32, idx);
        packet.ReadUShort("Speed", 16, idx);
        packet.ReadUInt("Position", 32, idx);
        packet.ReadByte("Mode", 4, idx);
        packet.ReadUInt("Offset", 32, idx);
        packet.ReadBool("AdjustSpeedToLength", idx);
    }

    public static void ReadCommandSetPositionPath(Packet packet, params object[] idx)
    {
        uint count = packet.ReadUShort("PositionCount", 10, idx);
        for (var i = 0; i < count; ++i)
            StaticHandler.ReadVector3(packet, idx, i, "Position");

        packet.ReadPackedFloat("Speed", idx);
        packet.ReadByte("Type", 2, idx);
        packet.ReadByte("Mode", 4, idx);
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
                    packet.ReadUInt("Mode", 32, i, command);
                    break;
                case EntityCommand.SetMoveDefaults:
                case EntityCommand.SetModeDefault:
                case EntityCommand.SetRotationDefaults:
                case EntityCommand.SetVelocityDefaults:
                    packet.ReadBool("Blend", i, command);
                    break;
                case EntityCommand.SetMove:
                    StaticHandler.ReadMoveData(packet, i, "MoveData", command);
                    packet.ReadBool("Blend", i, command);
                    break;
                case EntityCommand.SetPlatform:
                    packet.ReadUInt("UnitId", 32, i, command);
                    break;
                case EntityCommand.SetPosition:
                    StaticHandler.ReadVector3(packet, i, "Position", command);
                    packet.ReadBool("Blend", i, command);
                    break;
                case EntityCommand.SetTime:
                    packet.ReadUInt("Time", 32, i, command);
                    break;
                case EntityCommand.SetRotationFaceUnit:
                    packet.ReadUInt("UnitId", 32, i, command);
                    packet.ReadBool("Blend", i, command);
                    break;
                case EntityCommand.SetRotation:
                case EntityCommand.SetRotationFacePosition:
                    StaticHandler.ReadVector3(packet, i, "Position", command);
                    packet.ReadBool("Blend", i, command);
                    break;
                case EntityCommand.SetVelocity:
                    StaticHandler.ReadMoveData(packet, i, "VelocityData", command);
                    packet.ReadBool("Blend", i, command);
                    break;
                case EntityCommand.SetState:
                    packet.ReadUInt("State", 32, i, command);
                    break;
                case EntityCommand.SetRotationSpline:
                    ReadCommandSetRotationSpline(packet, i, command);
                    break;
                case EntityCommand.SetPositionPath:
                    ReadCommandSetPositionPath(packet, i, command);
                    break;
                case EntityCommand.SetStateKeys:
                case EntityCommand.SetModeKeys:
                    ReadCommandSetKeys(packet, i, command);
                    break;
                default:
                    packet.WriteLine($"[{i}] Unsuported EntityCommand: {command}");
                    break;
            }
        }
    }

    [Message(Opcodes.ServerEntityGroupAssociation)]
    public static void HandleEntityGroupAssociation(Packet packet)
    {
        packet.ReadUInt("UnitId");
        packet.ReadULong("GroupId");
    }
}
