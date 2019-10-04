using System;

namespace WildstarPacketParser.Network.Message
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageAttribute : Attribute
    {
        public Opcodes Opcode { get; }

        public MessageAttribute(Opcodes opcode)
        {
            Opcode = opcode;
        }
    }
}
