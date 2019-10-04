using System;
using System.Collections.Generic;
using System.Text;

namespace WildstarPacketParser.Network.Message
{
    public interface IReadable
    {
        void Read(Packet reader, string idx);
    }
}
